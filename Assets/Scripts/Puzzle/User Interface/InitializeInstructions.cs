using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeInstructions : MonoBehaviour
{

	private GameObject puzzleGenerator;
	private GameObject contentPanel;
	private List<OpCode> availableInstructions;

	public void Start()
	{
		puzzleGenerator = GameObject.Find("PuzzleGenerator");
		if (puzzleGenerator == null)
		{
			Debug.LogWarning("PuzzleGenerator not found");
		}

		contentPanel = transform.Find("Scroll View/Viewport/Content").gameObject;
		availableInstructions = puzzleGenerator.GetComponent<PuzzleGenerator>().GetFilteredInstructions();

		foreach (OpCode command in availableInstructions)
		{
			AddInstruction(command);
		}
	}

	private void AddInstruction(OpCode command)
	{
		var factory = FindObjectOfType<InstructionFactory>();
		GameObject instructionObj = factory.SpawnInstruction(command, contentPanel.transform);

		instructionObj.GetComponent<DragNDrop>().isClonable = true;
		instructionObj.GetComponent<Command>().Instruction = command;
		instructionObj.GetComponent<DragNDrop>().Initialize();
		instructionObj.transform.localScale = new Vector3(1,1,1);
	}
}
