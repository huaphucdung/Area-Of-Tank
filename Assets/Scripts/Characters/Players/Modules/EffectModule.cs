using Photon.Pun;
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
    public PhotonView pv;
    private IData data;

    private void Start()
    {
        pv = GetComponent<PhotonView>();

        effectDictionary = new Dictionary<EffectType, BaseEffect>();
        foreach (BaseEffect effect in effects)
        {
            effectDictionary[effect.type] = effect;
        }
    }

    [PunRPC]
    public void DoEffect(EffectType type)
    {
        if (effectDictionary.ContainsKey(type))
        {
            /*effectDictionary[type].StartEffect(data);*/
        }
    }
}
