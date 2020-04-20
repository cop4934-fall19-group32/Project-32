using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolume : MonoBehaviour
{
    public Slider Volume;

    void Start()
    {
        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetString("MasterVolume", "1.00");
        }

        float volumeSaved = float.Parse(PlayerPrefs.GetString("MasterVolume"));
        AudioListener.volume = volumeSaved;
        Volume.value = volumeSaved;
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.volume = Volume.value;
        PlayerPrefs.SetString("MasterVolume", Volume.value.ToString());
    }
}
