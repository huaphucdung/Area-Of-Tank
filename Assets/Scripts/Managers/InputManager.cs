using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    public static InputController input;
    public static InputController.PlayerActions playerAction;
    public static InputController.UIActions uiAction;
    public static void Initialzie()
    {
        if (input == null)
        {
            input = new InputController();
            playerAction = input.Player;
            uiAction = input.UI;
        }
    }

    public static void Enable()
    {
        input?.Enable();
    }

    public static void Disable()
    {
        input?.Disable();
    }
    
    public static void EnablePlayerAction()
    {
        if (input == null) return;
        playerAction.Enable();
    }

    public static void DisablePlayerAction()
    {
        if (input == null) return;
        playerAction.Disable();
    }
}
