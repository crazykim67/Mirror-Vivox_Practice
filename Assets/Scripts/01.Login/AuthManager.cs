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
                    Debug.Log(_id + " 로 로그인 하셨습니다.");
                }
                else if(task.IsFaulted)
                {
                    LoginMenu.Instance.OnWarningText("아이디가 존재하지 않거나 비밀번호가 일치하지 않습니다");
                }
            }
        );
    }

    public void OnRegister(string _id, string _pass)
    {
        // 제공되는 함수 : 이메일과 비밀번호로 회원가입 시켜 줌
        auth.CreateUserWithEmailAndPasswordAsync($"{_id}", _pass).ContinueWith(
            task => {
                if (!task.IsCanceled && !task.IsFaulted)
                {
                    Debug.Log(_id + "로 회원가입\n");
                }
                else
                    Debug.Log("회원가입 실패\n");
            }
            );
    }

}
