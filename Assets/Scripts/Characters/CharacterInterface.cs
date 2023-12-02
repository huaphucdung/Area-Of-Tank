using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    bool HitDamage(int value, bool kill = false);
}

public interface ITakeDamage
{
    bool Attack(int value, bool kill = false);
}

public interface ITakeEffect
{
    void DoEffect(EffectType type);
}
