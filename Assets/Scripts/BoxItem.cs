using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoxItem : MonoBehaviour
{
    private void OnEnable()
    {
        transform.DORotate(new Vector3(0f, 360f, 0f), 1f, RotateMode.WorldAxisAdd)
            .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }  
}
