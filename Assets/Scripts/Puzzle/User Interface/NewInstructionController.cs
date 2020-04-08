using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewInstructionController : MonoBehaviour
{
	private UIController UIController;
	private GameObject PuzzleGenerator;
	public GameObject newInstructionsPanel;
	public GameObject newInstructionPrefab;
	private GameObject newInstructionContainer;
	private List<GameObject> newInstructionObjects;
	private GameObject continueButton;
	private bool finished;

	void Awake()
	{
		finished = false;
		UIController = FindObjectOfType<UIController>();
		PuzzleGenerator = GameObject.Find("PuzzleGenerator");
		newInstructionContainer = newInstructionsPanel.transform.Find("NewInstructionContainer").gameObject;
		continueButton = newInstructionsPanel.transform.Find("ContinueButton").gameObject;
		continueButton.SetActive(false);
		newInstructionObjects = new List<GameObject>();
		newInstructionsPanel.SetActive(false);
	}


	public IEnumerator IntroduceNewInstructions(List<OpCode> newInstructions)
	{
		Debug.Log("new instruction count: " + newInstructions.Count);
		if (newInstructions.Count < 1)
		{
			Debug.Log("No new instructions detected. Stopping coroutine...");
			yield break;
		}

		foreach (Transform child in newInstructionContainer.transform)
		{
			Destroy(child.gameObject);
		}

		newInstructionsPanel.SetActive(true);
		UIController.Blur();
		finished = false;

		foreach (OpCode newOp in newInstructions)
		{
			Debug.Log("New Instruction: " + newOp);

			// Create new instruction card and initialize it
			GameObject newInstructionObj = Instantiate(newInstructionPrefab, newInstructionContainer.transform);
			newInstructionObj.gameObject.GetComponent<NewInstruction>().Initialize(newOp);
			newInstructionObjects.Add(newInstructionObj);
		}

		// Wait for all new instructions to be clicked.
		while (!CheckIfDone())
		{
			yield return null;
		}

		continueButton.SetActive(true);

		// Wait for the player to press the continue button.
		while (!finished)
		{
			yield return null;
		}

		yield return null;
	}


	// Check if all new instructions have been reviewed (clicked)
	private bool CheckIfDone()
	{
		foreach(GameObject instruction in newInstructionObjects)
		{
			if (instruction.GetComponent<NewInstruction>().revealed == false)
			{
				return false;
			}
		}
		return true;
	}


	// After the continue button is pressed, clear instruction awarding and move on to tutorial or normal gameplay.
	public void Finish()
	{
		foreach(GameObject newInstructionObj in newInstructionObjects)
		{
			Destroy(newInstructionObj.gameObject);
		}
		newInstructionObjects = new List<GameObject>();
		continueButton.SetActive(false);
		newInstructionsPanel.SetActive(false);
		UIController.ClearFocus();
		finished = true;
	}
}
