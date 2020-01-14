using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputBox : MonoBehaviour
{
    public const int OutputBoxSize = 5;
    public OutputGeneration outputGeneration;
    public ArrayList outputContents;
    public ArrayList outputExpected;
    public GameObject DataCube;
    public AudioSource outputSound;

    public void InitializeOutput(int[] InputStream, int puzzleId)
    {
        outputContents = new ArrayList();
        outputGeneration = GetComponent<OutputGeneration>();
        outputExpected = outputGeneration.generateExpectedOutput(puzzleId, InputStream);
    }

    public void ResetOutput()
    {
        for (int i = 0; i < outputContents.Count; i++)
        {
            GameObject OutputDataCube = GameObject.Find("OutputNum" + i);
            Destroy(OutputDataCube.gameObject);
        }
        outputContents = new ArrayList();
    }

    public bool GradeOutput()
    {
        if (outputContents.Count != outputExpected.Count)
        {
            GetComponent<SubmitPanel>().ShowSubmitPanel(false);
            return false;
        }

        for (int i = 0; i < outputContents.Count; i++)
        {
            if ((int)outputContents[i] != (int)outputExpected[i])
            {
                GetComponent<SubmitPanel>().ShowSubmitPanel(false);
                return false;
            }
        }

        GetComponent<SubmitPanel>().ShowSubmitPanel(true);
        return true;
    }

    // This function takes in data from Computron, moves it into the correct position in 
    // the output box (bottom up), and updates the outputContents to accurately reflect the 
    // new state of the output box.
    // It returns:
    // - true for success
    // - false for runtime error
    public bool Output(int? numData)
    {
        ArrayList testExp = outputExpected;

        if (outputContents.Count >= outputExpected.Count)
            return false;

        if (numData != (int)outputExpected[outputContents.Count])
            return false;

        outputSound.Play();
        outputContents.Add(numData);

        var targetSlot = GameObject.Find("OutputSlot" + (outputContents.Count - 1));

        // Instantiate the correct data item and place it in the next open outbox slot.
        GameObject number = Instantiate(
            DataCube,
            targetSlot.transform.position,
            Quaternion.identity
        );
        

        number.name = "OutputNum" + (outputContents.Count - 1);
        number.GetComponentInChildren<TextMesh>().text = numData.ToString();
        return true;
    }
}
