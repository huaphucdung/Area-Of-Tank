using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    Heal,
    Shield,
    BuffDame
}

public class EffectModule : MonoBehaviour, ITakeEffect
{
    [SerializeField] private List<BaseEffect> effects;

    private PlayerModel data;

    public void Initialize(PlayerModel data)
    {
        this.data = data;
    }

    public void DoEffect(EffectType type)
    {
        foreach(BaseEffect effect in effects)
        {
            if(effect.GetEffectType() == type)
            {
                effect.StartEffect(data);
                break;
            }
        }
    }
}
