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

    private Dictionary<TankType, TankModule> dictionaryTank;
    private bool _isReady;
    private Player _player;

    public bool IsReady => _isReady;
    private TankModule _currentTank;
    [Button]
    public void Initialize()
    {
        dictionaryTank = new Dictionary<TankType, TankModule>();
        foreach (TankType tankType in Enum.GetValues(typeof(TankType)))
        {
            dictionaryTank[tankType] = TankReferenceSO.InstanceTank(tankType.ToString(), transform, Quaternion.Euler(tankRotation)).GetComponent<TankModule>();
            dictionaryTank[tankType].gameObject.SetActive(false);
        }
        /*ActiveTank(type);*/
    }

    public void SetPlayer(Player player)
    {
        _player = player;
        ActiveTank((TankType) _player.CustomProperties["TankType"]);
    }

    private void ActiveTank(TankType type)
    {
        _currentTank = dictionaryTank[type];
        _currentTank?.gameObject.SetActive(true);
        _currentTank?.TankFree();
    }

    public void ChangeTank(TankType type)
    {
        _currentTank?.gameObject.SetActive(false);
        ActiveTank(type);
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
