using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelInitializer : MonoBehaviour
{
    void serializeLevel1(string path)
    {
        using (StreamWriter stream = new StreamWriter(path))
        {
            PuzzleData puzzleLevel = new PuzzleData();
            puzzleLevel.InputStream = new int[] { 3, 1, 7, 9, 2 };
            puzzleLevel.Description = "Move the contents of the input box over to the output box in the " +
                "same order they appear in.";
            puzzleLevel.Instructions = new int[] { 0, 0, 1, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            string json = JsonUtility.ToJson(puzzleLevel);
            stream.Write(json);
        }
    }
    void serializeLevel2(string path)
    {
        using (StreamWriter stream = new StreamWriter(path))
        {
            PuzzleData puzzleLevel = new PuzzleData();
            puzzleLevel.InputStream = new int[] { 1, 2, 3, 4, 5 };
            puzzleLevel.Description = "Move the contents of the input box over to the output box in the " +
                "same order they appear in.";
            puzzleLevel.Instructions = new int[] { 0, 0, 1, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            string json = JsonUtility.ToJson(puzzleLevel);
            stream.Write(json);
        }
    }

    public void initLevel(int puzzleId, string path)
    {
        switch (puzzleId)
        {
            case 1:
                serializeLevel1(path);
                break;
            case 2:
                serializeLevel2(path);
                break;
            default:
                break;
        }
    }
}
