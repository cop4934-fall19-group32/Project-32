using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputGeneration : MonoBehaviour
{
    ArrayList generateOutput1(int[] inputStream)
    {
        ArrayList output = new ArrayList();
        for (int i = 0; i < inputStream.Length; i++)
            output.Add(inputStream[i]);
        return output;
    }
    ArrayList generateOutput2(int[] inputStream)
    {
        ArrayList output = new ArrayList();
        for (int i = 0; i < inputStream.Length; i++)
            output.Add(inputStream[i]);
        return output;
    }

    public ArrayList generateExpectedOutput(int puzzleId, int[] inputStream)
    {
        ArrayList output = null;
        switch (puzzleId)
        {
            case 1:
                output = generateOutput1(inputStream);
                break;
            default:
                output = generateOutput2(inputStream);
                break;
        }

        return output;
    }
}