using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VivoxUnity;

public class UserData : MonoBehaviour
{
    public IParticipant user;

    public TextMeshProUGUI userName;

    public Toggle muteBox;

    public void Awake()
    {
        muteBox.onValueChanged.AddListener((mute) => UserMute(mute));
    }

    public void SetUserData(IParticipant _user)
    {
        user = _user;
        userName.text = _user.Account.Name;
    }

    public void UserMute(bool isMute)
    {
        if (user == null || VivoxManager.Instance == null)
            return;
        user.SetIsMuteForAll() = isMute;
        VivoxManager.Instance.MuteOtherUser(user, isMute);
    }
}
