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
    public GameObject registerCard;
    public GameObject stackCard;
    public GameObject queueCard;
    public GameObject heapCard;

    private Transform LevelHolder;

    void DeserializePuzzleData(int puzzleId)
    {
        string path = "Assets/Resources/PuzzleSaves/puzzle" + puzzleId + ".json";
        using (StreamReader stream = new StreamReader(path))
        {
            string json = stream.ReadToEnd();
            this.puzzleData = JsonUtility.FromJson<PuzzleData>(json);
        }
    }
    void SerializePuzzleData(int puzzleId)
    {
        string path = "Assets/Resources/PuzzleSaves/puzzle" + puzzleId + ".json";
        using (StreamWriter stream = new StreamWriter(path))
        {
            string json = JsonUtility.ToJson(puzzleData);
            stream.Write(json);
        }
    }

    // This method is temporarily being used to initialize a sample scene containing:
    //     - 2 register cards
    //     - 2 stack cards
    //     - 2 queue cards
    //     - 2 heap cards
    // 
    // when this is updated to be dynamic, how will the number of each card be passed to the function?
    void CreateSampleLevel()
    {
        string path = "Assets/Resources/PuzzleSaves/puzzle1.json";
        using (StreamWriter stream = new StreamWriter(path))
        {
            PuzzleData samplePuzzle = new PuzzleData(2, 2, 2, 2);
            int x = samplePuzzle.NumHeapCards;
            string json = JsonUtility.ToJson(samplePuzzle);
            stream.Write(json);
        }
    }

    // This method instantiates different cards based on the numbers specified in the
    // puzzleData object.
    void instantiateCards()
    {
        for (int i = 0; i < puzzleData.NumRegisterCards; i++)
            Instantiate(registerCard, new Vector3(i+1, 1), Quaternion.identity);

        for (int i = 0; i < puzzleData.NumStackCards; i++)
            Instantiate(stackCard, new Vector3(i+1, 3), Quaternion.identity);

        for (int i = 0; i < puzzleData.NumQueueCards; i++)
            Instantiate(queueCard, new Vector3(i+1, 5), Quaternion.identity);

        for (int i = 0; i < puzzleData.NumQueueCards; i++)
            Instantiate(heapCard, new Vector3(i+1, 7), Quaternion.identity);
    }

    public void SetupBoard(int level)
    {
        CreateSampleLevel();
        DeserializePuzzleData(level);
        instantiateCards();
    }
}
