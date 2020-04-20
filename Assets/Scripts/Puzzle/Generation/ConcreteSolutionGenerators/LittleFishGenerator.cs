using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleFishGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>();

		var min = 1000;
		for (int i = 0; i < inputStream.Length; i++)
		{
			if (inputStream[i] < min)
				min = inputStream[i];
		}

		output.Add(min);

		return output;
	}

}
