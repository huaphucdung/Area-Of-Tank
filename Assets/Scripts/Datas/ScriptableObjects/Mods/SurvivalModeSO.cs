using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SurvivalModeSO", menuName = "Data/Mods/SurvivalMode")]
public class SurvivalModeSO : BaseModeSO
{
    public int maxDead;
    public override int GetScore(int kill, int dead)
    {
        return maxDead - dead;
    }

    public override bool CanRespawn(int kill, int dead)
    {
        return (dead < maxDead);
    }
}