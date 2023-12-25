using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffect : BaseEffect
{
    [SerializeField] private int value;
    public override void StartEffect(IData data)
    {
        /*data.SetTakeHeal(value);*/
        effect.Play();
    }

    public override void EndEffect(IData data)
    {
    }
}
