using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    [Header("Shell")]
    [SerializeField] private AssetReference shell;
    [SerializeField] private Transform bulletParent;

    [Header("BoxItem")]
    [SerializeField] private AssetReference boxItem;
    [SerializeField] private Transform boxItemParent;

    public static Func<Vector3, Quaternion,Shell> GetShellEvent;
    public static Action<Shell> ReleaseShellEvent;

    public static Action<BoxItem> ReleaseBoxItemEvent;

    private ObjectPool<Shell> shellPool;
    private ObjectPool<BoxItem> boxItemPool;

    private GameObject shellPerfab;

    private void Awake()
    {
        shellPool = new ObjectPool<Shell>(CreateShell, OnTakeShellFromPool, OnReturnShellToPool, OnDestroyShell, false, 50 , 1000);
        boxItemPool = new ObjectPool<BoxItem>(CreateBox, OnTakeBoxFromPool, OnReturnBoxToPool, OnDestroyBox, false, 10, 100);

        GetShellEvent += GetShell;
        ReleaseShellEvent += RelaseShell;
        ReleaseBoxItemEvent += RelaseBoxItem;
    }

    private void OnDestroy()
    {
        GetShellEvent -= GetShell;
        ReleaseShellEvent -= RelaseShell;
        ReleaseBoxItemEvent -= RelaseBoxItem;
    }

    private void Start()
    {
        //Init shell
        var handle = shell.LoadAssetAsync<GameObject>();
        handle.WaitForCompletion();
        shellPerfab = handle.Result;
    }

    #region Shell Method
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
        Shell newShell = Instantiate(shellPerfab).GetComponent<Shell>();
        newShell.gameObject.SetActive(false);
        newShell.transform.parent = bulletParent;
        Debug.Log("Create 1 new shell");
        return newShell;
    }

    private void OnTakeShellFromPool(Shell shell)
    {
        shell.ResetVelocity();
        shell.gameObject.SetActive(true);
    }

    private void OnReturnShellToPool(Shell shell)
    {
        shell.gameObject.SetActive(false);
    }

    
    private void OnDestroyShell(Shell shell)
    {
        GameObject.Destroy(shell.gameObject);
    }
    #endregion

    #region BoxItem Method
    private BoxItem GetBoxItem()
    {
        BoxItem boxItem = boxItemPool.Get();
        return boxItem;
    }
    private void RelaseBoxItem(BoxItem box)
    {
        boxItemPool.Release(box);
    }
    private BoxItem CreateBox()
    {
        var handle = boxItem.InstantiateAsync();
        handle.WaitForCompletion();
        BoxItem newBox = handle.Result.GetComponent<BoxItem>();
        newBox.gameObject.SetActive(false);

        newBox.transform.parent = boxItemParent;
        Debug.Log("Create 1 new BoxItem");
        return newBox;
    }

    private void OnTakeBoxFromPool(BoxItem box)
    {
        box.gameObject.SetActive(true);
    }

    private void OnReturnBoxToPool(BoxItem box)
    {
        box.gameObject.SetActive(false);
    }

    private void OnDestroyBox(BoxItem box)
    {
        GameObject.Destroy(box.gameObject);
    }
    #endregion
}
