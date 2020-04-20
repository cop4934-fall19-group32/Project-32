using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapFishGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var copy = new List<int>(inputStream);
		var output = new List<int>();

		copy.Sort();
		for (int i = inputStream.Length-1; i >= 0; i--)
		{
			output.Add(copy[i]);
		}

		return output;
	}

}
