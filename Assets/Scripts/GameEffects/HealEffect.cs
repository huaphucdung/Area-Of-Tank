using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : BaseEffect
{
    [SerializeField] private int value;
    public override void StartEffect()
    {
        if (module.getDataFunc != null) module.getDataFunc.Invoke().AddHealth(value);
        effect.Play();
    }
}
