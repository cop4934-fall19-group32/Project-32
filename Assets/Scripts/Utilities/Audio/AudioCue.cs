using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    MUSIC,
    UI,
    GAMEPLAY
}

public class AudioCue : MonoBehaviour
{
    public string Name;

    public List<AudioClip> AudioClips;

    public AudioSource Source = null;

    public AudioType Type;

    public void SetAudioSource()
    {
        Source = GetComponent<AudioSource>();
    }

    public void SetVolume()
    {
        if (Type == AudioType.MUSIC)
            Source.volume = float.Parse(PlayerPrefs.GetString("MusicVolume"));
        else if (Type == AudioType.UI)
            Source.volume = float.Parse(PlayerPrefs.GetString("UIVolume"));
        else
            Source.volume = float.Parse(PlayerPrefs.GetString("GameplayVolume"));
    }

    public void Play()
    {
        if (Source == null)
            SetAudioSource();
        Source.clip = AudioClips[Random.Range(0, AudioClips.Count)];
        SetVolume();
        Source.Play();
    }
}
