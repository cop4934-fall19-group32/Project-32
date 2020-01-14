using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitPanel : MonoBehaviour
{
    public GameObject SubmitPanelObject;
    public void ShowSubmitPanel(bool success)
    {
        if (success)
            SubmitPanelObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Correct Output";
        else
            SubmitPanelObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Incorrect Output";

        SubmitPanelObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RemoveSubmitPanel()
    {
        SubmitPanelObject.SetActive(false);
        Time.timeScale = 1f;
    }
}