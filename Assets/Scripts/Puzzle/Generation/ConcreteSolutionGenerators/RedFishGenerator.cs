using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedFishGenerator : SolutionGenerator
{

	public override List<int> GenerateSolution(int[] inputStream)
	{
		if (inputStream.Length < 2)
		{
			throw new System.Exception("Malformed input stream");
		}

		var output = new List<int>();

		var max1 = 0;
		var max2 = 0;
		var max3 = 0;
		var temp1 = 0;
		var temp2 = 0;
		for (int i = 0; i < inputStream.Length; i++)
		{
			if (inputStream[i] > max1)
			{
				temp1 = max1;
				max1 = inputStream[i];
				if (temp1 > max2)
				{
					temp2 = max2;
					max2 = temp1;
					if (temp2 > max3)
					{
						max3 = temp2;
					}
				}
			}
			else if (inputStream[i] > max2)
			{
				temp1 = max2;
				max2 = inputStream[i];
				if (temp1 > max3)
					max3 = temp1;
			}
			else if (inputStream[i] > max3)
				max3 = inputStream[i];
		}

		output.Add(max1);
		output.Add(max2);
		output.Add(max3);

		return output;
	}

}
