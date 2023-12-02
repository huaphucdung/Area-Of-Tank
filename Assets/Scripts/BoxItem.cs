using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class BoxItem : MonoBehaviour
{
    [SerializeField] private EffectType type;

    [SerializeField] private bool IsRandomEffect;
    private void Start()
    {
        if (!IsRandomEffect) return;
        int index = Random.Range(0, Enum.GetValues(typeof(EffectType)).Length);
        type = (EffectType) Enum.GetValues(typeof(EffectType)).GetValue(index);
    }

    private void OnEnable()
    {
        transform.DORotate(new Vector3(0f, 360f, 0f), 1f, RotateMode.WorldAxisAdd)
            .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void OnCollisionEnter(Collision collision)
    {
        ITakeEffect takeEffect = collision.gameObject.GetComponent<ITakeEffect>();
        if (takeEffect == null) return;
        takeEffect.DoEffect(type);
    }
}
