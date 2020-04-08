using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardContainer : MonoBehaviour, IDropHandler {
    public Vector3 containerScale = new Vector3(1.0f, 1.0f, 1.0f);

    public int Count { get { return transform.childCount; } }

    /**
     * Allows actor to query CardContainer for a card with the supplied address
     * @param address The integer value of the card's address
     * @return The API for the card who's hex address matches the int supplied
     */
    abstract public CardLogic GetCard(int address);


    public void OnDrop(PointerEventData eventData) {
        Debug.Log(eventData.pointerDrag.name + " dropped on " + gameObject.name);

        AddCard(eventData.pointerDrag);
    }

    public void ResetCards() {
        foreach (Transform child in transform) {
            child.GetComponent<CardLogic>().ClearData();
        }
    }

    public abstract List<GameObject> GetCards();

    public abstract void AddCard(GameObject card);
}
