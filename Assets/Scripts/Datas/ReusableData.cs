using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ReusableData
{
    public float currentEngine;
    public float cooldown;
    public void Initialize()
    {
        currentEngine = 0;
        cooldown = Time.time;
    }
}
