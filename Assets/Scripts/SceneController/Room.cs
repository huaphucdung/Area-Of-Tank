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

    
    public static Func<bool> readyAction;

    private Dictionary<Player, RoomItem> playerItems;
    private int numberPlayerInRoom;
    private bool ready;
    private RoomUI ui;

    private int index = 1;

    private void Awake()
    {
        Lobby.joinRoomSuccessAction += OnJoinRoom;
        Lobby.leaveRoomAction += OnLeftRoom;
        roomSettingUpdateAction += OnRoomSettingUpdate;
        playerSettingInRoomUpdateAction += OnPlayerInRoomUpdate;

        hasPlayerJoinRoomAction += OnNewPlayerEnter;
        hasPlayerLeaveRoomAction += OnPlayerLeft;

        readyAction += SwapReady;
    }

   
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        playerItems = new Dictionary<Player, RoomItem>();
        foreach (var item in roomItems)
        {
            item.Initialize();
            item.SetEmpty();
        }
    }

    private void ShowUI()
    {
        ui = UIManager.GetAndShowUI<RoomUI>(null, new UIShowRoomData()
        {
            currentRoom = PhotonManager.GetCurrentRoom()
        });
        ready = false;
    }

    private void HideUI()
    {
        ui.Hide();
    }

    public static void LeaveRoom()
    {
        PhotonManager.LeaveRoom();
    }

    //NEED FIX IT
    private void ShowPlayerInRoom()
    {
        playerItems.Clear();
        foreach (var item in roomItems)
        {
            item.SetEmpty();
        } 
        numberPlayerInRoom = PhotonManager.GetNumberPlayerInRoom();
        index = 1;
        foreach (KeyValuePair<int, Player> dictionary in PhotonManager.GetPlayerInRoom())
        {
            Player player = dictionary.Value;
            Debug.Log(player.UserId);
            if (player.IsLocal)
            {
                //Check play is Local Player
                AddPlayer(player, 0);

            }
            else
            {
                AddPlayer(player, index);
                index++;
            }
        }
    }

    private void AddPlayer(Player player, int position)
    {
        RoomItem item = roomItems[position];
        
        ui.AddPlayerValue(player, position);
        item.gameObject.SetActive(true);
        item.SetPlayer(player);
        playerItems.Add(player, item);
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

    private void OnJoinRoom()
    {
        ShowUI();
        ShowPlayerInRoom();
    }

    private void OnLeftRoom()
    {
        HideUI();
    }

    private void OnNewPlayerEnter(Player player)
    {
        AddPlayer(player, index);
        index++;
    }

    private void OnPlayerLeft(Player player)
    {
        ShowPlayerInRoom();
    }

    #endregion

    private bool SwapReady()
    {
        ready = !ready;
        return ready;
    }
}
