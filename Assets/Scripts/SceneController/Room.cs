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

    private bool ready;
    private RoomUI ui;

    private int playerIndex = 1;
    private static Dictionary<Player, RoomItem> playerItems;

    private static List<string> listTankCanChoice;
    private static int tankIndex = 0;

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

    private void ShowPlayerInRoom()
    {
        ui.HideAllPlayerUI();
        playerItems.Clear();
        foreach (var item in roomItems)
        {
            item.SetEmpty();
        }
       
        playerIndex = 1;
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
                AddPlayer(player, playerIndex);
                playerIndex++;
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

    private void ChangeReady(bool value)
    {
        PhotonManager.LocalPlayerChangeReady(value);
    }

    public static void ChangeTank(int value)
    {
        tankIndex += value;
        if (tankIndex < 0) tankIndex = listTankCanChoice.Count - 1;
        if (tankIndex >= listTankCanChoice.Count) tankIndex = 0;

        if (playerItems.ContainsKey(PhotonManager.GetLocalPlayer()))
        {
            playerItems[PhotonManager.GetLocalPlayer()].ChangeTank(listTankCanChoice[tankIndex]);

            PhotonManager.LocalPlayerChangeTank(listTankCanChoice[tankIndex]);
        }
    }


    private void SettingRoom(string map, string mode)
    {
        PhotonManager.UpdateRoomProperties(map, mode);
    }

    #region Callback Methods
    private void OnRoomSettingUpdate(Hashtable obj)
    {
    }

    private void OnPlayerInRoomUpdate(Player player, Hashtable properties)
    {
        ui.PlayerChangeValue(player, properties);
        if (playerItems.ContainsKey(player))
        {
            playerItems[player].ChangeTank(properties["TankType"] as string);
        }

        //Check Is All Player Ready
        ChechReadyForPlay();
    }

    private void ChechReadyForPlay()
    {
        if (PhotonManager.IsHost())
        {
            if (PhotonManager.GetNumberPlayerInRoom() < 2)
            {
                ui.EnableStartBtn(false);
                return;
            }

            foreach (KeyValuePair<int, Player> dictionary in PhotonManager.GetPlayerInRoom())
            {
                Player one = dictionary.Value;
                if (!(bool)one.CustomProperties["Ready"])
                {
                    ui.EnableStartBtn(false);
                    return;
                }
            }

            ui.EnableStartBtn(true);
        }
    }

    private void OnJoinRoom()
    {
        ShowUI();
        ShowPlayerInRoom();
        listTankCanChoice = ResourceManager.GetListTank();
        tankIndex = 0;
    }

    private void OnLeftRoom()
    {
        HideUI();
    }

    private void OnNewPlayerEnter(Player player)
    {
        AddPlayer(player, playerIndex);
        playerIndex++;
    }

    private void OnPlayerLeft(Player player)
    {
        ShowPlayerInRoom();
    }

    #endregion

    private bool SwapReady()
    {
        ready = !ready;
        ChangeReady(ready);
        return ready;
    }
}
