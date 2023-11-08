using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "UIReferenceSO", menuName = "Data/UIReferenceSO")]
public class UIReferenceSO : ScriptableObject
{
    [SerializeField] private List<UIReference> uiReferences;

    private static Dictionary<string, UIReference> _uiReferenceDictionary;
    private static Dictionary<string, List<BaseUI>> _uiDictionary;

    public void Intialize()
    {
        _uiReferenceDictionary = new Dictionary<string, UIReference>();
        _uiDictionary = new Dictionary<string, List<BaseUI>>();

        foreach (UIReference reference in uiReferences)
        {
            _uiReferenceDictionary[reference.key] = reference;
        }
    }

    //Need to move it to UIManager
    public static T GetUI<T>(UIData data = null, bool show = true) where T : BaseUI
    {
        string k = typeof(T).ToString();
        T UI = null;
        if (!_uiReferenceDictionary.ContainsKey(k) || _uiReferenceDictionary == null) return UI;

        UIReference reference = _uiReferenceDictionary[k];

        if (_uiDictionary.ContainsKey(k) && _uiDictionary[k] != null)
        {
            //Get Old UI
            if (reference.isSingle)
            {
                UI = (T)_uiDictionary[k][0];
            }
            else
            {
                foreach (T ui in _uiDictionary[k])
                {
                    if (!ui.IsActive)
                    {
                        UI = ui;
                        break;
                    }
                }
            }
        }
        else
        {
            //Create new value in dictionary UI and UI
            _uiDictionary[k] = new List<BaseUI>();
            var handle = reference.uiAsset.InstantiateAsync();
            handle.WaitForCompletion();
            UI = handle.Result.GetComponent<T>();
        }

        if (UI == null)
        {
            //Instance New UI
            var handle = reference.uiAsset.InstantiateAsync();
            handle.WaitForCompletion();
            UI = handle.Result.GetComponent<T>();
        }

        UI.Initialize(data);
        if (show)
        {
            UI.Show(data);
        }
        else
        {
            UI.Hide();
        }
        return UI;
    }

    public static void HideAllUI<T>() where T : BaseUI
    {
        string k = typeof(T).ToString();
        if (_uiDictionary.ContainsKey(k) && _uiDictionary[k] != null)
        {
            foreach(T ui in _uiDictionary[k])
            {
                ui.Hide();
            }
        }
    }
}

[Serializable]
public class UIReference
{
    public string key;
    public bool isSingle;
    public AssetReference uiAsset;
}