using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonHandler : MonoBehaviour
{
    public Sprite EmptySlotGraphic;
    public Sprite UsedSlotGraphic;
    public Sprite DeleteSlotGraphic;
    public GameObject DeleteButton;
    public Image SlotImage;

    [SerializeField]
    public GameObject SaveSelectMenu;
    public GameObject ConfirmationPanel;

    public bool SlotFilled { 
        get {
            return FindObjectOfType<PlayerState>().SlotHasSave(Slot);
        } 
    }

    private int Slot;
    private void Awake() {
        Slot = transform.GetSiblingIndex() + 1;

        if (!SlotFilled) {
            transform.GetComponentInChildren<Text>().text = "<<Empty>>";
            DeleteButton.SetActive(false);
            SlotImage.sprite = EmptySlotGraphic;
        }
        else {
            transform.GetComponentInChildren<Text>().text = "Slot #" + Slot;
            SlotImage.sprite = UsedSlotGraphic;
        }
    }

    public void HandleLoad(int slot) {
        var playerState = FindObjectOfType<PlayerState>();
        if (!playerState) {
            Debug.LogError("PlayerState not found. Load failed");
        }

        playerState.LoadGame(slot);
    }

    public void HandleDelete(int slot) {
        if (!SlotFilled) {
            return;
        }
        ConfirmationPanel.GetComponent<DeleteConfirmationPanel>().ShowDeleteConfirmationPanel(slot, gameObject);
        SaveSelectMenu.SetActive(false);
    }

    public void ConfirmedDelete(int slot) {
        var playerState = FindObjectOfType<PlayerState>();
        playerState.EraseSave(slot);
        SlotImage.sprite = EmptySlotGraphic;
        transform.GetComponentInChildren<Text>().text = "<<Empty>>";
        DeleteButton.SetActive(false);
    }
}
