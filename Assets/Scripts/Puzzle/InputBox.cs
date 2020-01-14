using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBox : MonoBehaviour
{
    public const int InputBoxSize = 5;
    public ArrayList inputContents;
    public GameObject DataCube;
    public AudioSource inputSound;

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
    }

    void ClearInputBox()
    {
        for (int i = InputBoxSize - inputContents.Count; i < InputBoxSize; i++)
        {
            GameObject InputDataCube = GameObject.Find("Num" + i);
            Destroy(InputDataCube.gameObject);
        }
    }

    void PopulateInputBox()
    {
        for (int i = 0; i < inputContents.Count; i++)
        {
            GameObject number = (GameObject)Instantiate(DataCube,
                GameObject.Find("InputSlot" + i).transform.position,
                Quaternion.identity);

            number.name = "Num" + i;
            number.GetComponentInChildren<TextMesh>().text = inputContents[i].ToString();
        }
    }

    // This function removes the top most item from the input box, so long as there are items to spare.
    // It updates the inputContents to accurately reflect the new state of the input box and returns 
    // the numerical data associated with the removed item.
    public int? Input()
    {
        if (inputContents.Count == 0)
            return null;

        inputSound.Play();

        // Get the item from the top of the input box.
        GameObject NumObject = GameObject.Find("Num" + (InputBoxSize - inputContents.Count));

        // Get the data the item represents and remove it from inputContents.
        int numData = (int)inputContents[0];
        inputContents.RemoveAt(0);
        Destroy(NumObject.gameObject);

        // Move the rest of the input box contents up one input slot.
        for (int i = 0; i < inputContents.Count; i++)
        {
            GameObject CurrentNumObject = GameObject.Find("Num" + (InputBoxSize - inputContents.Count + i));
            CurrentNumObject.transform.position = GameObject.Find("InputSlot" + i).transform.position;
        }

        return numData;
    }
}
