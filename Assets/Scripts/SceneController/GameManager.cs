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
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("CameraSettings:")]
    [SerializeField] private CinemachineVirtualCamera cameraCV;
    [SerializeField] private float timeRespawn = 3f;
    public static Action<Transform> setCameraTargetEvent;
    private int numberPlayerLoaded;
    private int numberTank;

    public static PhotonView pv;
    private Map map;
    public ReusableData reusableData;
    private TankModule mineTank;
    private Dictionary<Player, PlayerScore> playerScores;

    private bool isDead;
    private const byte StartGameEventCode = 0;
    private const byte EndGameEventCode = 1;
    private const byte SendScoreEventCode = 2;

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
        if (mineTank == null || !mineTank.IsInit || isDead) return;

        mineTank?.Move(InputManager.playerAction.Move.ReadValue<Vector2>(), reusableData);
        mineTank?.TurretRotate(InputManager.playerAction.MousePosition.ReadValue<Vector2>());
        if (InputManager.playerAction.Shoot.triggered)
        {
            mineTank?.Shot(reusableData);
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    public override void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    #endregion

    #region Init Methods
    private void Initiazlie()
    {
        reusableData = new ReusableData();
        playerScores = new Dictionary<Player, PlayerScore>();
        reusableData.Initialize();

        setCameraTargetEvent += SetTargetCamera;
    }

    private void OnDestroy()
    {
        setCameraTargetEvent -= SetTargetCamera;
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

        mineTank.tankDefaultTrigger += OnTankDefault;
        mineTank.tankDeadTrigger += OnTankDead;

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
    private void StartGame()
    {
        Debug.Log("StartGame");
        //Set tank dafault
        mineTank?.pv.RPC("TankDefault", RpcTarget.All);
    }
    #endregion

    #region In Gameplay Methods
    public static void SendScore(Player whoKill, Player whoDead)
    {
        object[] content = new object[2];
        content[0] = whoKill;
        content[1] = whoDead;

        //Send score to all player;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SendScoreEventCode, content, raiseEventOptions , SendOptions.SendReliable);
    }

    private void UpdateScore(Player whoKill, Player whoDead)
    {
        if (playerScores.ContainsKey(whoKill))
        {
            playerScores[whoKill].Kill();
        }

        if (playerScores.ContainsKey(whoDead))
        {
            playerScores[whoDead].Dead();
        }
    }
    

    #endregion

    #region Endgame Methods

    #endregion

    #region Callback Methods
    [PunRPC]
    private void OnFinishLoaded()
    {
        playerScores.Clear();
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerScores[player] = new PlayerScore();
        }

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
        
        /*pv.RPC("StartGame", RpcTarget.All);*/
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(StartGameEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnTankDefault()
    {
        if (mineTank == null) return;
        isDead = false;
        reusableData.SetDefault(mineTank.data);
        mineTank.GetComponent<TakeDamageModule>().TakeDameEvent += reusableData.ChangeHealth;
    }

    private void OnTankDead()
    {
        if (mineTank == null) return;
        isDead = true;
        reusableData.SetDead();
        mineTank.GetComponent<TakeDamageModule>().TakeDameEvent -= reusableData.ChangeHealth;

        //Check codition before respawn
        Timing.RunCoroutine(RespawnTank());
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        switch(eventCode)
        {
            case StartGameEventCode:
                StartGame();
                break;
            case EndGameEventCode:
                break;
            case SendScoreEventCode:
                object[] data = (object[])photonEvent.CustomData;
                UpdateScore((Player)data[0], (Player)data[1]);
                break;
        }
    }
    #endregion

    IEnumerator<float> RespawnTank()
    {
        yield return Timing.WaitForSeconds(timeRespawn);
        mineTank?.SetPosition(map.GetSpawnPosition());
        mineTank?.pv.RPC("TankDefault", RpcTarget.All);
    }
}

public class PlayerScore
{
    public int kill;
    public int dead;

    public PlayerScore()
    {
        kill = 0;
        dead = 0;
    }

    public void Kill()
    {
        kill++;
    }

    public void Dead()
    {
        dead++;
    }
}