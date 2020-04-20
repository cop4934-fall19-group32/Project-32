using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayedCardContainer : CardContainer
{
    /**
     * Allows actor to query CardContainer for a card with the supplied address
     * @param address The integer value of the card's address
     * @return The API for the card who's hex address matches the int supplied
     */
    public override CardLogic GetCard(int address) {
        // convert the address from int to hex representation so it can be found in the scene
        string s = "0x" + System.Convert.ToString(address, 16);
        return transform.Find(s).GetComponent<CardLogic>();
    }

    public override void AddCard(GameObject card) {
        var cardLogic = card.GetComponent<CardLogic>();
        var dragController = card.GetComponent<CardDragBehavior>();
        if (cardLogic == null || dragController == null) {
            return;
        }

        dragController.transform.SetParent(transform);
        dragController.DragTargetValid = true;
        dragController.transform.localScale = containerScale;
        dragController.transform.rotation = Quaternion.identity;
    }

    public override List<GameObject> GetCards() {
        List<GameObject> children = new List<GameObject>();
        
        foreach (Transform child in transform) {
            children.Add(child.gameObject);
        }

        return children;
    }

    public IEnumerator ReturnCards() {
        var cards = GetCards();
        var lastCard = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);

        foreach (var card in GetCards()) {
            StartCoroutine(card.GetComponent<CardDragBehavior>().FlyBack());
        }

        yield return StartCoroutine(lastCard.GetComponent<CardDragBehavior>().FlyBack());
    }
}
