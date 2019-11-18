using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputOutputButtonHandler : MonoBehaviour
{
    public PuzzleGenerator puzzleGenerator;

    public void InputToOutput()
    {
        puzzleGenerator = GetComponent<PuzzleGenerator>();
        puzzleGenerator.InputToOutput();
    }
}
