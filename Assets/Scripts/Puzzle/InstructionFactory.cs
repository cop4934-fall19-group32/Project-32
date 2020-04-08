using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionFactory : MonoBehaviour
{
	public GameObject BasicInstructionPrefab;
	public GameObject JumpInstructionPrefab;
	public GameObject JumpAnchorPrefab;
	public GameObject CopyMoveInstructionPrefab;
	public GameObject CardJumpInstructionPrefab;
	public GameObject MathInstructionPrefab;

	public GameObject SpawnInstruction(OpCode instruction, Transform desiredParent) {
		switch (instruction) {
			case OpCode.INPUT:
			case OpCode.OUTPUT:
				return Instantiate(BasicInstructionPrefab, desiredParent);

			case OpCode.JUMP:
			case OpCode.JUMP_IF_NULL:
				return Instantiate(JumpInstructionPrefab, desiredParent);

			case OpCode.NO_OP:
				return Instantiate(JumpAnchorPrefab, desiredParent);

			case OpCode.MOVE_TO:
			case OpCode.MOVE_FROM:
			case OpCode.COPY_TO:
			case OpCode.COPY_FROM:
				return Instantiate(CopyMoveInstructionPrefab, desiredParent);

			case OpCode.JUMP_IF_GREATER:
			case OpCode.JUMP_IF_LESS:
				return Instantiate(CardJumpInstructionPrefab, desiredParent);

			case OpCode.ADD:
			case OpCode.SUBTRACT:
				return Instantiate(MathInstructionPrefab, desiredParent);

			default:
				throw new System.Exception("OpCode not recognized. Please fix your shit");
		}	
	}
}
