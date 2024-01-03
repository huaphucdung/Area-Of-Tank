using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Lobby : MonoBehaviourPunCallbacks
{
    [SerializeField] private CinemachineVirtualCamera roomCamera;
    [SerializeField] private PostProcessVolume postProcessVolume;
    public static Action<List<RoomInfo>> roomListChangeAction;

    public static Action joinLobbySuccessAction;

    public static Action leftLobbySuccessAction;

    public static Action createRoomSuccessAction;

    public static Action joinRoomSuccessAction;
    public static Action joinRoomFailseAction;
    public static Action leaveRoomAction;

    private LobbyUI ui;

    private static ExitGames.Client.Photon.Hashtable defaultData = new ExitGames.Client.Photon.Hashtable
        {
            {"TankType", "TankT34"},
            {"Ready", false},
        };

    private void Awake()
    {
        roomListChangeAction += OnRoomListUpdates;

        joinLobbySuccessAction += OnJoinLooby;
        leftLobbySuccessAction += OnLeftLobby;

        createRoomSuccessAction += OnCreateRoom;

        joinRoomSuccessAction += OnJoinRoomSuccess;
        joinRoomFailseAction += OnJoinRoomFailse;
        leaveRoomAction += OnLeaveRoom;
    }

    
    private void OnRoomListUpdates(List<RoomInfo> roomList)
    {
        if (ui == null) return;
        ui.ShowListRoom(roomList);
    }

    public static void LeaveLobby()
    {
        PhotonManager.LeaveLobby();
    }

    public static void JoinRoomRandom()
    {
        PhotonManager.JoinRoomRandom();
    }

    public static void JoinRoom(string roomName)
    {
        SetDefaultPlayerData();
        PhotonManager.JoinRoom(roomName);
    }

    public static void CreateRoom(string roomName, string map, string mode)
    {
        SetDefaultPlayerData();
        PhotonManager.CreateRoom(roomName, map, mode);
    }


    private static void SetDefaultPlayerData()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(defaultData);
    }

    #region Callback Methods
    private void OnJoinLooby()
    {
        ShowUI();
    }

  
    private void OnLeftLobby()
    {
        HideUI();
        MainMenu.ShowMainMenuUI?.Invoke();
    }

    private void OnCreateRoom()
    {
        Debug.Log("Create Room Success");
    }

    private void OnJoinRoomSuccess()
    {
        HideUI();
        roomCamera.gameObject.SetActive(true);
    }

    private void OnJoinRoomFailse()
    {
        //Show dialouge
    }

    private void OnLeaveRoom()
    {
        roomCamera.gameObject.SetActive(false);
    }
    #endregion

    private void ShowUI()
    {
        DepthOfField pr;
        if (postProcessVolume.sharedProfile.TryGetSettings<DepthOfField>(out pr))
        {
            pr.focusDistance.overrideState = true;
        }
        ui = UIManager.GetAndShowUI<LobbyUI>();
    }

    private void HideUI()
    {
        DepthOfField pr;
        if (postProcessVolume.sharedProfile.TryGetSettings<DepthOfField>(out pr))
        {
            pr.focusDistance.overrideState = false;
        }
        ui.Hide();
    }
}
