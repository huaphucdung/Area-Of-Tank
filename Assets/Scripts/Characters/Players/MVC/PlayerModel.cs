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
            state = CharacterState.Live
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

    public bool SetChangeHealth(int value, int defenceBase = 10)
    {
        if(value >= 0)
        {
            reusableData.currentEngine = Mathf.Clamp(reusableData.currentEngine + value, 0, tankData.health);
            return false;
        }

        reusableData.currentEngine += value + ((int)tankData.defense / defenceBase);
        return (reusableData.currentEngine <= 0) ? true : false;
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
}

public class PlayerModelData : IData
{
    public TankStruct tankData;
}