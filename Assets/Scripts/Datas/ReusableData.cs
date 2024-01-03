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

    public bool isShield;
    public bool isBuffDamage;
    public void Initialize()
    {
        currentEngine = 0;
        cooldown = Time.time;
        isShield = false;
        isBuffDamage = false;
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

    public bool AddHealth(int value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        return true;
    }

    public bool SubHealth(int value)
    {
        if (isShield) return false;
        currentHealth -= value;
        return true;
    }

    public bool IsDead()
    {
        if (state == PlayerState.Dead) return true;
        
        if(currentHealth <= 0)
        {
            SetDead();
            return true;
        }
        return false;
    }

    public float GetPercentHealth()
    {
        return (float)currentHealth / maxHealth;
    }
}
