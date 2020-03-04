using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitPanel : MonoBehaviour
{
    public GameObject SubmitPanelObject;
    public AudioSource solvedSound;

    public void ShowSubmitPanel()
    {
        solvedSound.Play();
        SubmitPanelObject.SetActive(true);
    }

    public void RemoveSubmitPanel()
    {
        SubmitPanelObject.SetActive(false);
    }
}