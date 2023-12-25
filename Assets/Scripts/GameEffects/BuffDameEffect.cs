using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDameEffect : BaseEffect
{
    public override void StartEffect(IData data)
    {
        /*data.reusableData.IsBuffDame = true;*/
        effect.Play();
        base.StartEffect(data);
    }

    public override void EndEffect(IData data)
    {
        /*data.reusableData.IsBuffDame = false;*/
        effect.Stop();
    }
}
