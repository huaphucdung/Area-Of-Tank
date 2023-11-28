using Photon.Realtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Object:")]
    [SerializeField] private List<RoomItem> roomItems;

    private Dictionary<int, Player> playersInRoom;
    private int numberPlayerInRoom;

    private void Awake()
    {
        Lobby.joinRoomSuccessAction += ShowPlayerInRoom;
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
}
