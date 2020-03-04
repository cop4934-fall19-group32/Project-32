using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputGeneration : MonoBehaviour
{
    ArrayList GenerateOutput_InsAndOuts1(int[] inputStream)
    {
        ArrayList output = new ArrayList();
        for (int i = 0; i < inputStream.Length; i++)
            output.Add(inputStream[i]);
        return output;
    }
    ArrayList GenerateOutput_InsAndOuts2(int[] inputStream)
    {
        ArrayList output = new ArrayList();
        for (int i = 0; i < inputStream.Length; i++)
            output.Add(inputStream[i]);
        return output;
    }
    ArrayList GenerateOutput_Alternator(int[] inputStream)
    {
        ArrayList output = new ArrayList();
        for (int i = 1; i < inputStream.Length; i+=2)
            output.Add(inputStream[i]);
        return output;
    }

    public ArrayList generateExpectedOutput(string puzzleName, int[] inputStream)
    {
        ArrayList output = null;
        switch (puzzleName)
        {
            case "InsAndOuts1":
                output = GenerateOutput_InsAndOuts1(inputStream);
                break;
            case "InsAndOuts2":
                output = GenerateOutput_InsAndOuts2(inputStream);
                break;
            case "Alternator":
                output = GenerateOutput_Alternator(inputStream);
                break;
            default:
                break;
        }

        return output;
    }
}