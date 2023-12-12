using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Initialize : MonoBehaviour
{
    private void Start()
    {
        UIManager.Initialize();
        Timing.RunCoroutine(InitLoadAll());
    }

    IEnumerator<float> InitLoadAll()
    {
        LoadingUI loading = UIManager.GetAndShowUI<LoadingUI>();
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ResourceManager.initCoroutine));

        InputManager.Initialzie();
        InputManager.Enable();

        SceneManager.LoadScene("Menu");
    }
}
