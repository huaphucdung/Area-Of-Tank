using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : IController
{
    protected IModel model;
    protected IView view;

    public virtual void Initialize(IData data = null)
    {
        ControllerData cData = null;
        if (data is ControllerData)
        {
            cData = (ControllerData) data;
        }
        model = cData?.model;
        view = cData?.view;
    }

    public virtual void DoUpdate()
    {
    }

    public virtual void DoFixedUpdate()
    {
    }
    
}

public class ControllerData : IData
{
    public IModel model;
    public IView view;
}
