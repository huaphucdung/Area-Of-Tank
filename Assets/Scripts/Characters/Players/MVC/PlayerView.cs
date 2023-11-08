using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PlayerView : BaseView
{
    public override void SpawnModel(IData data = null)
    {
        base.SpawnModel(data);
        if (data == null | !(data is PlayerViewData)) return;

        GameObject modeltank = TankReferenceSO.InstanceTank(((PlayerViewData)data).keyModel);

        if (modeltank == null) return;
        modeltank.transform.parent = transform;
    }
}

public class PlayerViewData : IData
{
    public string keyModel;
}
