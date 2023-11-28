using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initialize : MonoBehaviour
{
    private void Start()
    {
        InputManager.Initialzie();
        InputManager.Enable();
        Timing.RunCoroutine(InitLoadAll());
    }

    IEnumerator<float> InitLoadAll()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ResourceManager.initCoroutine));
        
        SceneManager.LoadScene("Menu");
    }
}
