using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TryAgainButtonHandler : MonoBehaviour
{
    private GameObject PuzzleGenerator;
    private GameObject PuzzleCacher;
    private GameObject HaltButton;

    public void TryAgain()
    {
        PuzzleGenerator = GameObject.Find("PuzzleGenerator");
        PuzzleCacher = GameObject.Find("PuzzleCacher");
        HaltButton = GameObject.Find("Halt Button").transform.Find("Button").gameObject;

        PuzzleGenerator.GetComponent<PuzzleGenerator>().ResetBoard();
        PuzzleCacher.GetComponent<SubmitPanel>().RemoveSubmitPanel();
        if (HaltButton == null)
        {
        	Debug.Log("Halt Button not found");
        }
        else
        {
            HaltButton.gameObject.GetComponent<Button>().onClick.Invoke();
        }
    }
}
