using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMultiplierGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{

		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>();
		for (int i = 0; i < inputStream.Length; i++)
		{
			var quadra = inputStream[i] * 4;
			output.Add(quadra);
		}

		return output;
	}

}
