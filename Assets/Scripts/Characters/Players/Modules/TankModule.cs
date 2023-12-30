using MEC;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankModule : MonoBehaviourPunCallbacks
{
    [Header("Sounds:")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip tankIdle;
    [SerializeField] private AudioClip tankDead;
    [SerializeField] private AudioClip tankShot;

    [Header("Transforms:")]
    [SerializeField] private Transform turret;
    [SerializeField] private Transform barrel;
    [SerializeField] private Transform gunEnd;

    [Header("Effects:")]
    [SerializeField] private ParticleSystem smokeBarrel;
    [SerializeField] private ParticleSystem smokeLowHealth;
    [SerializeField] private ParticleSystem smokeExplosion;

    [Header("AnimatorParamaters:")]
    [SerializeField] private string Moving = "Move";
    [SerializeField] private string Turning = "Turn";
    [SerializeField] private string ShotTrigger = "Shot";

    [SerializeField] private string Idling = "Idle";

    [SerializeField] private List<string> DeadTrigger;
    [SerializeField] private List<string> FreeTrigger;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private BoxCollider[] _boxColliders;

    public TankStruct data;
    public PhotonView pv;

    private bool isInit = false;
    public bool IsInit => isInit;

    public event Action tankDefaultTrigger;
    public event Action tankDeadTrigger; 

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _boxColliders = GetComponents<BoxCollider>();
        pv = GetComponent<PhotonView>();
    }


    public void Intialize(TankStruct _data)
    {
        data = _data;
        isInit = true;
    }

    [PunRPC]
    public void InitializePhoton()
    {
        data = ResourceManager.GetTankData(pv.Owner.CustomProperties["TankType"] as string);
        isInit = true;
    }
 
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetPosition(Vector3 position, Quaternion rotation)
    {
        SetPosition(position);
        transform.rotation = rotation;
    }

    #region Control Methods
    public void Move(Vector2 moveInput, ReusableData reusableData)
    {
        Vector3 currentVelicity = _rigidbody.velocity;
        currentVelicity.y = 0;
        reusableData.currentEngine += data.moveSpeedUpdate * ((moveInput.y != 0) ? 1 : -1);
        reusableData.currentEngine = Mathf.Clamp(reusableData.currentEngine, 0, data.maxSpeed);
        _rigidbody.AddForce(transform.forward * reusableData.currentEngine * moveInput.y - currentVelicity, ForceMode.VelocityChange);
        _rigidbody.MoveRotation(Quaternion.Euler(0, transform.eulerAngles.y + moveInput.x * data.rotationSpeed, 0));
        _animator.SetFloat(Moving, moveInput.y * reusableData.currentEngine);
        _animator.SetFloat(Turning, moveInput.x);
    }

    public void TurretRotate(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 direction = hit.point - turret.position;
            Vector3 newDir = Vector3.RotateTowards(turret.forward, direction, 1, 0.0F);
            var target = Quaternion.LookRotation(newDir);
            turret.rotation = Quaternion.Euler(-90, target.eulerAngles.y, 0);
            Debug.DrawLine(turret.position, turret.position + direction, Color.red);
        }
    }

    public void Shot(ReusableData reusableData)
    {
        if (reusableData.cooldown > Time.time) return;
        Debug.Log("Tank shot");
        pv.RPC("CreateShell", RpcTarget.All);
        AudioManager.PlayOneShotAudio(audioSource, tankShot);
        _animator.SetTrigger(ShotTrigger);
        smokeBarrel.Play();
        reusableData.cooldown = Time.time + data.cooldown;
    }

    [PunRPC]
    private void CreateShell()
    {
        Shell tankShell = SpawnManager.GetShellEvent?.Invoke(gunEnd.position, Quaternion.Euler(turret.eulerAngles.x, turret.eulerAngles.y + 180, turret.eulerAngles.z)); /*new Quaternion(-turret.rotation.x, turret.rotation.y, turret.rotation.z, turret.rotation.w));*/
        tankShell.SetData(new ShellData
        {
            player = pv.Owner,
            damage = data.damage,
        });
        tankShell.Rb.AddForce(-turret.up.normalized * data.range, ForceMode.Force);
    }
    #endregion

    #region SetStateForTank
    [PunRPC]
    public void TankDefault()
    {
        foreach (var box in _boxColliders)
        {
            box.enabled = true;
        }
        AudioManager.PlayAudio(audioSource, tankIdle);
        _animator.CrossFade(Idling, 0f);

        tankDefaultTrigger?.Invoke();
    }

    [PunRPC]
    public void TankDead()
    {
        _rigidbody.velocity = Vector3.zero;
        foreach (var box in _boxColliders)
        {
            box.enabled = false;
        }
        AudioManager.StopPlayAudio(audioSource);
        AudioManager.PlayOneShotAudio(audioSource, tankDead);
        smokeExplosion.Play();
        _animator.SetTrigger(DeadTrigger[UnityEngine.Random.Range(0, DeadTrigger.Count)]);

        tankDeadTrigger?.Invoke();
    }

    public void TankFree()
    {
        _animator.SetTrigger(FreeTrigger[UnityEngine.Random.Range(0, FreeTrigger.Count)]);
    }
    #endregion

    public void PlayEffectLowHealth(bool value)
    {
        if (value && !smokeLowHealth.isPlaying)
        {
            smokeLowHealth.Play();
            return;
        }
        smokeLowHealth.Stop();
    }

    public void DefaultTuretRotation()
    {
        turret.rotation = Quaternion.Euler(-90, transform.eulerAngles.y ,0);
    }

    public void StopAudio()
    {
        AudioManager.StopPlayAudio(audioSource);
    }
}
 
