using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class BaseEffect : MonoBehaviour, IEffect
{
    [SerializeField] private EffectType type;
    [SerializeField] private float duration = 3f;
    [SerializeField] protected ParticleSystem effect;
    public virtual void StartEffect(PlayerModel data)
    {
        Timing.RunCoroutine(DoEffect(data));
    }

    public virtual void EndEffect(PlayerModel data)
    {
    }

    IEnumerator<float> DoEffect(PlayerModel data)
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
    void StartEffect(PlayerModel data);
    void EndEffect(PlayerModel data);

    EffectType GetEffectType();
}