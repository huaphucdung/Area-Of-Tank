using Photon.Pun;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("MainMenu Object:")]
    [SerializeField] private Transform tankPosition;
    
    [Header("Room Object:")]
    [SerializeField] private List<Transform> playerPosition;

    private void Awake()
    {
        LoginHandle.loginAccountAction += PlayfabManager.Login;
        LoginHandle.loginSuccessAction += OnLoginSuccess;

        RegiserHandle.registerAction += PlayfabManager.RegisterAccount;
        RegiserHandle.registerSuccessAction += OnRegisterSuccess;
    }

    private void Start()
    {
        if(!PhotonManager.IsConnected())
        {
            ShowLoginUI();
        }
    }

    private void ShowLoginUI()
    { 
    }

    #region Callback Methods
    private void OnLoginSuccess(string playerID)
    {
        Debug.Log(playerID);
        PhotonManager.ConnectServer(playerID);
    }
    private void OnRegisterSuccess()
    {
        Debug.Log("Register Success");
        ShowLoginUI();
    }
    #endregion

    //Test
    [Button]
    private void TestLogin(string username, string password)
    {
        LoginHandle.loginAccountAction?.Invoke(username, password);
    }

    [Button]
    private void TestRegister(string displayName, string username, string password)
    {
        RegiserHandle.registerAction?.Invoke(displayName, username, password);
    }

    [Button]
    private void TestConnectServer()
    {
        Debug.Log(PhotonManager.IsConnected());
        Debug.Log(PhotonNetwork.LocalPlayer.UserId);
    }
}

public class LoginHandle
{
    public static Action<string, string> loginAccountAction;
    public static Action<string> loginSuccessAction;
}

public class RegiserHandle
{
    public static Action<string, string, string> registerAction;
    public static Action registerSuccessAction;
}