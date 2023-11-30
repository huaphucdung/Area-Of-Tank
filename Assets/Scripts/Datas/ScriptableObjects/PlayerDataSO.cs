using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultPlayerData", menuName = "Data/PlayerData/Default")]
public class PlayerDataSO : ScriptableObject
{
    public int exp = 0;
    public int coin = 0;
    public List<string> tankTypeList;

    public Dictionary<string, string> GetDataForSave()
    {
        return new Dictionary<string, string>()
        {
            {"Exp", exp.ToString()},
            {"Coin", coin.ToString()},
            {"TankTypeList", string.Join(",", tankTypeList)}
        };
    }

    public void ConvertData(Dictionary<string, UserDataRecord> data)
    {
        exp = int.Parse(data["Exp"].Value);
        coin = int.Parse(data["Coin"].Value);
        tankTypeList = new List<string>(data["TankTypeList"].Value.Split(","));
    }
}
