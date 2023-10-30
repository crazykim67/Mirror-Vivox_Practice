using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUI : MonoBehaviour
{
    [SerializeField]
    private Button mouseControllerBtn;
    [SerializeField]
    private Button keyboardMouseControllerBtn;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        switch (PlayerSettings.controllerType)
        {
            case EControllerType.Mouse:
                {
                    mouseControllerBtn.image.color = Color.green;
                    keyboardMouseControllerBtn.image.color = Color.white;
                    break;
                }
            case EControllerType.KeyboardMouse:
                {
                    mouseControllerBtn.image.color = Color.white;
                    keyboardMouseControllerBtn.image.color = Color.green;
                    break;
                }
        }
    }

    public void SetControlMode(int controlType)
    {
        PlayerSettings.controllerType = (EControllerType)controlType;
        switch(PlayerSettings.controllerType) 
        {
            case EControllerType.Mouse:
                {
                    mouseControllerBtn.image.color = Color.green;
                    keyboardMouseControllerBtn.image.color = Color.white;
                    break;
                }
            case EControllerType.KeyboardMouse:
                {
                    mouseControllerBtn.image.color = Color.white;
                    keyboardMouseControllerBtn.image.color = Color.green;
                    break;
                }
        }
    }

    #region Close

    public void OnClose()
    {
        StartCoroutine(OnCloseDelay());
    }

    public IEnumerator OnCloseDelay()
    {
        anim.SetTrigger("close");
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        anim.ResetTrigger("close");
    }

    #endregion
}
