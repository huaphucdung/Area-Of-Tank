using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController, ICharacter
{
    protected TankModule tankModule;
    protected TakeDamageModule takeDamageModule;
    public PlayerView playerView => (PlayerView)view;
    public PlayerModel playerModel => (PlayerModel)model;

    #region Override Methods
    public override void Initialize(IData data = null)
    {
        base.Initialize(data);

        tankModule = playerView.GetComponentInChildren<TankModule>();
        tankModule.Intialize(playerModel.tankData, playerModel.reusableData);

        takeDamageModule = playerView.GetComponentInChildren<TakeDamageModule>();
        takeDamageModule.Initialize(new TakeDameData { character = this });

        Default();
    }

    public override void DoFixedUpdate()
    {
        if (playerModel.IsDead()) return;
        tankModule?.Move(InputManager.playerAction.Move.ReadValue<Vector2>());

        tankModule?.TurretRotate(InputManager.playerAction.MousePosition.ReadValue<Vector2>());
    }
    #endregion

    #region Main Methods
    public void SetTankPoistion(Vector3 position)
    {
        tankModule?.SetPosition(position);
    }
    public void Default()
    {
        GameManager.setCameraTargetEvent?.Invoke(tankModule?.transform);
        playerModel.SetDefault();
        tankModule?.TankDefault();
        if (takeDamageModule != null)
            takeDamageModule.TakeDameEvent += HitDamage;
        AddInput();
    }
    public void Dead()
    {
        GameManager.setCameraTargetEvent?.Invoke(null);
        playerModel.SetDead();
        tankModule?.TankDead();
        if (takeDamageModule != null)
            takeDamageModule.TakeDameEvent -= HitDamage;
        RemoveInput();

        GameManager.playerDeadAction?.Invoke(this);
    }

    public bool HitDamage(int value)
    {
        bool IsDead = playerModel.SetChangeHealth(-value);
        if (IsDead)
            Dead();
        return IsDead;
    }

    private void AddInput()
    {
        InputManager.playerAction.Shoot.started += Shoot;
    }

    private void RemoveInput()
    {
        InputManager.playerAction.Shoot.started -= Shoot;
    }
    #endregion

    #region Callback Methods
    private void Shoot(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        tankModule?.Shot(this);
    }
    #endregion
}
