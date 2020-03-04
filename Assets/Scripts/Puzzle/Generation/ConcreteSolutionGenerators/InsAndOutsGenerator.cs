using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsAndOutsGenerator : SolutionGenerator
{
	public override List<int> GenerateSolution(int[] inputStream) {
		return new List<int>(inputStream);
	}
}
