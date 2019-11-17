using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter : MonoBehaviour
{

	public GameObject SolutionPanel;

	public enum OpCode { 
		RESET,
		TERMINATE,
		SUBMIT,
		INPUT,
		OUTPUT,
		JUMP,
		JUMP_IF_NULL,
		JUMP_IF_LESS,
		JUMP_IF_GREATER,
		MOVE_TO,
		COPY_TO,
		MOVE_FROM,
		COPY_FROM,
		ADD,
		SUBTRACT		
	}

	/**
	 * @struct Command
	 * @brief Command is a simple structure to bind all of the information
	 *		  needed for the Actor to execute an instruction 
	 */
	public struct Command {
		public OpCode Op { get; }
		public int Arg { get; }
		public float SimulationSpeed { get; }

		public Command(OpCode op, int arg = -1, int speed = 1) {
			Op = op;
			Arg = arg;
			SimulationSpeed = speed;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		Instructions = new List<Command>();
		Instructions.Add(new Command(OpCode.INPUT));
		Instructions.Add(new Command(OpCode.OUTPUT));
		ProgramCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
		
    }

	public void SubmitSolution(List<Command> instructions) 
	{
		Instructions = instructions;
	}

	/**
	 * Allows Actor to poll the interpreter for the next instruction to execute
	 * @return The next instruction to execute
	 */
	public Command PollInstruction() 
	{
		var command = Instructions[ProgramCounter++];
		UpdateProgramCounter(command);
		return command;
	}

	/**
	 *  Updates program counter based on current command
	 */
	private void UpdateProgramCounter(Command c) 
	{
		if (/*isSomeSortOfJump(c.Op)*/ false) {
			//ProgramCounter = c.Arg;	
		}
		else {
			ProgramCounter++;
			ProgramCounter %= Instructions.Count;
		}
	}

	/** Represents player solution in SolutionUI */
	private List<Command> Instructions;

	/** Represents pointer to next instruction to execute */
	private int ProgramCounter;
}
