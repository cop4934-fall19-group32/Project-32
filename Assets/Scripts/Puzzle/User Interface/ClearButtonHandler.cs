using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearButtonHandler : MonoBehaviour
{
    public PlayedCardContainer PlayArea;

    public void ResetCards() {
        StartCoroutine(PlayArea.ReturnCards());        
    }
}
