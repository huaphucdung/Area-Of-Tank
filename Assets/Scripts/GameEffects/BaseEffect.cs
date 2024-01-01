using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class BaseEffect : MonoBehaviour, IEffect
{
    [SerializeField] public EffectType type;
    [SerializeField] private float duration = 3f;
    [SerializeField] protected ParticleSystem effect;
    public virtual void StartEffect(IData data)
    {
        /*Timing.RunCoroutine(DoEffect(data));*/
    }

    public virtual void EndEffect(IData data)
    {
    }

    IEnumerator<float> DoEffect(IData data)
    {
        yield return Timing.WaitForSeconds(duration);
        EndEffect(data);
    }

    public EffectType GetEffectType()
    {
        return type;
    }
}


public interface IEffect
{
    void StartEffect(IData data);
    void EndEffect(IData data);

    EffectType GetEffectType();
}