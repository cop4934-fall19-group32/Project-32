using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    [SerializeField]
    Actor Executor;

    public GameObject SolutionPanel;

    /** Represents player solution in SolutionUI */
    private List<Command> Instructions;

    /** Represents pointer to next instruction to execute */
    private int ProgramCounter;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SubmitSolution()
    {
        ProgramCounter = 0;
        Instructions = new List<Command>(SolutionPanel.GetComponentsInChildren<Command>());

        if (Instructions.Count < 1) {
            return;
        }

        Instructions.Add(GetComponent<Command>());
        Executor.BeginExecution();
    }

    /**
	 * Allows Actor to poll the interpreter for the next instruction to execute
	 * @return The next instruction to execute
	 */
    public Command PollInstruction()
    {
        return Instructions[ProgramCounter];
    }

    /**
	 *  Updates program counter based on current command
	 */
    public void UpdateProgramCounter(ExecutionReport report)
    {
        if (report.RuntimeErrorDetected == true) {
            Executor.HaltSimulation();  
        }
        var op = Instructions[ProgramCounter].Instruction;
        var arg = Instructions[ProgramCounter].Arg;

        if (op == OpCode.JUMP) {
            ProgramCounter = (int)Instructions[ProgramCounter].Arg;
        }
        else if(op == OpCode.JUMP_IF_NULL)
        {
            if (report.ConditionalEvaluation == true) {
                ProgramCounter = (int)arg;
            }
            else { 
                ProgramCounter++;
            }
        }
        else {
            ProgramCounter++;
        }
    }
}
