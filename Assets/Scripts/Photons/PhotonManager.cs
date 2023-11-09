using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static string roomNameCreate;

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
        PhotonNetwork.JoinLobby();
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

    public override void OnCreatedRoom()
    {
        JoinRoom(roomNameCreate);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        
    }

    public static void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public static void JoinRoomRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public static bool InLooby()
    {
        return PhotonNetwork.InLobby;
    }
    #endregion

    #region Room
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public override void OnJoinedRoom()
    {
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
}
