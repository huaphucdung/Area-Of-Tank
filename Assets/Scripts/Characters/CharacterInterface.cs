using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    bool HitDamage(int value);
}

public interface ITakeDamage
{
    bool Attack(int value);
}

