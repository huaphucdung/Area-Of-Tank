using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private AssetReference shell;
    [SerializeField] private Transform bulletParent;

    public static Func<Vector3, Quaternion,Shell> GetShellEvent;
    public static Action<Shell> RelaseShellEvent;

    private ObjectPool<Shell> shellPool;

    private void Awake()
    {
        shellPool = new ObjectPool<Shell>(CreateShell, OnTakeShellFromPoll, OnRetrunShellToPoll, OnDestroyShell, false, 50 , 1000);

        GetShellEvent += GetShell;
        RelaseShellEvent += RelaseShell;
    }

    private Shell GetShell(Vector3 postion, Quaternion rotation)
    {
        Shell shellGet = shellPool.Get();
        shellGet.SetTransfrom(postion, rotation);
        return shellGet;
    }

    private void RelaseShell(Shell shell)
    {
        shellPool.Release(shell);
    }

    private Shell CreateShell()
    {
        var handle = shell.InstantiateAsync();
        handle.WaitForCompletion();
        Shell newShell = handle.Result.GetComponent<Shell>();
        newShell.gameObject.SetActive(false);

        newShell.transform.parent = bulletParent;
        Debug.Log("Create 1 new shell");
        return newShell;
    }

    private void OnTakeShellFromPoll(Shell shell)
    {
        shell.ResetVelocity();
        shell.gameObject.SetActive(true);
    }

    private void OnRetrunShellToPoll(Shell shell)
    {
        shell.gameObject.SetActive(false);
    }

    
    private void OnDestroyShell(Shell shell)
    {
        GameObject.Destroy(shell.gameObject);
    }

    
}
