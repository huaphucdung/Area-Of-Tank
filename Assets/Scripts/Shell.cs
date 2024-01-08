using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviourPunCallbacks
{
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private int damage;
    [SerializeField] private ParticleSystem boomEffect;
    [SerializeField] private AudioClip sound;

    private AudioSource _source;
    private MeshRenderer _render;
    private SphereCollider _sphereCollider;
    private Rigidbody _rigidbody;
    private PhotonView _view;
    public Rigidbody Rb => _rigidbody;
    public Player player;
    #region Unity
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _source = GetComponent<AudioSource>();
        _render = GetComponent<MeshRenderer>();
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        CancelInvoke();
        _render.enabled = false;
        _sphereCollider.enabled = false;
        boomEffect.Play();
        AudioManager.PlayOneShotAudio(_source, sound);
        ResetVelocity();
        Invoke("ReturnToPool", 0.5f);

        if (!PhotonManager.IsHost()) return;
        TakeDamageModule takeDamage = collision.gameObject.GetComponent<TakeDamageModule>();
        if (takeDamage == null) return;
        takeDamage.view.RPC("Attack", RpcTarget.All, player, damage);
    }

    private void OnEnable()
    {
        _render.enabled = true;
        _sphereCollider.enabled = true;
        Invoke("ReturnToPool", lifeTime);
    }

    #endregion
    public void SetData(IData data = null)
    {
        if (data == null || !(data is ShellData)) return;

        player = ((ShellData)data).player;

        damage = ((ShellData)data).damage;
    }

    public void SetTransfrom(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }

    public void ResetVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
    }

    private void ReturnToPool()
    {
        SpawnManager.ReleaseShellEvent(this);
    }
}

public class ShellData : IData
{
    public Player player;
    public int damage;
}
