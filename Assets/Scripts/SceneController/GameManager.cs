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
    private PhotonView pv;

    private Map map;
    private ReusableData reusableData;
    private TankModule mineTank;
   
    #region Unity
    private void Awake()
    {
        Initiazlie();
        InstanceMap();
        numberPlayerLoaded = 0;
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        pv.RPC("OnFinishLoaded", RpcTarget.AllBuffered);
    }

   

    private void FixedUpdate()
    {
        if (mineTank == null || !mineTank.IsInit) return;
        mineTank?.Move(InputManager.playerAction.Move.ReadValue<Vector2>());
        mineTank?.TurretRotate(InputManager.playerAction.MousePosition.ReadValue<Vector2>());
    }

    #endregion

    #region Main Methods
    private void Initiazlie()
    {
        setCameraTargetEvent += SetTargetCamera;
        reusableData = new ReusableData();
        reusableData.Initialize();
        /*playerDeadAction += OnPlayerDead;*/
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
        mineTank.Intialize(ResourceManager.GetTankData(PhotonManager.GetLocalPlayer().CustomProperties["TankType"] as string), reusableData);

        SetTargetCamera(mineTank.transform);
    }

    private void SetTargetCamera(Transform transform = null)
    {
        if (cameraCV == null) return;
        cameraCV.Follow = transform;
        cameraCV.LookAt = transform;
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
    #endregion
}
