using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    void Awake()
    {
        GetComponent<AudioCue>().SetAudioSource();
        GetComponent<AudioCue>().Source.loop = true;
        GetComponent<AudioCue>().Play();
    }

    void Update()
    {
        GetComponent<AudioCue>().SetVolume();
    }
}
