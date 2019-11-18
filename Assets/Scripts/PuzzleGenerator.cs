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
    public const int InputBoxSize = 5;
    public PuzzleData puzzleData;
    public GameObject registerCard;
    public GameObject stackCard;
    public GameObject queueCard;
    public GameObject heapCard;
    public ArrayList inputCurrent;
    public ArrayList outputCurrent;
    public GameObject[] numbers;

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

    // This method is temporarily being used to initialize a sample scene.
    // 
    // when this is updated to be dynamic, how will the number of each card be passed to the function?
    void CreateSampleLevel()
    {
        string path = "Assets/Resources/PuzzleSaves/puzzle1.json";
        using (StreamWriter stream = new StreamWriter(path))
        {
            PuzzleData samplePuzzle = new PuzzleData(
                new int[] { 3, 1, 7, 9, 2 },
                2, 2, 2, 2
            );
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

    void initializeInputStream()
    {
        for (int i = 0; i < puzzleData.InputStream.Length; i++)
        {
            GameObject number = (GameObject)Instantiate(numbers[puzzleData.InputStream[i]],
                GameObject.Find("InputSlot" + i).transform.position,
                Quaternion.identity);
            number.name = "Num" + i;
            inputCurrent.Add(puzzleData.InputStream[i]);
        }
    }

    public (GameObject, int) Input()
    {
        if (inputCurrent.Count == 0)
            return (null, 0);

        // Get the highest item from the input box and make it invisible.
        GameObject NumObject = GameObject.Find("Num" + (inputCurrent.Count - 1));
        NumObject.GetComponent<Renderer>().enabled = false;

        // Get the data the item represents and remove it from inputCurrent.
        int numData = (int)inputCurrent[inputCurrent.Count - 1];
        inputCurrent.RemoveAt(inputCurrent.Count - 1);

        // Move the rest of the input to the top of the input box.
        for (int i = 0; i < inputCurrent.Count; i++)
        {
            GameObject CurrentNumObject = GameObject.Find("Num" + (inputCurrent.Count - 1 - i));
            CurrentNumObject.transform.position = GameObject.Find("InputSlot" + (InputBoxSize - 1 - i)).transform.position;
        }

        return (NumObject, numData);
    }

    public void Output(GameObject NumObject, int numData)
    {
        if (NumObject == null)
            return;

        NumObject.transform.position = GameObject.Find("OutputSlot" + outputCurrent.Count).transform.position;
        NumObject.GetComponent<Renderer>().enabled = true;
        outputCurrent.Add(numData);
    }

    public void InputToOutput()
    {
        (GameObject NumObject, int numData) = Input();
        Output(NumObject, numData);
    }

    public void SetupBoard(int level)
    {
        CreateSampleLevel();
        DeserializePuzzleData(level);
        inputCurrent = new ArrayList();
        outputCurrent = new ArrayList();
        initializeInputStream();
    }
}
