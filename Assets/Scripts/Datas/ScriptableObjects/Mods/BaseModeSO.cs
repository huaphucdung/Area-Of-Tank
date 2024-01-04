using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseModeSO : ScriptableObject
{
    public string key;
    public float maxTime = 300f;

    public virtual int GetScore(int kill, int dead)
    {
        return 0;
    }

    public virtual bool CanRespawn(int kill, int dead)
    {
        return true;
    }
}
