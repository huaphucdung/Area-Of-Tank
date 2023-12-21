using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;
using System.Linq;

[CreateAssetMenu(fileName = "MapReferenceSO", menuName = "Data/MapReferenceSO")]
public class MapReferenceSO : ScriptableObject
{
    [SerializeField] private List<MapReference> maps;

    private static Dictionary<string, MapReference> _referenceDictionary;
    private static Dictionary<string, Map> _mapDictionary;

    public void Initialize()
    {
        _referenceDictionary = new Dictionary<string, MapReference>();
        _mapDictionary = new Dictionary<string, Map>();
        foreach (var map in maps)
        {
            _referenceDictionary[map.key] = map;

            var handleLoad =  map.mapAsset.LoadAssetAsync<GameObject>();
            handleLoad.Completed += (handle) =>
            {
                _mapDictionary[map.key] = handle.Result.GetComponent<Map>();
            };
        }
    }

    public static Map InstanceMap(string key)
    {
        if (_mapDictionary !=null && _mapDictionary.ContainsKey(key))
        {
            return GameObject.Instantiate(_mapDictionary[key]);
        }
        return null;
    }

    public static List<string> GetAllMapReferenceKey()
    {
        return _referenceDictionary.Keys.ToList();
    }

    public static MapReference GetMapReference(string key)
    {
        if(_referenceDictionary != null && _referenceDictionary.ContainsKey(key))
        {
            return _referenceDictionary[key];
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
