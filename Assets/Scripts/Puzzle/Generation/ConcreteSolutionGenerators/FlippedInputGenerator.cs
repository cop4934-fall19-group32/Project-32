using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlippedInputGenerator : SolutionGenerator {
	public override List<int> GenerateSolution(int[] inputStream) {
		int[] flippedInput = (int[])inputStream.Clone();
		System.Array.Reverse(flippedInput);
		return new List<int>(flippedInput);
	}








}
