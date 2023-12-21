using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseUI : MonoBehaviourPunCallbacks
{
    protected UIData data;
    protected UIShowData showData;
    protected bool init;
    protected bool active;

    protected VisualElement root;

    public bool IsActive => active;
    public bool IsInit => init;

    public virtual void Initialize(UIData data = null)
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        init = true;
    }

    public virtual void Show(UIShowData data = null)
    {
        active = true;
        root.style.display = DisplayStyle.Flex;
    }


    public virtual void Hide()
    {
        active = false;
        root.style.display = DisplayStyle.None;
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }
}


public class UIData : IData
{

}

public class UIShowData: IData
{

}
