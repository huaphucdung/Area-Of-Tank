using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using MEC;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static string roomNameCreate;

    private bool inLobby;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    #region Connecet
    public static void ConnectServer(string userID)
    {
        if (!PhotonNetwork.IsConnected)
        {

            PhotonNetwork.GameVersion = "v1";
            PhotonNetwork.AuthValues = new AuthenticationValues(userID);
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("On Connected");
        if (inLobby) PhotonNetwork.JoinLobby();
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
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Lobby.roomListChangeAction?.Invoke(roomList);
    }

    public static void JoinLobby()
    {
        if (!PhotonNetwork.IsConnected) return;
        PhotonNetwork.JoinLobby();
    }

    public static void LeaveLobby()
    {
        if (!PhotonNetwork.IsConnected || !IsInLobby()) return;
        PhotonNetwork.LeaveLobby();
    }

    public static void CreateRoom(string roomName, string map, string mode)
    {
        roomNameCreate = roomName;
        RoomOptions option = new RoomOptions()
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true,
            PublishUserId = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                {"map", map},
                {"mode", mode }
            },
            CustomRoomPropertiesForLobby = new string[] { "map", "mode" }
        };

        PhotonNetwork.CreateRoom(roomName, option, TypedLobby.Default);
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
        Lobby.leftLobbySuccessAction?.Invoke();
    }


    #endregion

    #region Room
    public static Photon.Realtime.Room GetCurrentRoom()
    {
        return PhotonNetwork.CurrentRoom;
    }

    public static void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public static void UpdateRoomProperties(string map, string mode)
    {
        PhotonNetwork.CurrentRoom.CustomProperties.Add("map", map);
        PhotonNetwork.CurrentRoom.CustomProperties.Add("mode", mode);
    }

    public static void LocalPlayerChangeTank(string tankType)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
            {"Ready", (bool) GetLocalPlayer().CustomProperties["Ready"]},
            {"TankType", tankType}
        });
    }

    public static void LocalPlayerChangeReady(bool value)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() {
            {"Ready", value},
            {"TankType", (string) GetLocalPlayer().CustomProperties["TankType"]}
        });
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Room.roomSettingUpdateAction?.Invoke(propertiesThatChanged);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("Update");
        Room.playerSettingInRoomUpdateAction?.Invoke(targetPlayer, changedProps);
    }



    public override void OnJoinedRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
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

    public override void OnLeftRoom()
    {
        inLobby = true;
        PhotonNetwork.AutomaticallySyncScene = false;
        Lobby.leaveRoomAction?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Room.hasPlayerJoinRoomAction?.Invoke(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Room.hasPlayerLeaveRoomAction?.Invoke(otherPlayer);
    }

    public static bool IsInRoom()
    {
        return PhotonNetwork.InRoom;
    }

    public static bool IsHost()
    {
        return PhotonNetwork.IsMasterClient;
    }


    #endregion

    #region Player
    public static Player GetLocalPlayer()
    {
        return PhotonNetwork.LocalPlayer;
    }

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

    public static void LoadScene(string nameScene)
    {
        PhotonNetwork.LoadLevel(nameScene);
    }
}
