using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseFishGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>(inputStream);
		output.Sort();

		return output;
	}

}
