using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuUI : BaseUI
{
    //Player Information
    private Label playerName;
    private Label playerCoin;
    
    //Top Button
    private Button settingBtn;
    private Button quitBtn;

    //Tank Button    
    private Button leftTankBtn;
    private Button rightTankBtn;
    private Button testTankBtn;

    private Button buyTankBtn;

    //Lobby
    private Button lobbyBtn;


    public override void Initialize(UIData data = null)
    {
        base.Initialize(data);

        playerName = root.Q<Label>("PlayerName");
        playerCoin = root.Q<Label>("PlayerCoin");

        settingBtn = root.Q<Button>("SettingBtn");

        leftTankBtn = root.Q<Button>("LeftBtn");
        rightTankBtn = root.Q<Button>("RightBtn");

        testTankBtn = root.Q<Button>("TestBtn");

        lobbyBtn = root.Q<Button>("ToLobbyBtn");
    }

    public override void Show(UIShowData data = null)
    {
        base.Show(data);

        leftTankBtn.clicked += LeftTank;
        rightTankBtn.clicked += RightTank;

        testTankBtn.clicked += TestTank;

        lobbyBtn.clicked += ToLobby;
    }

   
    public override void Hide()
    {
        leftTankBtn.clicked -= LeftTank;
        rightTankBtn.clicked -= RightTank;

        testTankBtn.clicked -= TestTank;

        lobbyBtn.clicked -= ToLobby;

        base.Hide();
    }

    private void LeftTank()
    {
        MainMenu.NextLeftTankAction?.Invoke();
    }

    private void RightTank()
    {
        MainMenu.NextRightTankAction?.Invoke();
    }

    private void TestTank()
    {
        MainMenu.TestTankAction?.Invoke();
    }

    private void ToLobby()
    {
        MainMenu.JoinLobbyAction?.Invoke();
    }
}
