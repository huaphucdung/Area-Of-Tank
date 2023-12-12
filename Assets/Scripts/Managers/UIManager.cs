using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIReferenceSO uiReferenceSO;

    private static Dictionary<string, List<BaseUI>> _uiDictionary;
    private static UIReferenceSO uiStaticSO;
    private static Transform UIOverlayTransfrom;
   
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        uiStaticSO = uiReferenceSO;
        UIOverlayTransfrom = transform;
    }

    public static void Initialize()
    {
        _uiDictionary = new Dictionary<string, List<BaseUI>>();
        uiStaticSO.Intialize();
    }

    public static T GetAndShowUI<T>(UIData data = null, UIShowData showData = null, bool show = true) where T : BaseUI
    {
        string k = typeof(T).ToString();
        
        T UI = null;
       
        //Check has key UI in UI ReferenceSO
        if (!uiStaticSO.IsContainsUI(k)) return UI;
        UIReference reference = uiStaticSO.GetUIReference(k);

        //Check if has old UI then get it
        if (_uiDictionary.ContainsKey(k) && _uiDictionary[k] != null)
        {
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
            //Create new value in dicrection for key UI
            _uiDictionary[k] = new List<BaseUI>();
        }


        //IF NOT GET OLD UI CREATE THEN INSTANCE NEW UI
        if (UI == null)
        {
            //Check Is Prefab or AssetReference
            if (reference.isPrefabs)
            {
                //Load by Prefab
                UI = UILoadingByPrefab<T>(reference);
            }
            else
            {
                //Load by AssetReference
                UI = UILoadingByAssetReference<T>(reference);
            }
        }

        if (!UI.IsInit)
        {
            UI.Initialize(data);
            _uiDictionary[k].Add(UI);
        }
        if (show)
        {
            UI.Show(showData);
        }
        else
        {
            UI.Hide();
        }
        return UI;
    }

    private static T UILoadingByPrefab<T>(UIReference reference) where T : BaseUI
    {
        return Instantiate(reference.prefabs, UIOverlayTransfrom).GetComponent<T>();
    }

    private static T UILoadingByAssetReference<T>(UIReference reference) where T : BaseUI
    {
        var handle = reference.asset.InstantiateAsync();
        handle.WaitForCompletion();

        GameObject ui = handle.Result;
        ui.transform.SetParent(UIOverlayTransfrom);
        
        return ui.GetComponent<T>();
    }

    public static void HideUI<T>() where T : BaseUI
    {
        string k = typeof(T).ToString();
        if (_uiDictionary.ContainsKey(k) && _uiDictionary[k] != null)
        {
            //Check has key UI in UI ReferenceSO
            if (uiStaticSO.IsContainsUI(k)) return;

            UIReference reference = uiStaticSO.GetUIReference(k);

            if(reference.isSingle)
            {
                _uiDictionary[k][0].Hide();
            }
        }
    }

    public static void HideAllUI<T>() where T : BaseUI
    {
        string k = typeof(T).ToString();
        if (_uiDictionary.ContainsKey(k) && _uiDictionary[k] != null)
        {
            foreach (var ui in _uiDictionary[k])
            {
                ui.Hide();
            }
        }
    }
}
