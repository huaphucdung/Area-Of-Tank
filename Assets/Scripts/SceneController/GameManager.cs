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
using DG.Tweening;

public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("CameraSettings:")]
    [SerializeField] private CinemachineVirtualCamera cameraCV;
    
    [Header("Settings:")]
    [SerializeField] private float timeRespawnTank = 3f;
    [SerializeField] private float timeRespawnBoxItem = 10f;

    public static Action<Transform> setCameraTargetEvent;
    private int numberPlayerLoaded;
    private int numberTank;

    private Map map;
    private BaseModeSO mode;

    public ReusableData reusableData;
    private TankModule mineTank;
    private TakeDamageModule mineTakeDamage;
    private EffectModule effectModule;

    private Dictionary<Player, PlayerScore> playerScores;
    private Dictionary<int, BoxItem> boxItemDictionary;


    private const byte LocalPlayerLoadedEventCode = 10;
    private const byte SpawnTankEventCode = 12;
    private const byte StartGameEventCode = 40;

    private const byte HideBoxItemEventCode = 42;
    private const byte ShowBoxItemEventCode = 43;

    private const byte EndGameEventCode = 50;
    private const byte SendScoreEventCode = 45;
    private const byte SendDeadFlagEventCode = 47;

    #region Unity
    private void Awake()
    {
        Initiazlie();
        
    }

    private void Start()
    {
        numberTank = numberPlayerLoaded = 0;
        reusableData.Initialize();
        //Load Map
        InstanceMap();
        //Set Mode
        mode = ModeReferenceSO.GetModeReference(PhotonManager.GetCurrentRoom().CustomProperties["mode"] as string);
        //Set Score        
        playerScores.Clear();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerScores[player] = new PlayerScore();
        }

        //Send Noitice to Host
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient};
        PhotonNetwork.RaiseEvent(LocalPlayerLoadedEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    private void FixedUpdate()
    {
        if (mineTank == null || !mineTank.IsInit || reusableData.IsDead()) return;

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
        boxItemDictionary = new Dictionary<int, BoxItem>();

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

    private void SpawnLocalPlayer()
    {
        GameObject tankLocal = TankReferenceSO.PhotonInstantiateTank(PhotonManager.GetLocalPlayer().CustomProperties["TankType"] as string,
            map.GetSpawnPositionByIndex(PhotonManager.GetPlayerIndex()), Quaternion.identity);
        
        mineTank = tankLocal.GetComponent<TankModule>();
        mineTakeDamage = tankLocal.GetComponent<TakeDamageModule>();
        effectModule = tankLocal.GetComponent<EffectModule>();

        mineTank.pv.RPC("InitializePhoton", RpcTarget.All);
        
        SetTargetCamera(mineTank.transform);

        mineTank.tankDefaultTrigger += OnTankDefault;
        mineTank.tankDeadTrigger += OnTankDead;
        effectModule.getDataFunc += GetReusableData;
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
        int id = 0;
        Debug.Log("StartGame");
        //Spawn Tank at local
        SpawnLocalPlayer();
        mineTank?.pv.RPC("TankDefault", RpcTarget.All);
        //Spawn All boxItem
        foreach (Transform boxItemPosition in map.GetBoxPositions)
        {
            BoxItem newBox = SpawnManager.GetBoxItemEvent.Invoke(boxItemPosition.position);
            newBox.disableAction += SetRespawnBoxItem;
            newBox.SetID(id);
            boxItemDictionary[id] = newBox;
            id++;
        }
        //Set Time
        float furrentTime = mode.maxTime;
        DOTween.To(() => furrentTime, x => furrentTime = x, 0f, mode.maxTime).OnComplete(() => {
            if (!PhotonManager.IsHost()) return;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(EndGameEventCode, null, raiseEventOptions, SendOptions.SendReliable);
        });
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
    
    private void SetPlayerDeadFlag(Player player)
    {
        if (playerScores.ContainsKey(player))
        {
            playerScores[player].SetGameOver();
        }

        if (!PhotonManager.IsHost()) return;

        int numberPlayerGameOver = 0;
        foreach(KeyValuePair<Player, PlayerScore> score in playerScores)
        {
            if (score.Value.gameOver) numberPlayerGameOver++;
        }

        if(numberPlayerGameOver >= (PhotonManager.GetNumberPlayerInRoom() - 1))
        {
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(EndGameEventCode, null, raiseEventOptions, SendOptions.SendReliable);
        }
    }
    #endregion

    #region Endgame Methods
    private void EndGame()
    {
        Debug.Log("endgame");
    }
    #endregion

    #region Callback Methods
    private void OnFinishLoaded()
    {
        //Check all player finish loaded tell they start game
        numberPlayerLoaded++;
        if (numberPlayerLoaded != PhotonManager.GetNumberPlayerInRoom()) return;
        
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All};
        PhotonNetwork.RaiseEvent(StartGameEventCode, null, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnTankDefault()
    {
        if (mineTank == null) return;
        reusableData.SetDefault(mineTank.data);
        mineTakeDamage.TakeDameEvent += OnTankHealthChange;
    }

    private void OnTankDead()
    {
        if (mineTank == null) return;
        reusableData.SetDead();
        mineTakeDamage.TakeDameEvent -= OnTankHealthChange;

        //Check can respawn
        PlayerScore score = playerScores[mineTank.pv.Owner];
        if (!mode.CanRespawn(score.kill, score.dead))
        {
            object[] content = new object[2];
            content[0] = mineTank.pv.Owner;
            
            //Send to all player;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(SendDeadFlagEventCode, content, raiseEventOptions, SendOptions.SendReliable);
            return;
        }
        Timing.RunCoroutine(RespawnTank());
    }

    private void OnTankHealthChange(Player playerKill, Player playerDead, int value)
    {
        if(reusableData.SubHealth(value)) mineTank.pv.RPC("ShowTakeDamageEffect", RpcTarget.All);
        
        //Send current health for all player
        if (reusableData.IsDead())
        {
            mineTank.pv.RPC("TankDead", RpcTarget.All);
            SendScore(playerKill, playerDead);   
        }
    }

    private ReusableData GetReusableData()
    {
        return reusableData;
    }

    private void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        switch(eventCode)
        {
            case LocalPlayerLoadedEventCode:
                OnFinishLoaded();
                break;
            case SpawnTankEventCode:
                break;
            case StartGameEventCode:
                StartGame();
                break;
            case EndGameEventCode:
                EndGame();
                break;
            case SendScoreEventCode:
                object[] scoreData = (object[])photonEvent.CustomData;
                UpdateScore((Player)scoreData[0], (Player)scoreData[1]);
                break;
            case SendDeadFlagEventCode:
                object[] playerDieData = (object[])photonEvent.CustomData;
                SetPlayerDeadFlag((Player) playerDieData[0]);
                break;
            case HideBoxItemEventCode:
                object[] hideBoxData = (object[])photonEvent.CustomData;
                if (boxItemDictionary.ContainsKey((int)hideBoxData[0]))
                {
                    boxItemDictionary[(int)hideBoxData[0]].gameObject.SetActive(false);
                }
                break;
            case ShowBoxItemEventCode:
                object[] showBoxData = (object[])photonEvent.CustomData;
                if (boxItemDictionary.ContainsKey((int)showBoxData[0]))
                {
                    boxItemDictionary[(int)showBoxData[0]].gameObject.SetActive(true);
                }
                break;
        }
    }
    #endregion

    private void SetRespawnBoxItem(BoxItem box)
    {
        //Send All turn off boxItem
        object[] content = new object[1];
        content[0] = box.id;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(HideBoxItemEventCode, content, raiseEventOptions, SendOptions.SendReliable);
        
        Timing.RunCoroutine(RespawnBoxItem(box.id));
    }

    IEnumerator<float> RespawnTank()
    {
        yield return Timing.WaitForSeconds(timeRespawnTank);
        mineTank?.SetPosition(map.GetSpawnPosition());
        mineTank?.pv.RPC("TankDefault", RpcTarget.All);
    }

    IEnumerator<float> RespawnBoxItem(int boxItemId)
    {
        yield return Timing.WaitForSeconds(timeRespawnBoxItem);
        object[] content = new object[1];
        content[0] = boxItemId;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(ShowBoxItemEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}

public class PlayerScore
{
    public int kill;
    public int dead;
    public bool gameOver;
    public PlayerScore()
    {
        kill = 0;
        dead = 0;
        gameOver = false;
    }

    public void Kill()
    {
        kill++;
    }

    public void Dead()
    {
        dead++;
    }

    public void SetGameOver()
    {
        gameOver = true;    
    }
}