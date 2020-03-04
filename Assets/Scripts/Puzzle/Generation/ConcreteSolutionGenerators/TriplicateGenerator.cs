using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriplicateGenerator : SolutionGenerator {
	public override List<int> GenerateSolution(int[] inputStream) {
		List<int> solution = new List<int>();

		for (int i = 2; i < inputStream.Length; i+=3) {
			solution.Add(inputStream[i]);
		}

		return solution;
	}
}
