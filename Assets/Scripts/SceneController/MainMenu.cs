using Cinemachine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("MainMenu Object:")]
    [SerializeField] private Transform tankTransform;
    [SerializeField] private CinemachineVirtualCamera targetCamera;

    public static Action ShowMainMenuUI;
    public static Action NextLeftTankAction;
    public static Action NextRightTankAction;
    public static Action TestTankAction;
    public static Action ExitTestTankAction;
    public static Action JoinLobbyAction;

    private List<TankModule> tankList;
    private int currentIndex = 0;
    private TankModule currentTank;

    private ReusableData reusableData;
    private bool IsTest;
    private MainMenuUI ui;

    private void Start()
    {
        tankList = new List<TankModule>();
        reusableData = new ReusableData();
        IsTest = false;
        
        ShowMainMenuUI += ShowUI;
        Initialize();
        UIManager.HideAllUI<LoadingUI>();


        NextLeftTankAction += NextLeftTank;
        NextRightTankAction += NextRightTank;
        TestTankAction += TestTank;
        ExitTestTankAction += ExitTest;
        JoinLobbyAction += JoinLobby;

        Lobby.joinLobbySuccessAction += HideUI;
    }

    private void ShowUI()
    {
        ui = UIManager.GetAndShowUI<MainMenuUI>();
    }

    private void HideUI()
    {
        ui.Hide();
    }

    #region Tank Test Control Method
    private void FixedUpdate()
    {
        if (!IsTest) return;
        
        currentTank?.Move(InputManager.playerAction.Move.ReadValue<Vector2>(), reusableData);
        currentTank?.TurretRotate(InputManager.playerAction.MousePosition.ReadValue<Vector2>());
        if(InputManager.playerAction.Shoot.triggered)
        {
            currentTank?.ShotTest(reusableData);
        }
    }
    #endregion

    private void Initialize()
    {
        reusableData.Initialize();

        foreach (string tankType in TankReferenceSO.GetListTankType())
        {
            TankModule newTank = TankReferenceSO.InstanceTank(tankType, tankTransform.position, tankTransform.rotation).GetComponent<TankModule>();
            newTank.Intialize(ResourceManager.GetTankData(tankType));
            newTank.gameObject.SetActive(false);
            tankList.Add(newTank);
        }
        if (tankList.Count > 0)
        {
            SwapTank();
        }
    }

    
    private void NextLeftTank()
    {
        if(currentIndex == 0)
        {
            currentIndex = tankList.Count - 1;
        }
        else
        {
            currentIndex--;
        }
        SwapTank();
    }

    
    private void NextRightTank()
    {
        if (currentIndex == tankList.Count - 1)
        {
            currentIndex = 0;
        }
        else
        {
            currentIndex++;
        }
        SwapTank();
    }

    
    private void TestTank()
    {
        IsTest = true;
        InputManager.EnablePlayerAction();
        currentTank?.TankDefault();
        targetCamera.gameObject.SetActive(true);
        ui.Hide();
    }

    private void ExitTest()
    {
        IsTest = false;
        InputManager.DisablePlayerAction();
        currentTank?.StopAudio();
        currentTank?.DefaultTuretRotation();

        targetCamera.gameObject.SetActive(false);
        currentTank?.SetPosition(tankTransform.position, tankTransform.rotation);
        ui.Show();
    }

    
    private void JoinLobby()
    {
        PhotonManager.JoinLobby();
    }

    private void SwapTank()
    {
        currentTank?.gameObject.SetActive(false);
        currentTank = tankList[currentIndex];
        currentTank?.gameObject.SetActive(true);
        SetCameraForTank(currentTank?.gameObject.transform);
    }

    private void SetCameraForTank(Transform obj)
    {
        targetCamera.Follow = obj;
        targetCamera.LookAt = obj;
    }
}