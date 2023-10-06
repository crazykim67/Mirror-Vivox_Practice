using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

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

    private void Awake()
    {
        instance = this;

        vivox.client = new Client();
        vivox.client.Uninitialize();
        vivox.client.Initialize();

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Login();
    }

    public void Login()
    {
        string userName = "Tester";
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
                catch(Exception e) 
                { 
                    Debug.LogException(e);
                    throw;
                }
            });
    }

    private void OnApplicationQuit()
    {
        vivox.client.Uninitialize();
    }
}
