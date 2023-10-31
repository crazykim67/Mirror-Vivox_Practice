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
}
