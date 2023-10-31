using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginMenu : MonoBehaviour
{
    #region Instance

    private static LoginMenu instance;

    public static LoginMenu Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new LoginMenu();
                return instance;
            }

            return instance;
        }
    }

    #endregion

    [Header("Login")]
    public GameObject loginMenu;
    public TMP_InputField lg_idField;
    public TMP_InputField lg_passField;

    public TMP_Dropdown lg_dropdown;

    public Button loginBtn;
    public Button registerBtn;

    [Header("Register")]
    public GameObject registerMenu;
    public TMP_InputField rg_idField;
    public TMP_InputField rg_passField;
    public TMP_InputField rg_passCheckField;

    public TMP_Dropdown rg_dropdown;

    public Button rg_registerBtn;

    [Header("Warning Text")]
    public TextMeshProUGUI warningText;
    private float timer = 0f;
    private bool warning = false;

    private void Awake()
    {
        instance = this;

        // 회원가입 하러가기
        registerBtn.onClick.AddListener(OnRegisterMenu);
        // 회원가입 완료
        rg_registerBtn.onClick.AddListener(OnRegister);
        loginBtn.onClick.AddListener(OnLogin);
    }

    private void Update()
    {
        OnWarning();
        InputLoginTab();
        InputRegisternTab();
    }

    // 로그인 버튼
    public void OnLogin()
    {
        if (lg_idField.text.Equals(string.Empty))
        {
            OnWarningText("아이디 입력칸이 비어있습니다.");
            return;
        }
        else if (lg_passField.text.Equals(string.Empty))
        {
            OnWarningText("비밀번호 입력칸이 비어있습니다.");
            return;
        }

        AuthManager.Instance.OnLogin($"{lg_idField.text}@{rg_dropdown.options[rg_dropdown.value].text}", lg_passField.text);
    }

    // 회원가입 완료
    public void OnRegister()
    {
        // 아이디 입력 값 비어있음
        if (rg_idField.text.Equals(string.Empty))
        {
            OnWarningText("아이디 입력칸이 비어있습니다.");
            return;
        }
        // 아이디 입력 값 5자 미만이거나 16자 이상임
        else if (rg_idField.text.Length < 5 || rg_idField.text.Length > 15)
        {
            OnWarningText("아이디는 5자 이상 15자 이하여야 합니다.");
            rg_idField.text = "";
            return;
        }
        // 비밀번호 입력 값 비어있음
        else if (rg_idField.text.Equals(string.Empty))
        {
            OnWarningText("비밀번호 입력칸이 비어있습니다.");
            return;
        }
        // 비밀번호 입력 값 8자 미만이거나 15자 이상임
        else if (rg_passField.text.Length < 8 || rg_passField.text.Length > 14)
        {
            OnWarningText("비밀번호는 8자 이상 14자 이하여야 합니다.");
            rg_passField.text = "";
            return;
        }
        // 비밀번호 확인 입력 값이 비어있음
        else if (rg_passCheckField.text.Equals(string.Empty))
        {
            OnWarningText("비밀번호 확인 입력칸이 비어있습니다.");
            return;
        }
        // 비밀번호 확인 입력 값이 비밀번호 입력 값과 일치하지 않음
        else if (!rg_passCheckField.text.Equals(rg_passField.text))
        {
            OnWarningText("입력하신 비밀번호와 비밀번호 확인 칸이 일치하지 않습니다.");
            rg_passCheckField.text = "";
            return;
        }

        AuthManager.Instance.OnRegister($"{rg_idField.text}@{rg_dropdown.options[rg_dropdown.value].text}", rg_passField.text);

        OnLoginMenu();
    }

    // 회원가입 화면 세팅
    public void OnRegisterMenu()
    {
        loginMenu.SetActive(false);
        registerMenu.SetActive(true);
        rg_dropdown.value = 0;
        // 아이디 InputField 비우기
        lg_idField.text = "";
        lg_passField.text = "";

        // 회원가입 InputField 비우기
        rg_idField.text = "";
        rg_passField.text = "";
        rg_passCheckField.text = "";
    }

    // 로그인 화면 세팅
    public void OnLoginMenu()
    {
        loginMenu.SetActive(true);
        registerMenu.SetActive(false);
        lg_dropdown.value = 0;

        // 아이디 InputField 비우기
        lg_idField.text = "";
        lg_passField.text = "";

        // 회원가입 InputField 비우기
        rg_idField.text = "";
        rg_passField.text = "";
        rg_passCheckField.text = "";
    }

    public void OnWarning()
    {
        if (warning)
            if (timer <= 3)
            {
                timer += Time.deltaTime;
                warningText.gameObject.SetActive(true);
            }
            else
            {
                timer = 0;
                warning = false;
                warningText.text = "";
                warningText.gameObject.SetActive(false);
            }
    }

    public void OnWarningText(string str)
    {
        timer = 0;
        warningText.text = str;
        warning = true;
    }

    public void InputLoginTab()
    {
        if (!Input.GetKeyDown(KeyCode.Tab))
            return;
        if (!loginMenu.activeSelf)
            return;

        if (lg_idField.isFocused)
            lg_passField.Select();
        else if (lg_passField.isFocused)
            loginBtn.Select();
    }

    public void InputRegisternTab()
    {
        if (!Input.GetKeyDown(KeyCode.Tab))
            return;
        if (!registerMenu.activeSelf)
            return;

        if (rg_idField.isFocused)
            rg_passField.Select();
        else if (rg_passField.isFocused)
            rg_passCheckField.Select();
    }
}
