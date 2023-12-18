using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoginUI : BaseUI
{
    private Button loginBtn;
    private Button switchToRegisterBtn;

    private TextField usernameField;
    private TextField passwordField;

    private Label errorLable;

    public override void Initialize(UIData data = null)
    {
        base.Initialize(data);
        loginBtn = root.Q<Button>("LoginBtn");
        switchToRegisterBtn = root.Q<Button>("RegisterBtn");

        usernameField = root.Q<TextField>("Username");
        passwordField = root.Q<TextField>("Password");

        errorLable = root.Q<Label>("ErrorLable");

    }

    public override void Show(UIShowData data = null)
    {
        base.Show(data);

        loginBtn.clicked += Login;
        switchToRegisterBtn.clicked += SwithToRegister;

        PlayfabManager.ErrorMessageAction += SetErrorAlter;
    }

    public override void Hide()
    {
        loginBtn.clicked -= Login;
        switchToRegisterBtn.clicked -= SwithToRegister;

        PlayfabManager.ErrorMessageAction -= SetErrorAlter;
        base.Hide();
    }

    private void Login()
    {
        LoginRegister.loginAccountAction?.Invoke(usernameField.value, passwordField.value);
    }

    private void SwithToRegister()
    {
        LoginRegister.switchToRegister?.Invoke();
    }

    private void SetErrorAlter(string message)
    {
        errorLable.text = message;
    }
}
