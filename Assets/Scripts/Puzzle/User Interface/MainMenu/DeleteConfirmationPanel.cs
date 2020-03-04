using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteConfirmationPanel : MonoBehaviour
{
    public GameObject DeleteConfirmationObject;
    public GameObject currentHandler;
    public int val;
    public void ShowDeleteConfirmationPanel(int slot, GameObject handler)
    {
        DeleteConfirmationObject.SetActive(true);
        val = slot;
        currentHandler = handler;
    }

    public void ConfirmButton()
    {
        currentHandler.GetComponent<SaveButtonHandler>().ConfirmedDelete(val);
        RemoveSubmitPanel();
    }

    public void CancelButton()
    {
        RemoveSubmitPanel();
    }

    public void RemoveSubmitPanel()
    {
        DeleteConfirmationObject.SetActive(false);
    }
}
