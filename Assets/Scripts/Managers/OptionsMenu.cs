using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public void SetMusicVolume(float volume)
    {
        volume = FixVolume(volume);
        audioMixer.SetFloat("music_Volume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        volume = FixVolume(volume);
        audioMixer.SetFloat("SFX_Volume", volume);
    }

    private float FixVolume(float volume)
    {
        volume *= 0.1f;
        if (volume == 0)
        {
            volume = 0.0001f;
        }
        return Mathf.Log10(volume) * 20;
    }
}
