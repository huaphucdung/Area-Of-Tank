using Photon.Realtime;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUI : BaseUI
{
    [SerializeField] VisualTreeAsset lobbyItem;

    private Button leaveLobbyBtn;
    private Button openDialougeCreateRoomBtn;
    private TextField searchField;
    private Button findRoomBtn;
    private Button quickJoinBtn;
    private Button refreshBtn;

    private ScrollView roomList;

    //Dialouge
    private VisualElement roomDialouge;
    private TextField roomName;
    private DropdownField modeMenu;
    private DropdownField mapMenu;
    private Button createRoomBtn;
    private Button closeDialougeBtn;

    public override void Initialize(UIData data = null)
    {
        base.Initialize(data);

        leaveLobbyBtn = root.Q<Button>("Leave");
        openDialougeCreateRoomBtn = root.Q<Button>("CreateRoom");
        searchField = root.Q<TextField>("SearchText");

        roomDialouge = root.Q<VisualElement>("Dialouge");

        roomName = root.Q<TextField>("RoomName");
        modeMenu = root.Q<DropdownField>("ModeMenu");
        mapMenu = root.Q<DropdownField>("MapMenu");

        createRoomBtn = root.Q<Button>("Create");
        closeDialougeBtn = root.Q<Button>("Close");

        roomList = root.Q<ScrollView>("RoomList");

        mapMenu.choices.Clear();
        foreach(string mapKey in MapReferenceSO.GetAllMapReferenceKey())
        {
            mapMenu.choices.Add(mapKey);
        }
        

        modeMenu.choices.Clear();
        foreach (string modeKey in ModeReferenceSO.GetAllModeReferenceKey())
        {
            modeMenu.choices.Add(modeKey);
        }
    }

    public override void Show(UIShowData data = null)
    {
        base.Show(data);
        mapMenu.index = 0;
        modeMenu.index = 0;

        HideCreateRoomDialouge();

        leaveLobbyBtn.clicked += LeaveLobby;

        openDialougeCreateRoomBtn.clicked += ShowCreateRoomDialouge;
        closeDialougeBtn.clicked += HideCreateRoomDialouge;

        createRoomBtn.clicked += CreateRoom;
    }

    public override void Hide()
    {
        base.Hide();

        leaveLobbyBtn.clicked -= LeaveLobby;

        openDialougeCreateRoomBtn.clicked -= ShowCreateRoomDialouge;
        closeDialougeBtn.clicked -= HideCreateRoomDialouge;

        createRoomBtn.clicked -= CreateRoom;
    }

    public void ShowListRoom(List<RoomInfo> listRoom)
    {
        roomList.contentContainer.Clear();
        foreach (RoomInfo room in listRoom)
        {
            lobbyItem.CloneTree(roomList.contentContainer);
            VisualElement item = roomList.contentContainer.Q<VisualElement>("LobbyItem");
            item.name = room.Name;
            item.Q<Label>("Name").text = $"Name: {room.Name}";
            item.Q<Label>("NumberPlayer").text = $"Player: {room.PlayerCount}/{room.MaxPlayers}";
            
            
            //Bug Cannot show mode and map value at lobby
            item.Q<Label>("Mode").text = $"Mode: {room.CustomProperties["mode"]}";
            foreach (DictionaryEntry entry in room.CustomProperties)
            {
                Debug.Log("Key: " + entry.Key + ", Value: " + entry.Value);
            }

            item.AddManipulator(new Clickable(evt => Lobby.JoinRoom(room.Name)));
        }

    }

    private void LeaveLobby()
    {
        Lobby.LeaveLobby();
    }

    private void CreateRoom()
    {
        Debug.Log(modeMenu.value);
        Lobby.CreateRoom(roomName.value, mapMenu.value, modeMenu.value);
    }

    private void ShowCreateRoomDialouge()
    {
        roomDialouge.style.display = DisplayStyle.Flex;
    }

    private void HideCreateRoomDialouge()
    {
        roomDialouge.style.display = DisplayStyle.None;
    }

    [Button]
    private void isInLobby()
    {
        Debug.Log(PhotonManager.IsInLobby());
        Debug.Log(PhotonManager.IsConnected());
    }
}
