using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "ScoreModeSO", menuName = "Data/Mods/ScoreMode")]
public class ScoreModeSO : BaseModeSO
{
    public override int GetScore(int kill, int dead)
    {
        return kill;
    }
}
