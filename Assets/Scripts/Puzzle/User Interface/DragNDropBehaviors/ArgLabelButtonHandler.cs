using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArgLabelButtonHandler : MonoBehaviour
{
	public GameObject instruction; 
	public void TriggerRelink() {
		if (instruction.GetComponent<CardCommandDragNDrop>().BoundCard == null) {
			return;
		}

		var linker = gameObject.AddComponent<CardInstructionLinker>();
		linker.Instruction = instruction;
	}
}
