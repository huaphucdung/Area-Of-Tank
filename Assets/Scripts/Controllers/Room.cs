using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Room Object:")]
    [SerializeField] private List<RoomItem> roomItems;

    private void Start()
    {
       /* Initialize();*/
    }

    private void Initialize()
    {
        foreach(var item in roomItems)
        {
            item.Initialize();
            item.gameObject.SetActive(false);
        }
    }
}
