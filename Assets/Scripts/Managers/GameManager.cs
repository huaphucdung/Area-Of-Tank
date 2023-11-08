using Sirenix.OdinInspector;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;
using MEC;

public enum Type
{
    Player
}

public class GameManager : MonoBehaviour
{
    [Header("CameraSettings:")]
    [SerializeField] private CinemachineVirtualCamera cameraCV;
    
    [Header("Settings:")]
    [SerializeField] private AssetReference playerPrefabs;
    [SerializeField] private Transform playerParent;

    private List<IController> controllers;
    private Map map;

    public event Action<IController> createPlayerEvent;
    public static Action<IController> playerDeadAction;

    public static Action<Transform> setCameraTargetEvent;
   

    #region Unity
    private void Awake()
    {
        Initiazlie();
        /*InstanceMap();*/
    }

    private void Update()
    {
        if(controllers != null)
        {
            foreach (var controller in controllers)
            {
                controller.DoUpdate();
            }
        }    
    }

    private void FixedUpdate()
    {
        if (controllers != null)
        {
            foreach (var controller in controllers)
            {
                controller.DoFixedUpdate();
            }
        }
    }
    #endregion

    #region Main Methods
    private void Initiazlie() {
        controllers = new List<IController>();

        InputManager.Initialzie();
        InputManager.Enable();

        setCameraTargetEvent += SetTargetCamera;
        createPlayerEvent += OnPlayerCreated;
        playerDeadAction += OnPlayerDead;
    }

    [Button]
    private void InstanceMap()
    {
        map = MapReferenceSO.InstanceMap("Islan");
    }

    private void AddController(Type type, TankType tankType)
    {
        switch (type)
        {
            default:
                var handle = playerPrefabs.InstantiateAsync();
                handle.WaitForCompletion();
                AddPlayer(handle.Result, tankType);
                break;
        }
    }

    private void AddPlayer(GameObject obj, TankType tankType)
    {
        //Set Model Data
        IModel model = MVCFactory.CreateModel(Type.Player);
        model.Initialize();
        model.ApplyDesgin(new PlayerModelData
        {
            tankData = ResourceManager.GetTankDataByType(tankType)
        });

        //Set View
        IView view = MVCFactory.CreateView(Type.Player, obj);
        view.Initialize();
        view.SpawnModel(new PlayerViewData { 
            keyModel = tankType.ToString()
        });

        //Set Controller
        IController controller = MVCFactory.CreateController(Type.Player);
        controller.Initialize(new ControllerData
        {
            model = model,
            view = view,
        });
        controllers.Add(controller);
        createPlayerEvent?.Invoke(controller);
    }

    private void SetTargetCamera(Transform transform = null)
    {
        if (cameraCV == null) return;
        cameraCV.Follow = transform;
        cameraCV.LookAt = transform;
    }

    ///Test
    [Button]
    private void TestAddPlayer(TankType type)
    {
        AddController(Type.Player, type);
    }

    IEnumerator<float> NewPlayerTank(PlayerController PC, float wait = 0f)
    {
        yield return Timing.WaitForSeconds(wait);
        PC.SetTankPoistion(map.GetSpawnPosition());
        PC.Default();
    }
    #endregion

    #region Callback Methods
    private void OnPlayerCreated(IController controller)
    {
        Debug.Log("Player Created Callback Trigger");
        PlayerController PC = (PlayerController)controller;
        PC.playerView.transform.SetParent(playerParent);
        
        Timing.RunCoroutine(NewPlayerTank(PC));
    }

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