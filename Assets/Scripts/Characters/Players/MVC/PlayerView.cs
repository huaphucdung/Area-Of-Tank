using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerView : BaseView
{
    PhotonView pv;
    public override void Initialize(IData data = null)
    {
        base.Initialize(data);
        pv = GetComponent<PhotonView>();
    }

    public override void SpawnModel(IData data = null)
    {
        base.SpawnModel(data);
        if (data == null | !(data is PlayerViewData)) return;
        pv.RPC("ShowTankSkin", RpcTarget.All, ((PlayerViewData)data).keyModel);
    }

    [PunRPC]
    private void ShowTankSkin(string key)
    {
        GameObject modeltank = TankReferenceSO.InstanceTank(key);
        if (modeltank == null) return;
        modeltank.transform.parent = transform;
    }
}

public class PlayerViewData : IData
{
    public string keyModel;
}
