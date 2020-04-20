using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotFishGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>();

		var pivot = inputStream[0];
		var greater = new List<int>();

		for (int i = 1; i < inputStream.Length; i++)
		{
			if (inputStream[i] < pivot)
				output.Add(inputStream[i]);
			else
				greater.Add(inputStream[i]);
		}

		output.Add(pivot);

		for (int i = 0; i < greater.Count; i++)
		{
			output.Add(greater[i]);
		}

		return output;
	}

}
