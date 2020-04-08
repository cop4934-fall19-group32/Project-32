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
        for (int i = 0; i < InputBoxSize; i++)
        {
            GameObject InputDataCube = GameObject.Find("InputNum" + i);
            if (InputDataCube != null)
            {
                Destroy(InputDataCube.gameObject);
            }
        }
    }

    void PopulateInputBox()
    {
        int numDataCubes = (inputContents.Count < InputBoxSize) ? inputContents.Count : InputBoxSize;
        for (int i = 0; i < numDataCubes; i++)
        {
            GameObject number = (GameObject)Instantiate(DataCube,
                GameObject.Find("InputSlot" + i).transform.position,
                Quaternion.identity);

            number.name = "InputNum" + i;
            number.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = inputContents[i].ToString();
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
        GameObject NumObject = GameObject.Find("InputNum0");

        // Get the data the item represents and remove it from inputContents.
        int numData = (int)inputContents[0];
        inputContents.RemoveAt(0);
        Destroy(NumObject.gameObject);

        ClearInputBox();
        PopulateInputBox();

        return numData;
    }
}
