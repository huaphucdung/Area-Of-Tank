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

    public static Func<Vector3, BoxItem> GetBoxItemEvent;
    public static Action<BoxItem> ReleaseBoxItemEvent;

    private ObjectPool<Shell> shellPool;
    private ObjectPool<BoxItem> boxItemPool;

    private GameObject shellPrefab;
    private GameObject boxItemPrefab;

    private void Awake()
    {
        shellPool = new ObjectPool<Shell>(CreateShell, OnTakeShellFromPool, OnReturnShellToPool, OnDestroyShell, false, 50 , 1000);
        boxItemPool = new ObjectPool<BoxItem>(CreateBox, OnTakeBoxFromPool, OnReturnBoxToPool, OnDestroyBox, false, 10, 100);

        GetShellEvent += GetShell;
        ReleaseShellEvent += RelaseShell;

        GetBoxItemEvent += GetBoxItem;
        ReleaseBoxItemEvent += RelaseBoxItem;
    }

    private void OnDestroy()
    {
        GetShellEvent -= GetShell;
        ReleaseShellEvent -= RelaseShell;
        GetBoxItemEvent -= GetBoxItem;
        ReleaseBoxItemEvent -= RelaseBoxItem;
    }

    private void Start()
    {
        //Load and get prefab shell and boxItem;
        var handleShell = shell.LoadAssetAsync<GameObject>();
        var handleBoxItem = boxItem.LoadAssetAsync<GameObject>();
        handleShell.WaitForCompletion();
        shellPrefab = handleShell.Result;
        handleBoxItem.WaitForCompletion();
        boxItemPrefab = handleBoxItem.Result;
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
        Shell newShell = Instantiate(shellPrefab).GetComponent<Shell>();
        newShell.gameObject.SetActive(false);
        newShell.transform.parent = bulletParent;
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
    private BoxItem GetBoxItem(Vector3 position)
    {
        BoxItem boxItem = boxItemPool.Get();
        boxItem.transform.position = position;
        return boxItem;
    }
    private void RelaseBoxItem(BoxItem box)
    {
        boxItemPool.Release(box);
    }
    private BoxItem CreateBox()
    {
        BoxItem newBox = Instantiate(boxItemPrefab).GetComponent<BoxItem>();
        newBox.gameObject.SetActive(false);
        newBox.transform.parent = boxItemParent;
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
