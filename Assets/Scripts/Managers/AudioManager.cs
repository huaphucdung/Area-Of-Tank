using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    public static void PlayAudio(AudioSource source, AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public static void StopPlayAudio(AudioSource source)
    {
        source.Stop();
    }

    public static void PlayOneShotAudio(AudioSource source, AudioClip clip)
    {
        source.PlayOneShot(clip);
    }
}
