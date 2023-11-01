using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;

public class VivoxWorldManager : MonoBehaviour
{
    public Vivox vivox = new Vivox();

    private static VivoxWorldManager instance;

    public static VivoxWorldManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new VivoxWorldManager();
                return instance;
            }

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;

        vivox.client = new Client();
        vivox.client.Uninitialize();
        vivox.client.Initialize();

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
                    Debug.Log("Login Successful");
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
        if (vivox.channelSession != null)
        {
            vivox.channelSession.Disconnect();
            //vivox.loginSession.DeleteChannelSession(new ChannelId(vivox.issuer, ui.channelName.text, vivox.domain, ChannelType.NonPositional));
        }

        vivox.client.Uninitialize();
    }
}
