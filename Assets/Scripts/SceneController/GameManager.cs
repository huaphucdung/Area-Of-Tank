using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using MEC;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("CameraSettings:")]
    [SerializeField] private CinemachineVirtualCamera cameraCV;

    public static Action<Transform> setCameraTargetEvent;
    private int numberPlayerLoaded;
    private int numberTank;
    private PhotonView pv;

    private Map map;
    public ReusableData reusableData;
    private TankModule mineTank;
   
    #region Unity
    private void Awake()
    {
        Initiazlie();
        InstanceMap();
        numberTank = numberPlayerLoaded = 0;
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        pv.RPC("OnFinishLoaded", RpcTarget.AllBuffered);
    }

    private void FixedUpdate()
    {
        if (mineTank == null || !mineTank.IsInit) return;
        mineTank?.Move(InputManager.playerAction.Move.ReadValue<Vector2>(), reusableData);
        mineTank?.TurretRotate(InputManager.playerAction.MousePosition.ReadValue<Vector2>());
        if (InputManager.playerAction.Shoot.triggered)
        {
            mineTank?.Shot(reusableData);
        }
    }

    #endregion

    #region Init Methods
    private void Initiazlie()
    {
        setCameraTargetEvent += SetTargetCamera;
        reusableData = new ReusableData();
        reusableData.Initialize();
    }

    private void InstanceMap()
    {
        map = MapReferenceSO.InstanceMap(PhotonManager.GetCurrentRoom().CustomProperties["map"] as string);
    }

    [PunRPC]
    private void SpawnLocalPlayer()
    {
        GameObject tankLocal = TankReferenceSO.PhotonInstantiateTank(PhotonManager.GetLocalPlayer().CustomProperties["TankType"] as string,
            map.GetSpawnPositionByIndex(PhotonManager.GetPlayerIndex()), Quaternion.identity);
        mineTank = tankLocal.GetComponent<TankModule>();
        mineTank.pv.RPC("InitializePhoton", RpcTarget.AllBuffered);
        SetTargetCamera(mineTank.transform);

        pv.RPC("OnFinishSpawnTank", RpcTarget.MasterClient);
    }

    private void SetTargetCamera(Transform transform = null)
    {
        if (cameraCV == null) return;
        cameraCV.Follow = transform;
        cameraCV.LookAt = transform;
    }
    #endregion

    #region Start Game Method
    [PunRPC]
    private void StartGame()
    {
        SetTankDefault();
    }
    #endregion

    #region In Gameplay Methods

    #endregion

    #region Endgame Methods

    #endregion

    #region Tank Methods
    private void SetTankDefault()
    {
        if (mineTank == null) return;
        reusableData.SetDefault(mineTank.data);
        mineTank.TankDefault();

        mineTank.GetComponent<TakeDamageModule>().TakeDameEvent += reusableData.ChangeHealth;
    }

    private void SetTankDead()
    {
        if (mineTank == null) return;
        reusableData.SetDead();
        mineTank.TankDead();

        mineTank.GetComponent<TakeDamageModule>().TakeDameEvent -= reusableData.ChangeHealth;
    }

    #endregion


    #region Callback Methods
    [PunRPC]
    private void OnFinishLoaded()
    {
        if (!PhotonManager.IsHost()) return;
        numberPlayerLoaded++;
        if (numberPlayerLoaded != PhotonManager.GetNumberPlayerInRoom()) return;
        pv.RPC("SpawnLocalPlayer", RpcTarget.All);
        
    }

    [PunRPC]
    private void OnFinishSpawnTank()
    {
        if (!PhotonManager.IsHost()) return;
        numberTank++;
        if (numberTank != PhotonManager.GetNumberPlayerInRoom()) return;
        pv.RPC("StartGame", RpcTarget.All);
    }
    #endregion
}
