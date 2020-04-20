using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComparatronGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>();

		for (int i = 0; i < inputStream.Length - 1; i += 2)
		{
			var temp1 = inputStream[i];
			var temp2 = inputStream[i + 1];
			if (temp1 > temp2)
				output.Add(temp1);
			else
				output.Add(temp2);
		}

		return output;
	}

}
