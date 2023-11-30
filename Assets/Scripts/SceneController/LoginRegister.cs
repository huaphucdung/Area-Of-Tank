using Photon.Pun;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class LoginRegister : MonoBehaviour
{
    public static Action<string, string> loginAccountAction;
    public static Action<string> loginSuccessAction;

    public static Action<string, string, string> registerAction;
    public static Action registerSuccessAction;

    private void Start()
    {
        loginAccountAction += PlayfabManager.Login;
        loginSuccessAction += OnLoginSuccess;

        registerAction += PlayfabManager.RegisterAccount;
        registerSuccessAction += OnRegisterSuccess;
    }

 
    //Test
    [Button]
    private void TestLogin(string username, string password)
    {
        loginAccountAction?.Invoke(username, password);
    }

    [Button]
    private void TestRegister(string displayName, string username, string password)
    {
        registerAction?.Invoke(displayName, username, password);
    }

    [Button]
    private void TestConnectServer()
    {
        Debug.Log(PhotonManager.IsConnected());
        Debug.Log(PhotonNetwork.LocalPlayer.UserId);
    }

    #region Callback Methods
    private void OnLoginSuccess(string playerID)
    {
        Debug.Log("Login Success");
        PhotonManager.ConnectServer(playerID);
        //Load Data
        PlayfabManager.GetPlayerData();

    }
    private void OnRegisterSuccess()
    {
        Debug.Log("Register Success");
        //Save Default Data
        ResourceManager.SavePlayerData();
    }
    #endregion
}
