using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipFlopHopGenerator : SolutionGenerator {

	public override List<int> GenerateSolution(int[] inputStream) {
		if (inputStream.Length < 2) {
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>(inputStream);

		for (int i = 0; i < output.Count - 1; i+=2) {
			var temp = output[i];
			output[i] = output[i + 1];
			output[i + 1] = temp;

		}

		return output;
	}

}
