﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CardContainerType {
    HAND,
    PLAY
}

public class CardContainer : MonoBehaviour, IDropHandler {
    public Vector3 containerScale = new Vector3(1.0f, 1.0f, 1.0f);

    public CardContainerType ContainerType;

    /**
     * Allows actor to query CardContainer for a card with the supplied address
     * @param address The integer value of the card's address
     * @return The API for the card who's hex address matches the int supplied
     */
    public CardLogic GetCard(int address) {
        if (ContainerType == CardContainerType.HAND) {
            return null;
        }

        return transform.Find(System.Convert.ToString(address, 16)).GetComponent<CardLogic>();
    }

    public void OnDrop(PointerEventData eventData) {
        Debug.Log(eventData.pointerDrag.name + " dropped on " + gameObject.name);

        var drag = eventData.pointerDrag.GetComponent<CardController>();
        if (drag != null) {
            drag.ActiveContainerTransform = transform;
            drag.transform.SetParent(transform);
            drag.DragTargetValid = true;
            drag.transform.localScale = containerScale;
        }
    }

    public void ResetCards() {
        foreach (Transform child in transform) {
            child.GetComponent<CardLogic>().ClearData();
        }
    }

}
