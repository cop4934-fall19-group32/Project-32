using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HaltSound : MonoBehaviour
{
    public AudioSource HaltAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => PlaySound());
    }
    void PlaySound()
    {
        HaltAudioSource.Play();
    }
}
