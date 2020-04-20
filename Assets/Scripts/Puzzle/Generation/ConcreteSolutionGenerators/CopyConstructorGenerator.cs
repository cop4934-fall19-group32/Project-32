using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyConstructorGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		
		var output = new List<int>();
		output.Add(inputStream[0]);
		output.Add(inputStream[0]);

		return output;
	}

}
