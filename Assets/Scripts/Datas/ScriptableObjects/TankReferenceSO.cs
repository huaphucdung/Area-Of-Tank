using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "TankReferenceSO", menuName = "Data/TankReferenceSO")]
public class TankReferenceSO : ScriptableObject
{
    [SerializeField] private List<TankReference> tanks;

    private static Dictionary<string, GameObject> _tankDictionary;

    public void Initialize()
    {
        _tankDictionary = new Dictionary<string, GameObject>();
        foreach (var tank in tanks)
        {
            var handleLoad = tank.mapAsset.LoadAssetAsync<GameObject>();
            handleLoad.Completed += (handle) =>
            {
                _tankDictionary[tank.key] = handle.Result;
            };
        }
    }

    public static GameObject InstanceTank(string key)
    {
        if (_tankDictionary != null || _tankDictionary.ContainsKey(key))
        {
            return GameObject.Instantiate(_tankDictionary[key]);
        }
        return null;
    }
}

[Serializable]
public class TankReference
{
    public string key;
    public AssetReference mapAsset;
}

