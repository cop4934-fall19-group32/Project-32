﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonHandler : MonoBehaviour
{
    private int Slot;
    private void Awake() {
        Slot = transform.GetSiblingIndex() + 1;
    }
    private void Update() {
        
        var playerState = FindObjectOfType<PlayerState>();
        if (!playerState.SlotHasSave(Slot)) {
            transform.GetComponentInChildren<Text>().text = "<<Empty>>";
        }
        else {
            transform.GetComponentInChildren<Text>().text = "Slot #" + Slot;
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
        var playerState = FindObjectOfType<PlayerState>();
        if (!playerState) {
            Debug.LogError("PlayerState not found. Delete failed");
        }

        playerState.EraseSave(slot);
    }
}
