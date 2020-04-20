using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ALUGenerator : SolutionGenerator
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
			var doubler = inputStream[i] * 2;
			output.Add(doubler);
		}

		return output;
	}

}
