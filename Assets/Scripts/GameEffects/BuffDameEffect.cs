using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDameEffect : BaseEffect
{
    public override void StartEffect()
    {
        if (module.getDataFunc != null) module.getDataFunc.Invoke().isBuffDamage = true;
        base.StartEffect();
    }

    public override void EndEffect()
    {
        if (module.getDataFunc != null) module.getDataFunc.Invoke().isBuffDamage = false;
        base.EndEffect();
    }
}
