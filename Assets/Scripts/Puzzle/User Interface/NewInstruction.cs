using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NewInstruction : MonoBehaviour
{

	private UIController UIController;
	private GameObject PuzzleGenerator;
	public GameObject obfuscator;
	private GameObject InstructionCache;
	private GameObject InstructionContentPanel;
	private GameObject NewInstructionsPanel;

	public OpCode command;
	private string MatchingInstruction;
	public bool revealed;
	public TMPro.TextMeshProUGUI instructionHeader;
	public TMPro.TextMeshProUGUI instructionDescription;

	void Awake()
	{
		UIController = FindObjectOfType<UIController>();
		PuzzleGenerator = GameObject.Find("PuzzleGenerator");
		InstructionCache = GameObject.FindWithTag("InstructionCache");
		InstructionContentPanel = InstructionCache.transform.Find("Scroll View/Viewport/Content").gameObject;
		NewInstructionsPanel = GameObject.Find("NewInstructionsPanel");
	}


	public void Initialize(OpCode op)
	{
		this.command = op;

		this.revealed = false;
		this.obfuscator.SetActive(true);

		transform.SetAsLastSibling();

		instructionHeader = transform.Find("Header").GetComponentInChildren<TMPro.TextMeshProUGUI>();
		instructionHeader.text = command.ToString();
		instructionDescription = transform.Find("Description").GetComponentInChildren<TMPro.TextMeshProUGUI>();
		instructionDescription.text = "Here is a description of the " + command.ToString() + " instruction...";

		MatchingInstruction = FindMatchingInstruction(command);

		GameObject instructionObj = PuzzleGenerator.GetComponent<InstructionFactory>().SpawnInstruction(this.command, instructionHeader.transform);

		// Get rid of unnecessary components
		Destroy(instructionObj.GetComponent<UIControl>());
		Destroy(instructionObj.GetComponent<GraphicRaycaster>());
		Destroy(instructionObj.GetComponent<Canvas>());

		instructionObj.GetComponent<RectTransform>().anchorMin = Vector2.zero;
		instructionObj.GetComponent<RectTransform>().anchorMax = Vector2.one;
		instructionObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
		instructionObj.GetComponent<RectTransform>().anchoredPosition = instructionHeader.GetComponent<RectTransform>().anchoredPosition;
	}


	// Triggered by pressing the obfuscator panel
	public void Reveal()
	{
		Debug.Log("Reveal Instruction");
		// When user clicks button, disable obfuscator and reveal the hidden description
		this.revealed = true;
		this.obfuscator.gameObject.SetActive(false);
	}

	// Given a new type of command, find an instruction object in the instruction cache that has the same OpCode and return its ElementName to be used by the UIController.
	private string FindMatchingInstruction(OpCode op)
	{
		int i = 0;
		foreach (Transform child in InstructionContentPanel.transform)
		{
			i++;
			if (child.GetComponent<Command>().Instruction == op)
			{
				Debug.Log("Instruction found for OpCode: [" + op.ToString() + "]");
				return child.GetComponent<ControllableUIElement>().ElementName;;
			}
		}
		Debug.LogError("Instruction not found for OpCode: [" + op.ToString() + "]. Searched " + i + " elements.");
		return null;
	}
}
