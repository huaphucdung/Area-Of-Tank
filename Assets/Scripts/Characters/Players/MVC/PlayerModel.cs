using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    Live,
    Dead
}

public class PlayerModel : BaseModel
{
    public TankStruct tankData;
    public PlayerReusableData reusableData;

    public override void Initialize(IData data = null)
    {
        reusableData = new PlayerReusableData() {
            currentHealth = 100,
            currentEngine = 0,
            cooldown = Time.time,
            state = CharacterState.Live,
            IsShield = false,
            IsBuffDame = false
        };
    }

    public override void ApplyDesgin(IData data = null)
    {
        if (data == null | !(data is PlayerModelData)) return;
        tankData = ((PlayerModelData)data).tankData;
    }

    public void SetDefault()
    {
        reusableData.currentHealth = tankData.health;
        ChangeState(CharacterState.Live);
    }

    public void SetDead()
    {
        reusableData.currentHealth = 0;
        ChangeState(CharacterState.Dead);
    }

    public void ChangeState(CharacterState state)
    {
        reusableData.state = state;
    }

    public bool SetTakeDame(int value, bool kill, int defenceBase = 10)
    {
        if (kill)
        {
            reusableData.currentHealth = 0;
            return true;
        }

        if (reusableData.IsShield) return false;

        reusableData.currentHealth += value + ((int)tankData.defense / defenceBase);
        return (reusableData.currentHealth <= 0) ? true : false;
    }

    public void SetTakeHeal(int value)
    {
        reusableData.currentHealth = Mathf.Clamp(reusableData.currentHealth + value, 0, tankData.health);
    }

    public bool IsDead()
    {
        return reusableData.state == CharacterState.Dead;
    }
}

public struct PlayerReusableData
{
    public int currentHealth;
    public float currentEngine;
    public float cooldown;
    public CharacterState state;
    public bool IsShield;
    public bool IsBuffDame;
}

public class PlayerModelData : IData
{
    public TankStruct tankData;
}