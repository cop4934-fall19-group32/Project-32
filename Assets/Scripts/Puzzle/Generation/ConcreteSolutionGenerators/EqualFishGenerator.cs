using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EqualFishGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>();

		var num = inputStream[0];
		output.Add(num);
		for (int i = 1; i < inputStream.Length; i++)
		{
			if (inputStream[i] == num)
				output.Add(inputStream[i]);
		}

		return output;
	}

}
