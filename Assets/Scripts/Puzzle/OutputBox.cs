using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputBox : MonoBehaviour
{
    public const int OutputBoxSize = 5;
    public List<int> outputContents;
    public List<int> outputExpected;
    public GameObject DataCube;
    public GameObject[] OutputSlots = new GameObject[OutputBoxSize];
    public GameObject[] DataCubes = new GameObject[OutputBoxSize];
    private AudioCue OutputAudio;
    private AudioCue CorrectAudio;
    public AudioCue IncorrectAudio;
    public GameObject Actor;
    public GameObject PuzzleCacher;

    public void InitializeOutput(List<int> output)
    {
        outputContents = new List<int>();
        outputExpected = output;
        InitializeAudioCues();
    }

    private void InitializeAudioCues()
    {
        AudioCue[] audioCues = GetComponents<AudioCue>();
        foreach (AudioCue audioCue in audioCues)
        {
            if (audioCue.Name == "Output Audio")
            {
                OutputAudio = audioCue;
            }
            else if (audioCue.Name == "Correct Solution Audio")
            {
                CorrectAudio = audioCue;
            }
            else if (audioCue.Name == "Incorrect Solution Audio")
            {
                IncorrectAudio = audioCue;
            }
        }
    }

    public void ResetOutput()
    {
        for (int i = 0; i < OutputBoxSize; i++)
        {
            if (DataCubes[i] != null)
            {
                Destroy(DataCubes[i].gameObject);
                DataCubes[i] = null;
            }
        }
        outputContents = new List<int>();
    }

    public bool GradeOutput()
    {
        if (outputContents.Count != outputExpected.Count)
            return false;

        for (int i = 0; i < outputContents.Count; i++)
            if (outputContents[i] != outputExpected[i])
                return false;

        // Display Award Panel.
        CorrectAudio.Play();
        PuzzleCacher.GetComponent<PuzzleCacher>().ShowAwardPanel();
        return true;
    }

    public IEnumerator MoveDataCube(GameObject dataCube, Vector3 destination, float seconds)
    {
        float elapsedTime = 0f;
        Vector3 start = dataCube.transform.position;

        while (elapsedTime < seconds)
        {
            dataCube.transform.position = Vector3.Lerp(start, destination, elapsedTime / seconds);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        dataCube.transform.position = destination;
    }

    private void MoveDataCubes()
    {
        for (int i = OutputBoxSize - 1; i > 0; i--)
        {
            if (DataCubes[i - 1] == null)
            {
                continue;
            }

            DataCubes[i] = DataCubes[i - 1];
            StartCoroutine(MoveDataCube(DataCubes[i], OutputSlots[i].transform.position,
                Actor.GetComponent<Actor>().InstructionDelay));
        }
    }

    private void UpdateOutputBox(int numData)
    {
        GameObject bottomDataCube = DataCubes[OutputBoxSize - 1];

        MoveDataCubes();

        DataCubes[0] = (GameObject)Instantiate(DataCube,
            OutputSlots[0].transform.position,
            Quaternion.identity);

        DataCubes[0].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = numData.ToString();

        // Remove bottom data cube if another data cubed was moved in its place.
        if (bottomDataCube != null)
            Destroy(bottomDataCube.gameObject);
    }

    // This function takes in data from Computron, moves it into the correct position in 
    // the output box (bottom up), and updates the outputContents to accurately reflect the 
    // new state of the output box.
    // It returns:
    // - true for success
    // - false for runtime error
    public bool Output(int? numData)
    {
        if (outputContents.Count >= outputExpected.Count)
            return false;

        if (numData != outputExpected[outputContents.Count])
            return false;

        outputContents.Add((int)numData);
        UpdateOutputBox((int)numData);
        OutputAudio.Play();
        return true;
    }
}
