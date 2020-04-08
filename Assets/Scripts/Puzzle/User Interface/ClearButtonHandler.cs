using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButtonHandler : MonoBehaviour
{
    public PlayedCardContainer PlayArea;

    public void ResetCards() {
        var cards = PlayArea.GetCards();

        foreach (var card in cards) {
            StartCoroutine(card.GetComponent<CardDragBehavior>().FlyBack());
        }
    }
}
