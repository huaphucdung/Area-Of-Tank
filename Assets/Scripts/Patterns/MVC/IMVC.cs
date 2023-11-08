using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModel
{
    void Initialize(IData data = null);
    void ApplyDesgin(IData data = null);
}

public interface IView
{
    void Initialize(IData data = null);
    void SpawnModel(IData data = null);
}

public interface IController
{
    void Initialize(IData data = null);
    void DoUpdate();
    void DoFixedUpdate();
}

