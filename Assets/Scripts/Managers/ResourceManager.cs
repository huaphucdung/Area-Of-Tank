using MEC;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum TankType
{
    TankT34,
    TankISU152,
    TankKV2,
    TankT26
}

public class ResourceManager : MonoBehaviour
{
    [Header("Json Datas:")]
    [FilePath(ParentFolder = "Assets/Scripts/Datas/JsonFiles", Extensions = "json")] public string tankJson;
    
    public static Dictionary<string, TankStruct> TankDataDictionary;
    public static Dictionary<TankType, string> TankTypeToKeyDictionary;

    [Header("Rerference Datas:")]
    [SerializeField] private TankReferenceSO tankReferenceSO;
    [SerializeField] private MapReferenceSO mapReferenceSO;


    [Button]
    private void Initialize()
    {
        tankReferenceSO.Initialize();
        mapReferenceSO.Initialize();

        Timing.RunCoroutine(LoadTankData(tankJson));

        TankTypeToKeyDictionary = new Dictionary<TankType, string> {
            { TankType.TankT34, "TankT34"},
            { TankType.TankISU152, "TankISU152"},
            { TankType.TankKV2, "TankKV2"},
            { TankType.TankT26, "TankT26"},
        };
    }

    public static TankStruct GetTankDataByType(TankType type)
    {
        if (TankDataDictionary.TryGetValue(TankTypeToKeyDictionary[type], out TankStruct data))
        {
            return data;
        }
        return TankDataDictionary["default"];
    }


    IEnumerator<float> LoadTankData(string fileName)
    {
        string filePath = Path.Combine("Assets/Scripts/Datas/JsonFiles", fileName);
        if (!File.Exists(filePath)) yield break;

        string jsonText = File.ReadAllText(filePath);

        TankDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, TankStruct>>(jsonText);
        yield return Timing.WaitForOneFrame;
        Debug.Log("Finish load tank datas");
    }
}
