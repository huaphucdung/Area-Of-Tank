using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using Photon.Realtime;

public class RoomItem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private Vector3 tankRotation = Vector3.zero;
    [SerializeField] private Color readyColor;
    [SerializeField] private Color unreadyColor;

    private Dictionary<string, TankModule> dictionaryTank;
    private bool _isReady;
    private Player _player;
    public bool IsReady => _isReady;
    private TankModule _currentTank;
    
    public void Initialize()
    {
        dictionaryTank = new Dictionary<string, TankModule>();
        
        foreach (string tankType in TankReferenceSO.GetListTankType())
        {
            dictionaryTank[tankType] = TankReferenceSO.InstanceTank(tankType, transform, Quaternion.Euler(tankRotation)).GetComponent<TankModule>();
            dictionaryTank[tankType].gameObject.SetActive(false);
        }
    }

    public void SetPlayer(Player player)
    {
        _player = player;
        ChangeTank(player.CustomProperties["TankType"] as string);
    }

    public void SetEmpty()
    {
        _player = null;
        gameObject.SetActive(false);
    }

    private void ActiveTank(string tankType)
    {
        _currentTank = dictionaryTank[tankType];
        _currentTank?.gameObject.SetActive(true);
        _currentTank?.TankFree();
    }

    public void ChangeTank(string tankType)
    {
        _currentTank?.gameObject.SetActive(false);
        ActiveTank(tankType);
    }

    public void SetReady()
    {
        _isReady = true;
        text.color = readyColor;
        text.text = "Ready";
    }

    public void SetUnready()
    {
        _isReady = false;
        text.color = unreadyColor;
        text.text = "PlayerName";
    }

}
