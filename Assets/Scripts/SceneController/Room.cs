using ExitGames.Client.Photon;
using Photon.Realtime;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Object:")]
    [SerializeField] private List<RoomItem> roomItems;

    public static Action<ExitGames.Client.Photon.Hashtable> roomSettingUpdateAction;
    public static Action<Player, ExitGames.Client.Photon.Hashtable> playerSettingInRoomUpdateAction;

    public static Action<Player> hasPlayerJoinRoomAction;
    public static Action<Player> hasPlayerLeaveRoomAction;

    private Dictionary<int, Player> playersInRoom;
    private int numberPlayerInRoom;

    private void Awake()
    {
        Lobby.joinRoomSuccessAction += ShowPlayerInRoom;

        roomSettingUpdateAction += OnRoomSettingUpdate;
        playerSettingInRoomUpdateAction += OnPlayerInRoomUpdate;
    }


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach(var item in roomItems)
        {
            item.Initialize();
            item.gameObject.SetActive(false);
        }
    }

    [Button]
    public static void LeaveRoom()
    {
        PhotonManager.LeaveRoom();
    }

    //NEED FIX IT
    private void ShowPlayerInRoom()
    {
        playersInRoom = PhotonManager.GetPlayerInRoom();
        numberPlayerInRoom = PhotonManager.GetNumberPlayerInRoom();
        int index = 1;
        foreach(var dictionary in playersInRoom)
        {
            RoomItem item = null;
            Player player = dictionary.Value;
            if(player.IsLocal)
            {
                //Check play is Local Player
                item = roomItems[0];
            }
            else
            {
                item = roomItems[index];
            }

            item.SetPlayer(player);
            item.gameObject.SetActive(true);
            index++;
        }
    }

    private void SettingRoom(string map, string mode)
    {
        PhotonManager.UpdateRoomProperties(map, mode);
    }

    private void ChangeTankLocalPlayer(string tankType)
    {
        PhotonManager.LocalPlayerChangeTankType(tankType);
    }

    private void LocalPlayerReadyAndUnready(bool value)
    {
        PhotonManager.LocalPlayerReadAndUnready(value);
    }

    #region Callback Methods
    private void OnRoomSettingUpdate(Hashtable obj)
    {
    }

    private void OnPlayerInRoomUpdate(Player arg1, Hashtable arg2)
    {
    }
    #endregion
}
