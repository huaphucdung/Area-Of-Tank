using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Photon.Pun;
using Random = UnityEngine.Random;

public class BoxItem : MonoBehaviour
{
    [SerializeField] private EffectType type;
    [SerializeField] private bool IsRandomEffect;

    public Action<BoxItem> disableAction;
    public int id;

    private void Start()
    {
        if (!IsRandomEffect) return;
        int index = Random.Range(0, Enum.GetValues(typeof(EffectType)).Length);
        type = (EffectType) Enum.GetValues(typeof(EffectType)).GetValue(index);
    }

    public void SetID(int id)
    {
        this.id = id;
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


    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonManager.IsHost()) return;
        TankModule tank = other.gameObject.GetComponent<TankModule>();
        tank?.pv.RPC("DoEffect", RpcTarget.All, type);
        disableAction?.Invoke(this);
    }
}
