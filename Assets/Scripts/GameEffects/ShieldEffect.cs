using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : BaseEffect
{
    public override void StartEffect()
    {
        if (module.getDataFunc !=  null) module.getDataFunc.Invoke().isShield = true;
        base.StartEffect();
    }

    public override void EndEffect()
    {
        if (module.getDataFunc != null) module.getDataFunc.Invoke().isShield = false;
        base.EndEffect();
    }
}
