using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel : IModel
{
    public virtual void Initialize(IData data = null)
    {
    }

    public virtual void ApplyDesgin(IData data = null)
    {
    }
}
