using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseView : MonoBehaviour, IView
{
    public virtual void Initialize(IData data = null)
    {
    }

    public virtual void SpawnModel(IData data = null)
    {
    }
}