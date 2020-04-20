using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigFishGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>();

		var max = 0;
		for (int i = 0; i < inputStream.Length; i++)
		{
			if (inputStream[i] > max)
				max = inputStream[i];
		}

		output.Add(max);

		return output;
	}

}
