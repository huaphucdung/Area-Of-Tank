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

    private List<TankModule> tankList;
    private int currentIndex = 0;
    private TankModule currentTank;

    private PlayerReusableData reusableData = new PlayerReusableData();
    private bool IsTest;

    private void Start()
    {
        InputManager.Initialzie();
        InputManager.Enable();

        tankList = new List<TankModule>();
        IsTest = false;
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

    [Button]
    private void Initialize()
    {
        foreach (TankType tankType in Enum.GetValues(typeof(TankType)))
        {
            TankModule newTank = TankReferenceSO.InstanceTank(tankType.ToString(), tankTransform.position, tankTransform.rotation).GetComponent<TankModule>();
            newTank.Intialize(ResourceManager.GetTankDataByType(tankType), reusableData);
            newTank.gameObject.SetActive(false);
            tankList.Add(newTank);
        }
        if(tankList.Count > 0)
        {
            SwapTank();
        }
    }

    [Button]
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

    [Button]
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

    [Button]
    private void TestTank()
    {
        IsTest = true;
        InputManager.EnablePlayerAction();
        InputManager.playerAction.Shoot.started += Shoot;
        currentTank?.TankDefault();

        targetCamera.gameObject.SetActive(true);
    }

    [Button]
    private void ExitText()
    {
        IsTest = false;
        InputManager.DisablePlayerAction();
        InputManager.playerAction.Shoot.started -= Shoot;
        currentTank?.StopAudio();
        currentTank?.DefaultTuretRotation();

        targetCamera.gameObject.SetActive(false);
        currentTank?.SetPosition(tankTransform.position, tankTransform.rotation);
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

public class LoginHandle
{
    
}

public class RegiserHandle
{
    
}