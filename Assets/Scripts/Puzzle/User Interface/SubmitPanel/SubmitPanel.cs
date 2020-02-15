using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitPanel : MonoBehaviour
{
    public GameObject SubmitPanelObject;
    public void ShowSubmitPanel()
    {
        SubmitPanelObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Solution Accepted";
        SubmitPanelObject.SetActive(true);
    }

    public void RemoveSubmitPanel()
    {
        SubmitPanelObject.SetActive(false);
    }
}