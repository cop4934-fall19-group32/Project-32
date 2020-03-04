using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SolutionGenerator : MonoBehaviour
{
	public abstract List<int> GenerateSolution(int[] inputStream);
}
