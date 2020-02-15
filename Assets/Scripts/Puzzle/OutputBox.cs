using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputBox : MonoBehaviour
{
    public const int OutputBoxSize = 5;
    private OutputGeneration outputGeneration;
    public ArrayList outputContents;
    public ArrayList outputExpected;
    public GameObject DataCube;
    public AudioSource outputSound;

    public void InitializeOutput(int[] InputStream, string puzzleName)
    {
        outputContents = new ArrayList();
        outputGeneration = GetComponent<OutputGeneration>();
        outputExpected = outputGeneration.generateExpectedOutput(puzzleName, InputStream);
    }

    public void ResetOutput()
    {
        for (int i = 0; i < OutputBoxSize; i++)
        {
            GameObject OutputDataCube = GameObject.Find("OutputNum" + i);
            if (OutputDataCube != null)
            {
                Destroy(OutputDataCube.gameObject);
            }
        }
        outputContents = new ArrayList();
    }

    public bool GradeOutput()
    {
        if (outputContents.Count != outputExpected.Count)
        {
            return false;
        }

        for (int i = 0; i < outputContents.Count; i++)
        {
            if ((int)outputContents[i] != (int)outputExpected[i])
            {
                return false;
            }
        }

        GameObject PuzzleGenerator = GameObject.Find("PuzzleGenerator");
        PuzzleGenerator.GetComponent<SubmitPanel>().ShowSubmitPanel();
        PuzzleGenerator.GetComponent<PuzzleGenerator>().solved = true;
        return true;
    }

    void UpdateOutputBox()
    {
        int numDataCubes;

        if (outputContents.Count < OutputBoxSize)
        {
            numDataCubes = outputContents.Count;
        }
        else
        {
            numDataCubes = OutputBoxSize - 1;
            GameObject OutputDataCube = GameObject.Find("OutputNum" + (OutputBoxSize - 1));
            Destroy(OutputDataCube.gameObject);
        }

        for (int i = 0; i < numDataCubes; i++)
        {
            GameObject CurrentNumObject = GameObject.Find("OutputNum" + i);
            CurrentNumObject.transform.position = GameObject.Find("OutputSlot" + (i + 1)).transform.position;
            CurrentNumObject.name = "OutputNum" + (i + 1);
        }
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

        if (numData != (int)outputExpected[outputContents.Count])
            return false;

        outputSound.Play();

        UpdateOutputBox();
        outputContents.Add(numData);

        GameObject targetSlot = GameObject.Find("OutputSlot0");

        // Instantiate the correct data item and place it in the next open outbox slot.
        GameObject number = Instantiate(
            DataCube,
            targetSlot.transform.position,
            Quaternion.identity
        );

        number.name = "OutputNum0";
        number.GetComponentInChildren<TextMesh>().text = numData.ToString();
        return true;
    }
}
