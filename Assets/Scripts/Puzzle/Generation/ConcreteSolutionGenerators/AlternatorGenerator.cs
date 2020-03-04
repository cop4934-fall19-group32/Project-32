using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatorGenerator : SolutionGenerator {
	public override List<int> GenerateSolution(int[] inputStream) {
		List<int> solution = new List<int>();

		for (int i = 0; i < inputStream.Length; i++) {
			if (i % 2 == 0) {
				continue;
			}
			solution.Add(inputStream[i]);
		}

		return solution;
	}
}

