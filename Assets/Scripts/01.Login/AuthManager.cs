using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using UnityEngine.UI;
using TMPro;

public class AuthManager : MonoBehaviour
{
    #region Instance

    private static AuthManager instance;

    public static AuthManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new AuthManager();
                return instance;
            }

            return instance;
        }
    }

    #endregion

    private Firebase.Auth.FirebaseAuth auth;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    public void OnLogin(string _id, string _pass)
    {
        auth.SignInWithEmailAndPasswordAsync($"{_id}", _pass).ContinueWith(
            task => {
                if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                {
                    Debug.Log(_id + " �� �α��� �ϼ̽��ϴ�.");
                }
                else if(task.IsFaulted)
                {
                    LoginMenu.Instance.OnWarningText("���̵� �������� �ʰų� ��й�ȣ�� ��ġ���� �ʽ��ϴ�");
                }
            }
        );
    }

    public void OnRegister(string _id, string _pass)
    {
        // �����Ǵ� �Լ� : �̸��ϰ� ��й�ȣ�� ȸ������ ���� ��
        auth.CreateUserWithEmailAndPasswordAsync($"{_id}", _pass).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(_id + "�� ȸ������\n");
                }
                else
                    Debug.Log("ȸ������ ����\n");
            }
            );
    }

}
