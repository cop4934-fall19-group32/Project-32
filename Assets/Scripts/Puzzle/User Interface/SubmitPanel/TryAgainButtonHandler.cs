using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryAgainButtonHandler : MonoBehaviour
{
    private GameObject PuzzleGenerator;
    public void TryAgain()
    {
        PuzzleGenerator = GameObject.Find("PuzzleGenerator");
        PuzzleGenerator.GetComponent<PuzzleGenerator>().ResetBoard();
        PuzzleGenerator.GetComponent<SubmitPanel>().RemoveSubmitPanel();
    }
}
