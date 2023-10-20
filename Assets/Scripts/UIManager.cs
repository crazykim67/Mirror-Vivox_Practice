using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivoxUnity;

public class UIManager : MonoBehaviour
{
    public TMP_InputField userName;
    public TMP_InputField channelName;
    public TMP_InputField message;

    public UserData userItem;
    public Transform userPos;

    public GameObject chatItem;
    public Transform chatPos;

    public List<UserData> userList = new List<UserData>();

    public void LoginBtn()
    {
        VivoxManager.Instance.Login(userName.text);
    }

    public void LogOutBtn()
    {
        VivoxManager.Instance.vivox.loginSession.Logout();
        InputChat("로그아웃 완료");
    }

    public void JoinChannelBtn()
    {
        VivoxManager.Instance.JoinChannel(channelName.text, ChannelType.NonPositional);
    }

    public void LeaveChannelBtn()
    {
        VivoxManager.Instance.vivox.channelSession.Disconnect();
        // 채널에 나갔으면 로그인 세션에서도 채널에 관한 정보를 지워줘야 함.
        // 기존에 쓰던 Channel Id를 써도 상관없지만 현재 VivoxManager 와 분리 돼있기 때문에 따로 생성.
        VivoxManager.Instance.vivox.loginSession.DeleteChannelSession(new ChannelId(VivoxManager.Instance.vivox.issuer, channelName.text, VivoxManager.Instance.vivox.domain, ChannelType.NonPositional));
        InputChat("채널 접속 끊기");
    }

    public void InputChat(string str)
    {
        var temp = Instantiate(chatItem, chatPos);
        temp.GetComponent<TextMeshProUGUI>().text = str;
    }

    public void InputUser(IParticipant userData)
    {
        var temp = Instantiate(userItem, userPos);
        temp.SetUserData(userData);

        userList.Add(temp);
    }

    public void LeaveUser(IParticipant userData) 
    { 
        foreach(var user in userList)
        {
            if (user.user.Key.Equals(userData.Key))
            {
                Destroy(user.gameObject);
                userList.Remove(user);
                break;
            }
        }
    }

    public void MessageBtn()
    {
        VivoxManager.Instance.SendMessage(message.text);
        message.text = "";
    }

    public void AllMute()
    {

    }
}
