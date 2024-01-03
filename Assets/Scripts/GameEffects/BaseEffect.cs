using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class BaseEffect : MonoBehaviour, IEffect
{
    [SerializeField] public EffectType type;
    [SerializeField] private float duration = 3f;
    [SerializeField] protected ParticleSystem effect;

    protected EffectModule module;

    public void Initialize(EffectModule Module)
    {
        module = Module;
    }

    public virtual void StartEffect()
    {
        effect.Play();
        Timing.RunCoroutine(CooldownEffect());
    }

    public virtual void EndEffect()
    {
        effect.Stop();
    }

    IEnumerator<float> CooldownEffect()
    {
        yield return Timing.WaitForSeconds(duration);
        EndEffect();
    }

    public EffectType GetEffectType()
    {
        return type;
    }
}


public interface IEffect
{
    void StartEffect();
    void EndEffect();
    EffectType GetEffectType();
}