using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerState
{
    Life,
    Dead,
}

[Serializable]
public class ReusableData
{
    public float currentEngine;
    public float cooldown;
    public int maxHealth;
    public int currentHealth;
    public PlayerState state;
    public void Initialize()
    {
        currentEngine = 0;
        cooldown = Time.time;
    }

    public void SetDefault(TankStruct data)
    {
        state = PlayerState.Life;

        maxHealth = currentHealth = data.health;
    }

    public void SetDead()
    {
        state = PlayerState.Dead;
        currentHealth = 0;
    }

    public bool ChangeHealth(int value)
    {
        currentHealth -= value;
        if (currentHealth <=0)
        {
            SetDead();
            return true;
        }
        return false;
    }
}
