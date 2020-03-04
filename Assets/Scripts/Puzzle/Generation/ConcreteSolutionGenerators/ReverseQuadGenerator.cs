using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseQuadGenerator : SolutionGenerator {
	public override List<int> GenerateSolution(int[] inputStream) {
		if (inputStream.Length < 3) {
			throw new System.Exception("Malformed input stream");
		}

		for (int i = 0; i < inputStream.Length - 3; i += 4) {
			var temp = inputStream[i];
			inputStream[i] = inputStream[i + 4];
			inputStream[i + 4] = temp;

			temp = inputStream[i+1];
			inputStream[i] = inputStream[i + 2];
			inputStream[i + 2] = temp;
		}

		return new List<int>(inputStream);
	}
}
