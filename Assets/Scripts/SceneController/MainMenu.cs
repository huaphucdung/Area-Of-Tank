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

    private List<TankModule> tankList;
    private int currentIndex = 0;
    private TankModule currentTank;

    private PlayerReusableData reusableData = new PlayerReusableData();
    private bool IsTest;
    private MainMenuUI ui;

    private void Start()
    {
        tankList = new List<TankModule>();
        IsTest = false;
        ShowMainMenuUI += ShowUI;
        Initialize();
        UIManager.HideAllUI<LoadingUI>();
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
        //if (currentTank == null) Debug.Log("Null current tank");
        currentTank?.Move(InputManager.playerAction.Move.ReadValue<Vector2>());
        currentTank?.TurretRotate(InputManager.playerAction.MousePosition.ReadValue<Vector2>());
    }

    private void Shoot(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        currentTank?.Shot(null);
    }
    #endregion

    private void Initialize()
    {
        foreach (string tankType in TankReferenceSO.GetListTankType())
        {
            TankModule newTank = TankReferenceSO.InstanceTank(tankType, tankTransform.position, tankTransform.rotation).GetComponent<TankModule>();
            newTank.Intialize(ResourceManager.GetTankData(tankType), reusableData);
            newTank.gameObject.SetActive(false);
            tankList.Add(newTank);
        }
        if (tankList.Count > 0)
        {
            SwapTank();
        }
    }

    [Button]
    public void NextLeftTank()
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

    [Button]
    public void NextRightTank()
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

    [Button]
    public void TestTank()
    {
        IsTest = true;
        InputManager.EnablePlayerAction();
        InputManager.playerAction.Shoot.started += Shoot;
        currentTank?.TankDefault();

        targetCamera.gameObject.SetActive(true);
    }

    [Button]
    public void ExitTest()
    {
        IsTest = false;
        InputManager.DisablePlayerAction();
        InputManager.playerAction.Shoot.started -= Shoot;
        currentTank?.StopAudio();
        currentTank?.DefaultTuretRotation();

        targetCamera.gameObject.SetActive(false);
        currentTank?.SetPosition(tankTransform.position, tankTransform.rotation);
    }

    [Button]
    public void JoinLobby()
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