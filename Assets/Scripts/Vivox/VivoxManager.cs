﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;
using UnityEngine.Android;

[Serializable]
public class Vivox
{
    public Client client;

    public Uri server = new Uri("https://unity.vivox.com/appconfig/14568-mirro-31916-udash");
    public string issuer = "14568-mirro-31916-udash";
    public string domain = "mtu1xp.vivox.com";
    public string tokenKey = "2gNz3YoVorL6W5nqGxUisxOsnYNG2yMf";
    public TimeSpan timespan = TimeSpan.FromSeconds(90);

    public ILoginSession loginSession;
    public IChannelSession channelSession;

    public IAudioDevices audioInputDevice;
    public IAudioDevices audioOutputDevice;
}

public class VivoxManager : MonoBehaviour
{
    public Vivox vivox = new Vivox();

    private static VivoxManager instance;

    public static VivoxManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new VivoxManager();
                return instance;
            }

            return instance;
        }
    }
    
    public UIManager ui;

    private void Awake()
    {
        instance = this;

        vivox.client = new Client();
        vivox.client.Uninitialize();
        vivox.client.Initialize();
        SetAudioDevices();
        DontDestroyOnLoad(this.gameObject);
    }

    public void Login(string userName)
    {
        AccountId accoundId = new AccountId(vivox.issuer, userName, vivox.domain);
        vivox.loginSession = vivox.client.GetLoginSession(accoundId);

        // 공식 문서에서 TryCatch 사용이 무난하다고 함.
        vivox.loginSession.BeginLogin(vivox.server, vivox.loginSession.GetLoginToken(vivox.tokenKey, vivox.timespan), 
            callback =>
            {
                try
                {
                    vivox.loginSession.EndLogin(callback);
                    ui.InputChat("로그인 완료");
                    Debug.Log("Login Successful");
                }
                catch(Exception e) 
                { 
                    Debug.LogException(e);
                    throw;
                }
            });
    }

    public void JoinChannel(string channelName, ChannelType channelType)
    {
        ChannelId channelId = new ChannelId(vivox.issuer, channelName, vivox.domain, channelType);
        vivox.channelSession = vivox.loginSession.GetChannelSession(channelId);

        UserCallBacks(true, vivox.channelSession);
        ChannelCallBack(true, vivox.channelSession);
        vivox.channelSession.BeginConnect(true, true, true, vivox.channelSession.GetConnectToken(vivox.tokenKey, vivox.timespan), callback =>
        {
            try
            {
                vivox.channelSession.EndConnect(callback);
                ui.InputChat("채널 접속 완료");
            }
            catch (Exception e) 
            {
                Debug.LogException(e); 
                throw;
            }
        });
    }

    private void OnApplicationQuit()
    {
        if(vivox.channelSession != null)
        {
            vivox.channelSession.Disconnect();
            vivox.loginSession.DeleteChannelSession(new ChannelId(vivox.issuer, ui.channelName.text, vivox.domain, ChannelType.NonPositional));
        }

        vivox.client.Uninitialize();
    }

    #region 사용자 채팅 관련 콜백

    // 채널 접속 콜백
    public void ChannelCallBack(bool bind, IChannelSession session)
    {
        if (bind)
        {
            session.MessageLog.AfterItemAdded += ReciveMessage;
        }
        else
        {
            session.MessageLog.AfterItemAdded -= ReciveMessage;
        }
    }

    // 메세지 보내기
    public void SendMessage(string str)
    {
        vivox.channelSession.BeginSendText(str, callback =>
        {
            try
            {
                vivox.channelSession.EndSendText(callback);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        });
    }

    // 메세지 받기
    public void ReciveMessage(object sender, QueueItemAddedEventArgs<IChannelTextMessage> queue)
    {
        var name = queue.Value.Sender.Name;
        var message = queue.Value.Message;

        ui.InputChat($"{name} : {message}");
    }

    #endregion

    #region 사용자 참여, 나가기 관련 콜백

    // 사용자 참여, 나가기 콜백
    public void UserCallBacks(bool bind, IChannelSession session)
    {
        if (bind)
        {
            vivox.channelSession.Participants.AfterKeyAdded += AddUser;
            vivox.channelSession.Participants.BeforeKeyRemoved += LeaveUser;
        }
        else
        {
            vivox.channelSession.Participants.AfterKeyAdded -= AddUser;
            vivox.channelSession.Participants.BeforeKeyRemoved -= LeaveUser;
        }
    }

    // 사용자 추가
    public void AddUser(object sender, KeyEventArg<string> userData)
    {
        var temp = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = temp[userData.Key];
        ui.InputChat($"{user.Account.Name} 님이 채널에 접속했습니다. ");
        ui.InputUser(user);
    }

    // 사용자 나감
    public void LeaveUser(object sender, KeyEventArg<string> userData)
    {
        var temp = (VivoxUnity.IReadOnlyDictionary<string, IParticipant>)sender;

        IParticipant user = temp[userData.Key];
        ui.InputChat($"{user.Account.Name} 님이 채널에 나갔습니다. ");
        ui.LeaveUser(user);
    }

    #endregion

    #region 오디오 관련

    // 오디오 장치 세팅
    public void SetAudioDevices(/*IAudioDevice targetInput = null, IAudioDevice targetOutput = null*/)
    {
#if UNITY_ANDROID || UNITY_IOS
        if(!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Microphone))
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Microphone);
#endif

        vivox.audioInputDevice = vivox.client.AudioInputDevices;
        vivox.audioOutputDevice = vivox.client.AudioOutputDevices;
        CheckAudioDevice();
    }

    // 오디오 장치 검사
    public void CheckAudioDevice()
    {
        var inTemp = vivox.audioInputDevice.ActiveDevice;
        var outTemp = vivox.audioOutputDevice.ActiveDevice;

        Debug.Log($"Input Device : {inTemp.Name}");
        Debug.Log($"Output Device : {outTemp.Name}");
    }

    public void MuteOtherUser(IParticipant _user, bool isMute)
    {
        if (vivox.channelSession == null)
            return;

        // sip 세션 프로토콜
        string constructedParticipantKey = "sip:." + vivox.issuer + "." + _user.Account.Name + ".@" + vivox.domain;
        var participants = vivox.channelSession.Participants;

        if (participants[constructedParticipantKey].InAudio)
                participants[constructedParticipantKey].LocalMute = isMute;
        else
            // 상대방 오디오 장치 문제 발견
            Debug.Log("Try Again");
    }

    #endregion
}
