using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ModeReferenceSO", menuName = "Data/ModeReferenceSO")]
public class ModeReferenceSO : ScriptableObject
{
    [SerializeField] private List<ModeReference> modes;

    private static Dictionary<string, ModeReference> _referenceDictionary;
    public void Initialize()
    {
        _referenceDictionary = new Dictionary<string, ModeReference>();

        foreach (var mode in modes)
        {
            _referenceDictionary[mode.key] = mode;
        }
    }

    public static List<string> GetAllModeReferenceKey()
    {
        return _referenceDictionary.Keys.ToList();
    }

    public static ModeReference GetMoeReference(string key)
    {
        if (_referenceDictionary != null && _referenceDictionary.ContainsKey(key))
        {
            return _referenceDictionary[key];
        }
        return null;
    }
}

[Serializable]
public class ModeReference
{
    public string key;
    public int value;
}