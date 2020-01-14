using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryAgainButtonHandler : MonoBehaviour
{
    public GameState PuzzleElements;
    public void TryAgain()
    {
        PuzzleElements.GetComponent<PuzzleGenerator>().ResetBoard();
        PuzzleElements.GetComponent<SubmitPanel>().RemoveSubmitPanel();
    }
}
