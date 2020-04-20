using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProgramCounter : MonoBehaviour
{
	[SerializeField]
	public GameObject indicatorPrefab;
	private GameObject interpreter;
	private int pc;
	private GameObject indicator;
	private GameObject pcMask;
	private GameObject contentPanel;
	private GameObject currentInstruction;
	private Coroutine pcRoutine;


	public void Awake()
	{
		pcRoutine = null;
		interpreter = GameObject.Find("Interpreter");
		pcMask = transform.Find("Scroll View/PC Mask").gameObject;
		contentPanel = transform.Find("Scroll View/Viewport/Content").gameObject;
		if (contentPanel == null)
		{
			Debug.LogWarning(gameObject.name + "/Scroll View/Viewport/Content not found.");
		}
	}

	public void BeginProgramCounter()
	{
		pcRoutine = StartCoroutine(UpdateProgramCounter());
	}

	public void SpawnPCIndicator()
	{
		// Create indicator object as a child of the current PC instruction
		currentInstruction = contentPanel.transform.GetChild(pc).gameObject;
		Vector3 pos = currentInstruction.transform.position;
		indicator = Instantiate(indicatorPrefab, pos, Quaternion.identity, pcMask.transform);
		Vector3 localPos = indicator.transform.localPosition;
		localPos.x = 15;
		indicator.transform.localPosition = localPos;
	}


	public void TerminateProgramCounter()
	{
		// Destroy PC indicator object
		if (pcRoutine != null)
		{
			StopCoroutine(pcRoutine);
		}
		if (this.indicator != null)
		{
			Destroy(this.indicator.gameObject);
		}
		currentInstruction = null;
	}


	IEnumerator UpdateProgramCounter ()
	{
		while (true)
		{
			pc = interpreter.GetComponent<Interpreter>().GetProgramCounter();
			if (contentPanel.transform.childCount < 1)
			{
				Debug.Log("Program counter can not be updated because the content panel has no children");
				break;
			}
			else if (pc >= contentPanel.transform.childCount)
			{
				pc = contentPanel.transform.childCount - 1;
			}
			currentInstruction = contentPanel.transform.GetChild(pc).gameObject;

			if(this.indicator == null)
			{
				SpawnPCIndicator();
			}

			Transform target = currentInstruction.gameObject.transform.Find("PC Target").transform;
			Vector3 targetPosition = currentInstruction.transform.position;
			targetPosition.x = 15;

			indicator.transform.position = Vector3.Lerp(indicator.transform.position, target.position, 7f * Time.deltaTime);
			yield return null;
		}
	}
}
