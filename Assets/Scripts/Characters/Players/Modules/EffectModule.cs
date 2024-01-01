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

    private Dictionary<EffectType, BaseEffect> effectDictionary;
    private IData data;

    public void Initialize(IData data)
    {
        this.data = data;
        effectDictionary = new Dictionary<EffectType, BaseEffect>();
        foreach (BaseEffect effect in effects)
        {
            effectDictionary[effect.type] = effect;
        }
    }

    public void DoEffect(EffectType type)
    {
        if (effectDictionary.ContainsKey(type))
        {
            /*effectDictionary[type].StartEffect();*/
        }
    }
}
