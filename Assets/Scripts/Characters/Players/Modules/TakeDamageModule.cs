using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeDamageModule : MonoBehaviour, ITakeDamage
{
    private ICharacter character;
    public Func<int, bool, bool> TakeDameEvent;

    public void Initialize(IData data)
    {
        if (data == null || !(data is TakeDameData)) return;
        character = ((TakeDameData)data).character;
    }

    public bool Attack(int value, bool kill)
    {
        return TakeDameEvent.Invoke(value, kill);
    }
}


public class TakeDameData : IData
{
    public ICharacter character;
}
