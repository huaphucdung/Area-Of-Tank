using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class BaseEffect : MonoBehaviour, IEffect
{
    [SerializeField] private float duration = 3f;
    [SerializeField] private ParticleSystem effect;
    public virtual void StartEffect()
    {
        Timing.RunCoroutine(DoEffect());
    }

    public virtual void EndEffect()
    {
    }

    IEnumerator<float> DoEffect()
    {
        yield return Timing.WaitForSeconds(duration);
        EndEffect();
    }
}


public interface IEffect
{
    void StartEffect();
    void EndEffect();
}