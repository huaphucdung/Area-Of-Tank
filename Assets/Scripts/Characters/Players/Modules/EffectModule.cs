using Photon.Pun;
using System;
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
    public PhotonView pv;

    private Dictionary<EffectType, BaseEffect> effectDictionary;
    public Func<ReusableData> getDataFunc;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        effectDictionary = new Dictionary<EffectType, BaseEffect>();
        foreach (BaseEffect effect in effects)
        {
            effect.Initialize(this);
            effectDictionary[effect.type] = effect;
        }
    }

    [PunRPC]
    public void DoEffect(EffectType type)
    {
        Debug.Log(type.ToString());
        if (effectDictionary.ContainsKey(type))
        {
            effectDictionary[type].StartEffect();
        }
    }
}
