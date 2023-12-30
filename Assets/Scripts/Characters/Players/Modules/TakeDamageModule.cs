using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageModule : MonoBehaviour
{
    public Func<int, bool> TakeDameEvent;

    public PhotonView view;

    private void Start()
    {
        view = GetComponent<PhotonView>();
    }
    [PunRPC]
    public void Attack(Player player ,int value)
    {
        if (TakeDameEvent == null) return;
        if(TakeDameEvent.Invoke(value))
        {
            //Set dead for tank
            view.RPC("TankDead", RpcTarget.All);
            //Send who kill who dead
            GameManager.SendScore(player, view.Owner);
        }
    }
}

