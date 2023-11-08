using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float damage;
    [SerializeField] private ParticleSystem boomEffect;
    [SerializeField] private AudioClip sound;

    private PlayerController controller;
    private AudioSource _source;
    private MeshRenderer _render;
    private SphereCollider _sphereCollider;
    private Rigidbody _rigidbody;
    public Rigidbody Rb => _rigidbody;

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

        controller = ((ShellData)data).controller;
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
        SpawnManager.RelaseShellEvent(this);
    }
}

public class ShellData : IData
{
    public PlayerController controller;
    public float damage;
}
