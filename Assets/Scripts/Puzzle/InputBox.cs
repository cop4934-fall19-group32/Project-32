using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBox : MonoBehaviour
{
    public int Count { get { return inputContents.Count - NumRemoved; } }
    public const int InputBoxSize = 5;
    private int NumRemoved;
    public ArrayList inputContents;
    public GameObject DataCube;
    public GameObject[] InputSlots = new GameObject[InputBoxSize];
    public GameObject[] DataCubes = new GameObject[InputBoxSize];
    public GameObject Actor;

    public void ResetInput(int[] InputStream)
    {
        ClearInputBox();
        InitializeInput(InputStream);
    }

    public void InitializeInput(int[] InputStream)
    {
        inputContents = new ArrayList();
        inputContents.AddRange(InputStream);
        PopulateInputBox();
        NumRemoved = 0;
    }

    void ClearInputBox()
    {
        for (int i = 0; i < InputBoxSize; i++)
        {
            if (DataCubes[i] != null)
            {
                Destroy(DataCubes[i].gameObject);
                DataCubes[i] = null;
            }
        }
    }

    void PopulateInputBox()
    {
        int numDataCubes = (inputContents.Count < InputBoxSize) ? inputContents.Count : InputBoxSize;
        for (int i = 0; i < numDataCubes; i++)
        {
            DataCubes[i] = (GameObject)Instantiate(DataCube,
                InputSlots[i].transform.position,
                Quaternion.identity);

            DataCubes[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = inputContents[i].ToString();
        }
    }

    // This function removes the top most item from the input box, so long as there are items to spare.
    // It updates the inputContents to accurately reflect the new state of the input box and returns 
    // the numerical data associated with the removed item.
    public int? Input()
    {
        if (NumRemoved >= inputContents.Count)
            return null;

        GetComponent<AudioCue>().Play();

        GameObject toRemove = DataCubes[0];

        // Get the data the item represents and remove it from inputContents.
        int numData = (int)inputContents[NumRemoved];
        NumRemoved++;
        Destroy(toRemove.gameObject);

        // Slide the data cubes up.
        int numSlides = 0;
        for (int i = 0; i < InputBoxSize - 1; i++)
        {
            if (i + NumRemoved >= inputContents.Count)
            {
                break;
            }

            DataCubes[i] = DataCubes[i + 1];
            StartCoroutine(MoveDataCube(DataCubes[i], InputSlots[i].transform.position,
                Actor.GetComponent<Actor>().InstructionDelay));
            numSlides++;
        }

        // If there is still input, instantiate a new data cube in at the last input box slot.
        if (NumRemoved + (InputBoxSize - 1) < inputContents.Count)
        {
            DataCubes[InputBoxSize - 1] = (GameObject)Instantiate(DataCube,
                InputSlots[InputBoxSize - 1].transform.position,
                Quaternion.identity);

            DataCubes[InputBoxSize - 1].GetComponentInChildren<TMPro.TextMeshProUGUI>().text =
                inputContents[NumRemoved + InputBoxSize - 1].ToString();
        }
        else
        {
            // Set the rest of the data cube array to null.
            for (int i = numSlides; i < InputBoxSize; i++)
            {
                DataCubes[i] = null;
            }
        }

        return numData;
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
}
