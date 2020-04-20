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

		GameObject instructionObj = PuzzleGenerator.GetComponent<InstructionFactory>().SpawnInstruction(this.command, instructionHeader.transform);
		instructionDescription.text = instructionObj.GetComponent<Command>().Description;

		// Get rid of unnecessary components
		Destroy(instructionObj.GetComponent<UIControl>());
		Destroy(instructionObj.GetComponent<DragNDrop>());
		Destroy(instructionObj.GetComponent<GraphicRaycaster>());
		Destroy(instructionObj.GetComponent<Canvas>());

		instructionObj.GetComponent<RectTransform>().anchorMin = Vector2.zero;
		instructionObj.GetComponent<RectTransform>().anchorMax = Vector2.one;
		instructionObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
		instructionObj.GetComponent<RectTransform>().anchoredPosition = instructionHeader.GetComponent<RectTransform>().anchoredPosition;
		instructionObj.GetComponent<RectTransform>().localScale = new Vector3(1.75f, 1.75f, 1.75f);
	}


	// Triggered by pressing the obfuscator panel
	public void Reveal()
	{
		Debug.Log("Reveal Instruction");
		// When user clicks button, disable obfuscator and reveal the hidden description
		this.revealed = true;
		this.obfuscator.gameObject.SetActive(false);
	}

}
