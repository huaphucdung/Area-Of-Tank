using Cinemachine;
using Photon.Pun;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera roomCamera;

    public static Action joinLobbySuccessAction;

    public static Action createRoomSuccessAction;

    public static Action joinRoomSuccessAction;
    public static Action joinRoomFailseAction;
    public static Action leaveRoomAction;

    private static ExitGames.Client.Photon.Hashtable defaultData = new ExitGames.Client.Photon.Hashtable
        {
            {"TankType", TankType.TankT34},
        };

    private void Awake()
    {
        joinLobbySuccessAction += OnJoinLooby;

        createRoomSuccessAction += OnCreateRoom;

        joinRoomSuccessAction += OnJoinRoomSuccess;
        joinRoomFailseAction += OnJoinRoomFailse;
        leaveRoomAction += OnLeaveRoom;
    }


    [Button]
    public static void LeaveLobby()
    {
        PhotonManager.LeaveLobby();
    }

    [Button]
    public static void JoinRoomRandom()
    {
        PhotonManager.JoinRoomRandom();
    }

    [Button]
    public static void JoinRoom(string roomName)
    {
        SetDefaultPlayerData();
        PhotonManager.JoinRoom(roomName);
    }

    [Button]
    public static void CreateRoom(string roomName, string map, string mode)
    {
        SetDefaultPlayerData();
        PhotonManager.CreateRoom(roomName, map, mode);
    }


    private static void SetDefaultPlayerData()
    {
        PhotonNetwork.LocalPlayer.CustomProperties = defaultData;
    }

    #region Callback Methods
    private void OnJoinLooby()
    {
        Debug.Log("Join Lobby Success");
    }

    private void OnCreateRoom()
    {
        Debug.Log("Create Room Success");
    }

    private void OnJoinRoomSuccess()
    {
        Debug.Log("Join Room Success");
        roomCamera.gameObject.SetActive(true);
        //Medthod default data when join new room
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
}
