using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEffect : BaseEffect
{
    public override void StartEffect(PlayerModel data)
    {
        data.reusableData.IsShield = true;
        effect.Play();
        base.StartEffect(data);
    }

    public override void EndEffect(PlayerModel data)
    {
        base.EndEffect(data);
        data.reusableData.IsShield = false;
        effect.Stop();
    }
}
