
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomUI : BaseUI
{
    [SerializeField] float left;
    [SerializeField] float top;
    private Button leaveRoomBtn;

    private VisualElement topScreen;
    private Button readyAndUnreadyBtn;
    private Button choiceLeftTankBtn;
    private Button choiceRightTankBtn;
    private Button startBtn;

    private Button showSettingDialougeBtn;

    private Label roomName;
    private Label mode;
    private Label numberPlayer;
    private VisualElement map;
    private Label mapName;

    private VisualElement playerInfor0;
    private VisualElement playerInfor1;
    private VisualElement playerInfor2;
    private VisualElement playerInfor3;


    private Photon.Realtime.Room roomInfo;


    private Dictionary<Player, VisualElement> playerDictionary;


    public override void Initialize(UIData data = null)
    {
        base.Initialize(data);

        playerDictionary = new Dictionary<Player, VisualElement>();

        leaveRoomBtn = root.Q<Button>("LeaveBtn");
        topScreen = root.Q<VisualElement>("TopScreen");

        readyAndUnreadyBtn = root.Q<Button>("ReadyBtn");
        choiceLeftTankBtn = root.Q<Button>("LeftBtn");
        choiceRightTankBtn = root.Q<Button>("RightBtn");

        startBtn = root.Q<Button>("StartBtn");

        showSettingDialougeBtn = root.Q<Button>("SettingBtn");

        roomName = root.Q<Label>("RoomName");
        mode = root.Q<Label>("Mode");
        numberPlayer = root.Q<Label>("NumberPlayer");
        mapName = root.Q<Label>("MapName");

        map = root.Q<VisualElement>("Map");

        playerInfor0 = root.Q<VisualElement>("PlayerInfo0");
        playerInfor1 = root.Q<VisualElement>("PlayerInfo1");
        playerInfor2 = root.Q<VisualElement>("PlayerInfo2");
        playerInfor3 = root.Q<VisualElement>("PlayerInfo3");
    }

    public override void Show(UIShowData data = null)
    {
        base.Show(data);

        SetRoomInfo(((UIShowRoomData)data).currentRoom);
        ChangeReady(false);

        leaveRoomBtn.clicked += LeaveRoom;
        readyAndUnreadyBtn.clicked += ChangeReady;

        choiceLeftTankBtn.clicked += ChangeTankLeft;
        choiceRightTankBtn.clicked += ChangeTankRight;

        ShowUIForHost(PhotonManager.IsHost());
    }

    public override void Hide()
    {
        base.Hide();
        leaveRoomBtn.clicked -= LeaveRoom;
        readyAndUnreadyBtn.clicked -= ChangeReady;

        choiceLeftTankBtn.clicked -= ChangeTankLeft;
        choiceRightTankBtn.clicked -= ChangeTankRight;
    }

    private void ShowUIForHost(bool value)
    {
        if (value)
        {
            startBtn.SetEnabled(false);
            startBtn.style.visibility = Visibility.Visible;
            showSettingDialougeBtn.style.visibility = Visibility.Visible;
        }
        else
        {
            startBtn.style.visibility = Visibility.Hidden;
            showSettingDialougeBtn.style.visibility = Visibility.Hidden;
        }
    }

    private void ChangeReady()
    {
        ChangeReady(Room.readyAction.Invoke());
    }

    private void LeaveRoom()
    {
        Room.LeaveRoom();
    }

    private void ChangeReady(bool ready)
    {
        if (ready)
        {
            readyAndUnreadyBtn.text = "Unready";
            choiceLeftTankBtn.style.visibility = Visibility.Hidden;
            choiceRightTankBtn.style.visibility = Visibility.Hidden;
        }
        else
        {
            readyAndUnreadyBtn.text = "Ready";
            choiceLeftTankBtn.style.visibility = Visibility.Visible;
            choiceRightTankBtn.style.visibility = Visibility.Visible;
        }
    }

    private void ChangeTankLeft()
    {
        Room.ChangeTank(-1);
    }

    private void ChangeTankRight()
    {
        Room.ChangeTank(1);
    }

    public void PlayerChangeValue(Player player, ExitGames.Client.Photon.Hashtable properties)
    {
        if (playerDictionary.ContainsKey(player))
        {
            VisualElement item = playerDictionary[player];
            item.Q<Label>("Ready").style.visibility = ((bool)properties["Ready"]) ? Visibility.Visible : Visibility.Hidden;
        }
    }

    private void CheckIsHost(bool value)
    {
        if (value)
        {
            startBtn.style.visibility = Visibility.Visible;
        }
        else
        {
            startBtn.style.visibility = Visibility.Hidden;
        }
    }

    private void CheckAllPlayerReady(bool value)
    {
        startBtn.SetEnabled(value);
    }

    private void SetRoomInfo(Photon.Realtime.Room room)
    {
        roomInfo = room;

        roomName.text = $"Room: {room.Name}";
        mode.text = $"Mode: {room.CustomProperties["mode"]}";
        mapName.text = room.CustomProperties["map"].ToString();
        numberPlayer.text = $"Player: {room.PlayerCount}/{room.MaxPlayers}";
    }

    public void AddPlayerValue(Player player, int index)
    {
        VisualElement item = null;

        switch (index)
        {
            case 1:
                item = playerInfor1;
                break;
            case 2:
                item = playerInfor2;
                break;
            case 3:
                item = playerInfor3;
                break;
            default:
                item = playerInfor0;
                break;
        }

        item.Q<Label>("PlayerName").text = player.UserId;
        item.Q<VisualElement>("HostIcon").style.visibility = (PhotonManager.IsHost()) ? Visibility.Visible : Visibility.Hidden;
        item.Q<Label>("Ready").style.visibility = ((bool)player.CustomProperties["Ready"]) ? Visibility.Visible : Visibility.Hidden;
        item.style.visibility = Visibility.Visible;
        playerDictionary[player] = item;
    }

    public void HideAllPlayerUI()
    {
        playerDictionary.Clear();
        playerInfor0.style.visibility = playerInfor0.Q<Label>("Ready").style.visibility = Visibility.Hidden;
        playerInfor1.style.visibility = playerInfor1.Q<Label>("Ready").style.visibility = Visibility.Hidden;
        playerInfor2.style.visibility = playerInfor2.Q<Label>("Ready").style.visibility = Visibility.Hidden;
        playerInfor3.style.visibility = playerInfor3.Q<Label>("Ready").style.visibility = Visibility.Hidden;
    }

    public void EnableStartBtn(bool value)
    {
        startBtn.SetEnabled(value);
        if (value)
        {
            startBtn.clicked += StartGame;
        }
        else
        {
            startBtn.clicked -= StartGame;

        }
    }

    private void StartGame()
    {
        PhotonManager.StartGameplay();
    }
}


public class UIShowRoomData : UIShowData
{
    public Photon.Realtime.Room currentRoom;
}