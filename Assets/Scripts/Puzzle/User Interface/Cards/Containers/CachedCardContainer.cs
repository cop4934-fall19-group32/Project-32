using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedCardContainer : CardContainer {
    public GameObject RegisterSlot;
    public GameObject StackSlot;
    public GameObject QueueSlot;
    public GameObject HeapSlot;

    /**
     * Allows actor to query CardContainer for a card with the supplied address
     * @param address The integer value of the card's address
     * @return The API for the card who's hex address matches the int supplied
     */
    public override CardLogic GetCard(int address) {
        // convert the address from int to hex representation so it can be found in the scene
        string s = "0x" + System.Convert.ToString(address, 16);
        if (RegisterSlot.transform.Find(s)) {
            return RegisterSlot.transform.Find(s).GetComponent<CardLogic>();
        }
        else if (StackSlot.transform.Find(s)) {
            return StackSlot.transform.Find(s).GetComponent<CardLogic>();
        }
        else if (QueueSlot.transform.Find(s)) {
            return QueueSlot.transform.Find(s).GetComponent<CardLogic>();
        }
        else if (HeapSlot.transform.Find(s)) {
            return HeapSlot.transform.Find(s).GetComponent<CardLogic>();
        }
        else {
            return null;
        }
    }

    public override void AddCard(GameObject card) {
        var cardLogic = card.GetComponent<CardLogic>();
        var dragController = card.GetComponent<CardDragBehavior>();
        if (cardLogic == null || dragController == null) {
            return;
        }

        switch (cardLogic.CardType) {
            case CardType.REGISTER:
                dragController.transform.SetParent(RegisterSlot.transform);
                break;
            case CardType.STACK:
                dragController.transform.SetParent(StackSlot.transform);
                break;
            case CardType.HEAP:
                dragController.transform.SetParent(HeapSlot.transform);
                break;
            case CardType.QUEUE:
                dragController.transform.SetParent(QueueSlot.transform);
                break;
        }

        card.transform.localPosition = new Vector3(0, 0, 0);
        card.transform.localRotation = Quaternion.identity;
        dragController.DragTargetValid = true;
        dragController.transform.localScale = containerScale;
    }

    public Vector3 GetWaypoint(CardType type) {
        switch (type) {
            case CardType.REGISTER:
                return RegisterSlot.transform.position;
            case CardType.STACK:
                return StackSlot.transform.position;
            case CardType.HEAP:
                return HeapSlot.transform.position;
            case CardType.QUEUE:
                return QueueSlot.transform.position;
            default:
                throw new System.ArgumentException("Card type not recognized");
        }
    }

    public override List<GameObject> GetCards() {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform slot in transform) {
            foreach (Transform child in slot) {
                children.Add(child.gameObject);
            }
        }

        return children;
    }
}
