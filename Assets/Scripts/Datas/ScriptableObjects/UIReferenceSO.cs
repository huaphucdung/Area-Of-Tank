using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "UIReferenceSO", menuName = "Data/UIReferenceSO")]
public class UIReferenceSO : ScriptableObject
{
    [TableList] public List<UIReference> uiReferences;

    private Dictionary<string, UIReference> _uiReferenceDictionary;

    public void Intialize()
    {
        _uiReferenceDictionary = new Dictionary<string, UIReference>();
        
        foreach (UIReference reference in uiReferences)
        {
            _uiReferenceDictionary[reference.key] = reference;
        }
    }

    public bool IsContainsUI(string key)
    {
        return (_uiReferenceDictionary.ContainsKey(key) && _uiReferenceDictionary != null);
    }

    public UIReference GetUIReference(string key)
    {
        return _uiReferenceDictionary[key];
    }
}

[Serializable]
public class UIReference
{
    public string key;
    public bool isSingle;
    public bool isPrefabs;
    [HideIf("isPrefabs")]
    public AssetReference asset;
    [ShowIf("isPrefabs")]
    public BaseUI prefabs;
}