using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDameEffect : BaseEffect
{
    public override void StartEffect(PlayerModel data)
    {
        data.reusableData.IsBuffDame = true;
        effect.Play();
        base.StartEffect(data);
    }

    public override void EndEffect(PlayerModel data)
    {
        data.reusableData.IsBuffDame = false;
        effect.Stop();
    }
}
