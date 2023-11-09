using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;

public class PlayfabManager : MonoBehaviour
{
    public static void Login(string username, string password)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }

    public static void RegisterAccount(string displayName, string username, string password)
    {
        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = displayName,
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false,
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRigisterSuccess, OnError);
    }

    private static void OnLoginSuccess(LoginResult obj)
    {
        LoginHandle.loginSuccessAction?.Invoke(obj.InfoResultPayload.PlayerProfile.PlayerId);
    }
    private static void OnRigisterSuccess(RegisterPlayFabUserResult obj)
    {
        RegiserHandle.registerSuccessAction?.Invoke();
    }

    

    private static void OnError(PlayFabError obj)
    {
        Debug.Log(obj.ErrorMessage);
    }

    private void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataRecieved, OnError);
    }

    private void OnDataRecieved(GetUserDataResult obj)
    {
       
    }

    private void SavePlayerData()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }

    private void OnDataSend(UpdateUserDataResult obj)
    {
        
    }
}
