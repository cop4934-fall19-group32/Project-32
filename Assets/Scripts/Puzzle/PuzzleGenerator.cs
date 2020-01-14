using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// The BoardGenerator class is used to fill the puzzle board scene with
/// data that is specific to a puzzle's intial state or it's saved state.
/// </summary>
public class PuzzleGenerator : MonoBehaviour
{
    public PuzzleData puzzleData;
    private Transform LevelHolder;
    public const string levelFilesPath = "Assets/Resources/PuzzleSaves/";

    void DeserializePuzzleData(string path)
    {
        using (StreamReader stream = new StreamReader(path))
        {
            string json = stream.ReadToEnd();
            this.puzzleData = JsonUtility.FromJson<PuzzleData>(json);
        }
    }
    void SerializePuzzleData(string path)
    {
        using (StreamWriter stream = new StreamWriter(path))
        {
            string json = JsonUtility.ToJson(puzzleData);
            stream.Write(json);
        }
    }

    public void ResetBoard()
    {
        GetComponent<InputBox>().ResetInput(puzzleData.InputStream);
        GetComponent<OutputBox>().ResetOutput();
        GameObject.Find("Computron").GetComponent<Actor>().Start();
    }

    public void SetupBoard(int puzzleId)
    {
        string path = levelFilesPath + "puzzle" + puzzleId + ".json";

        if (!File.Exists(path))
            GetComponent<LevelInitializer>().initLevel(puzzleId, path);

        DeserializePuzzleData(path);
        GetComponent<InputBox>().InitializeInput(puzzleData.InputStream);
        GetComponent<OutputBox>().InitializeOutput(puzzleData.InputStream, puzzleId);
    }
}
