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

    public static GameObject InstanceTank(string key, Vector3 poistion, Quaternion rotation)
    {
        if (_tankDictionary != null || _tankDictionary.ContainsKey(key))
        {
            GameObject newTank = GameObject.Instantiate(_tankDictionary[key], poistion, rotation);
            return newTank;
        }
        return null;
    }

    public static GameObject InstanceTank(string key, Transform transform, Quaternion rotation)
    {
        if (_tankDictionary != null || _tankDictionary.ContainsKey(key))
        {
            GameObject newTank = GameObject.Instantiate(_tankDictionary[key], transform);
            newTank.transform.rotation = rotation;
            return newTank;
        }
        return null;
    }

    public static List<string> GetListTankType()
    {
        return new List<string>(_tankDictionary.Keys);
    }
}

[Serializable]
public class TankReference
{
    public string key;
    public AssetReference mapAsset;
}

