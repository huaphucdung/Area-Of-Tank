using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RegisterUI : BaseUI
{
    private TextField playerNameField;
    private TextField userNameField;
    private TextField passwordField;
    private TextField confirmPasswordField;

    private Label errorLable;

    private Button registerBtn;
    private Button switchToLoginBtn;

    public override void Initialize(UIData data = null)
    {
        base.Initialize(data);

        playerNameField = root.Q<TextField>("Name");
        userNameField = root.Q<TextField>("Username");
        passwordField = root.Q<TextField>("Password");
        confirmPasswordField = root.Q<TextField>("ConfirmPassword");

        errorLable = root.Q<Label>("ErrorLable");

        registerBtn = root.Q<Button>("RegisterBtn");
        switchToLoginBtn = root.Q<Button>("BackToLoginBtn");
    }

    public override void Show(UIShowData data = null)
    {
        base.Show(data);
        registerBtn.clicked += Register;
        switchToLoginBtn.clicked += SwitchToLogin;

        PlayfabManager.ErrorMessageAction += SetErrorAlter;
    }

    public override void Hide()
    {
        registerBtn.clicked -= Register;
        switchToLoginBtn.clicked -= SwitchToLogin;

        PlayfabManager.ErrorMessageAction -= SetErrorAlter;
        base.Hide();
    }

    private void Register()
    {
        Debug.Log(1);
        if (!(passwordField.value).Equals(confirmPasswordField.value))
        {
            SetErrorAlter("Not correct");
            return;
        }

        Debug.Log(2);
        LoginRegister.registerAction?.Invoke(playerNameField.value, userNameField.value, passwordField.value);
    }

    private void SwitchToLogin()
    {
        LoginRegister.switchToLogin?.Invoke();
    }

    private void SetErrorAlter(string message)
    {
        errorLable.text = message;
    }
}
