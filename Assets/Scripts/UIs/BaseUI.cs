using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    protected UIData data;
    protected bool init;
    protected bool active;
    public bool IsActive => active;
    public bool IsInit => init;
    public virtual void Initialize(UIData data = null)
    {
        init = true;
    }

    public virtual void Show(UIData data = null)
    {
        active = true;
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        active = false;
        gameObject.SetActive(false);
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}


public class UIData : IData
{

}