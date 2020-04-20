using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeOptions : MonoBehaviour
{
    public Slider MusicSlider;
    public Slider UISlider;
    public Slider GameplaySlider;

    void Start()
    {
        // Set defaults if needed.
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetString("MusicVolume", "0.7");
        }
        if (!PlayerPrefs.HasKey("UIVolume"))
        {
            PlayerPrefs.SetString("UIVolume", "1.00");
        }
        if (!PlayerPrefs.HasKey("GameplayVolume"))
        {
            PlayerPrefs.SetString("GameplayVolume", "1.00");
        }

        MusicSlider.value = float.Parse(PlayerPrefs.GetString("MusicVolume"));
        UISlider.value = float.Parse(PlayerPrefs.GetString("UIVolume"));
        GameplaySlider.value = float.Parse(PlayerPrefs.GetString("GameplayVolume"));
    }

    void Update()
    {
        PlayerPrefs.SetString("MusicVolume", MusicSlider.value.ToString());
        PlayerPrefs.SetString("UIVolume", UISlider.value.ToString());
        PlayerPrefs.SetString("GameplayVolume", GameplaySlider.value.ToString());
    }
}
