using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct InstructionFactoryEntry {
	public OpCode Instruction;
	public GameObject Prefab;
}


public class InstructionFactory : MonoBehaviour
{
	/**
	 * Used to construct factoryEntries map at runtime becuase Dictionaries are not serializable
	 */
	public InstructionFactoryEntry[] InstructionLibrary;

	/** Maps OpCode to Prefab to spawn instructions at runtime */
	private Dictionary<OpCode, GameObject> FactoryEntries;
	
	protected void Awake() {
		
	}

	public GameObject SpawnInstruction(OpCode instruction, Transform desiredParent) {
		//Avoid static initialization issues by lazily generating entries from library
		if (FactoryEntries == null) {
			FactoryEntries = new Dictionary<OpCode, GameObject>();

			foreach (var entry in InstructionLibrary) {
				FactoryEntries.Add(entry.Instruction, entry.Prefab);
			}
		}

		if (!FactoryEntries.ContainsKey(instruction)) {
			Debug.LogError("InstructionFactory received unrecognized opcode.");
			throw new System.Exception("Add the fucking opcode");
		}

		return Instantiate(FactoryEntries[instruction], desiredParent);
	}
}
