using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "MapReferenceSO", menuName = "Data/MapReferenceSO")]
public class MapReferenceSO : ScriptableObject
{
    [SerializeField] private List<MapReference> maps;

    private static Dictionary<string, Map> _mapDictionary;

    public void Initialize()
    {
        _mapDictionary = new Dictionary<string, Map>();
        foreach (var map in maps)
        {
            var handleLoad =  map.mapAsset.LoadAssetAsync<GameObject>();
            handleLoad.Completed += (handle) =>
            {
                _mapDictionary[map.key] = handle.Result.GetComponent<Map>();
            };
        }
    }


    public static Map InstanceMap(string key)
    {
        if (_mapDictionary !=null || _mapDictionary.ContainsKey(key))
        {
            return GameObject.Instantiate(_mapDictionary[key]);
        }
        return null;
    }
}

[Serializable]
public class MapReference
{
    public string key;
    public AssetReference mapAsset;
}
