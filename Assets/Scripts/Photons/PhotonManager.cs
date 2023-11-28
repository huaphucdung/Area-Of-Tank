using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static string roomNameCreate;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    #region Connecet
    public static void ConnectServer(string userID)
    {
        if (!PhotonNetwork.IsConnected)
        {
            /*PhotonNetwork.AutomaticallySyncScene = true;*/
            PhotonNetwork.GameVersion = "v1";
            PhotonNetwork.AuthValues = new AuthenticationValues(userID);
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("On Connected");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("On Disconected");
    }

    public static bool IsConnected()
    {
        return PhotonNetwork.IsConnected;
    }
    #endregion

    #region Lobby
    public static void JoinLobby()
    {        
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public static void LeaveLobby()
    {
        if (!PhotonNetwork.IsConnected || !IsInLobby()) return;
        PhotonNetwork.LeaveLobby();
    }

    public static void CreateRoom(string roomName, string map, string mode)
    {
        roomNameCreate = roomName;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                {"map", map},
                {"mode", mode}
            }
        });
    }

    public static void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public static void JoinRoomRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public static bool IsInLobby()
    {
        return PhotonNetwork.InLobby;
    }

    public override void OnCreatedRoom()
    {
        Lobby.createRoomSuccessAction?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {  
    }

    public override void OnJoinedLobby()
    {
        Lobby.joinLobbySuccessAction?.Invoke();
    }

    public override void OnLeftLobby()
    {
        Lobby.leaveRoomAction?.Invoke();
    }

    
    #endregion

    #region Room
    public static void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public override void OnJoinedRoom()
    {
        Lobby.joinRoomSuccessAction?.Invoke();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Lobby.joinRoomFailseAction?.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Lobby.joinRoomFailseAction?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public override void OnLeftRoom()
    { 
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public static bool IsInRoom()
    {
        return PhotonNetwork.InRoom;
    }
    #endregion

    #region Player
    public static Dictionary<int, Player> GetPlayerInRoom()
    {
        if (!IsInRoom()) return null;
        return PhotonNetwork.CurrentRoom.Players;
    }

    public static int GetNumberPlayerInRoom()
    {
        if (!IsInRoom()) return 0;
        return PhotonNetwork.CurrentRoom.PlayerCount;
    }
    #endregion
}
