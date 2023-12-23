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

public enum Type
{
    Player
}

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("CameraSettings:")]
    [SerializeField] private CinemachineVirtualCamera cameraCV;

    [Header("Settings:")]
    [SerializeField] private AssetReference playerPrefabs;

    private PhotonView pv;

    private Map map;

    public static Action<IController> playerDeadAction;

    public static Action<Transform> setCameraTargetEvent;
   
    private GameObject playerInstance;

    private int numberPlayerLoaded;

    private PlayerController pc;

    #region Unity
    private void Awake()
    {
        Initiazlie();
        InstanceMap();

        var handle = playerPrefabs.LoadAssetAsync<GameObject>();
        handle.WaitForCompletion();
        DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;

        playerInstance = handle.Result;
        pool.ResourceCache.Add(playerInstance.name, playerInstance);

        numberPlayerLoaded = 0;
    }

    private void Start()
    {
        pv = GetComponent<PhotonView>();
        
        pv.RPC("AllPlayerReady", RpcTarget.AllBuffered);
    }

    private void Update()
    {
        pc?.DoUpdate();    
    }

    private void FixedUpdate()
    {
        pc?.DoFixedUpdate();
    }

    #endregion

    #region Main Methods
    private void Initiazlie() {
        setCameraTargetEvent += SetTargetCamera; 
        playerDeadAction += OnPlayerDead;
    }

    private void InstanceMap()
    {
        map = MapReferenceSO.InstanceMap(PhotonManager.GetCurrentRoom().CustomProperties["map"] as string);
    }

    [PunRPC]
    private void AllPlayerReady()
    {
        if (!PhotonManager.IsHost()) return;
        numberPlayerLoaded++;
        if (numberPlayerLoaded != PhotonManager.GetNumberPlayerInRoom()) return;

        Debug.Log("All Player Finish Load");

        pv.RPC("SpawnLocalPlayer", RpcTarget.All);
    }

    [PunRPC]
    private void SpawnLocalPlayer()
    {
        GameObject obj = PhotonNetwork.Instantiate(playerInstance.name, Vector3.zero, Quaternion.identity);
        Player localPlayer = PhotonManager.GetLocalPlayer();

        pc = (PlayerController) AddPlayer(obj, localPlayer.CustomProperties["TankType"] as string);
    }
    
    [PunRPC]
    private IController AddPlayer(GameObject obj, string tankType)
    {
        //Set Model Data
        IModel model = MVCFactory.CreateModel(Type.Player);
        model.Initialize();
        model.ApplyDesgin(new PlayerModelData
        {
            tankData = ResourceManager.GetTankData(tankType)
        });

        //Set View
        IView view = MVCFactory.CreateView(Type.Player, obj);
        view.Initialize();
        view.SpawnModel(new PlayerViewData
        {
            keyModel = tankType.ToString()
        });

        //Set Controller
        IController controller = MVCFactory.CreateController(Type.Player);
        controller.Initialize(new ControllerData
        {
            model = model,
            view = view,
        });
     
        return controller;
    }

    private void SetTargetCamera(Transform transform = null)
    {
        if (cameraCV == null) return;
        cameraCV.Follow = transform;
        cameraCV.LookAt = transform;
    }

    IEnumerator<float> NewPlayerTank(PlayerController PC, float wait = 0f)
    {
        yield return Timing.WaitForSeconds(wait);
        PC.SetTankPoistion(map.GetSpawnPosition());
        PC.Default();
    }
    #endregion

    #region Callback Methods
    

    private void OnPlayerDead(IController controller)
    {
        PlayerController PC = (PlayerController)controller;

        Timing.RunCoroutine(NewPlayerTank(PC, 2f));
    }
    #endregion
}


public class MVCFactory
{ 
    public static IModel CreateModel(Type type)
    {
        switch (type)
        {
            default:
                return new PlayerModel();
        }
    }

    public static IView CreateView(Type type, GameObject obj)
    {
        switch (type)
        {
            default:
                return obj.GetComponent<PlayerView>();
        }
    }

    public static IController CreateController(Type type)
    {
        switch (type)
        {
            default:
                return new PlayerController();
        }
    }
}