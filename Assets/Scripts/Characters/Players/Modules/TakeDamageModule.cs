using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TakeDamageModule : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshBody;
    [SerializeField] private MeshRenderer meshTurret;
    public PhotonView view;

    public Action<Player, Player, int> TakeDameEvent;
    
    private void Start()
    {
        view = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void Attack(Player player ,int value)
    {
        TakeDameEvent?.Invoke(player, view.Owner, value);
    }

    [PunRPC]
    private void ShowTakeDamageEffect()
    {
        meshBody.material.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
        meshTurret.material.DOColor(Color.red, 0.1f).SetLoops(2, LoopType.Yoyo);
    }
}

