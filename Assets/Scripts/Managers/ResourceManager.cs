using MEC;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Player Data:")]
    public PlayerDataSO defaultPlayerDataSO;
    public static PlayerDataSO currentPlayerDataSO;
    [Header("Json Datas:")]
    [FilePath(ParentFolder = "Assets/StreamingAssets", Extensions = "json")] public string tankJson;

    public static Dictionary<string, TankStruct> _tankDataDictionary;
   
    public static IEnumerator<float> initCoroutine;

    [Header("Rerference Datas:")]
    [SerializeField] private TankReferenceSO tankReferenceSO;
    [SerializeField] private MapReferenceSO mapReferenceSO;

    [Header("Rerference Play Mode:")]
    [SerializeField] private ModeReferenceSO modeReferenceSO;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        initCoroutine = Initialize();
    }

    public static TankStruct GetTankData(string tankType)
    {
        if(_tankDataDictionary.TryGetValue(tankType, out TankStruct value))
        {
            return value;
        }
        return _tankDataDictionary["Default"];
    }

    IEnumerator<float> Initialize()
    {
        currentPlayerDataSO = defaultPlayerDataSO;

        tankReferenceSO.Initialize();
        mapReferenceSO.Initialize();
        modeReferenceSO.Initialize();

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(LoadTankData(tankJson)));
        
    }

    IEnumerator<float> LoadTankData(string fileName)
    {
        Debug.Log(1);
        string filePath = Path.Combine(Application.streamingAssetsPath , fileName);
        Debug.Log(filePath);
        if (!File.Exists(filePath)) yield break;

        string jsonText = File.ReadAllText(filePath);
        
        _tankDataDictionary = JsonConvert.DeserializeObject<Dictionary<string, TankStruct>>(jsonText);
        yield return Timing.WaitForOneFrame;
        Debug.Log("Finish load tank datas");
    }

    public static void SavePlayerData()
    {
        PlayfabManager.SavePlayerData(currentPlayerDataSO.GetDataForSave());
    }

   
    private static void TestCurrentData()
    {
        Debug.Log($"{currentPlayerDataSO.exp}, {currentPlayerDataSO.coin}");
    }

    public static List<string> GetListTank()
    {
        return currentPlayerDataSO.tankTypeList;
    }
}
