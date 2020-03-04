using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseTriplesGenerator : SolutionGenerator {
	public override List<int> GenerateSolution(int[] inputStream) {
		if (inputStream.Length < 3) {
			throw new System.Exception("Malformed input stream");
		}

		List<int> output = new List<int>(inputStream);

		for (int i = 0; i < output.Count - 2; i += 3) {
			var temp = inputStream[i];
			output[i] = output[i + 2];
			output[i + 2] = temp;

		}

		return new List<int>(output);
	}
}
