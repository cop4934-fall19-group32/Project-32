using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpreter : MonoBehaviour
{
    [SerializeField]
    Actor Executor;

    public GameObject SolutionPanel;

    /** Exposes simulation state to interested parties */
    public bool Running { get; private set; }

    /** Represents player solution in SolutionUI */
    private List<Command> Instructions;

    /** Represents pointer to next instruction to execute */
    private int ProgramCounter;

    // Start is called before the first frame update
    void Start()
    {
        Instructions = new List<Command>();
    }

    public void SubmitSolution()
    {
        // if the player hits play when they've already been stepping, ensure that stepping
        // is turned off and execute the rest of the solution
        if (Executor.step)
        {
            Executor.stepping = false;
            Executor.BeginProcessing();
        }

        if (Instructions.Count != 0) {
            return;
        }

        ProgramCounter = 0;
        Instructions = new List<Command>(SolutionPanel.GetComponentsInChildren<Command>());

        if (Instructions.Count < 1) {
            return;
        }

        Instructions.Add(GetComponent<Command>());
        Executor.BeginProcessing();
        Running = true;
    }

    public void SolutionStep()
    {
        // turn on the stepping boolean in the Actor script
        Executor.stepping = true;
        Executor.step = true;

        // if this is the first hit of the step button, build the solution behind the scenes
        if (Instructions.Count == 0)
        {
            ProgramCounter = 0;
            Instructions = new List<Command>(SolutionPanel.GetComponentsInChildren<Command>());

            if (Instructions.Count < 1)
            {
                return;
            }

            Instructions.Add(GetComponent<Command>());
        }

        // signals the Actor to grab the next instruction
        Executor.BeginProcessing();
    }

    public void HaltSimulation() {
        Instructions.Clear();
        ProgramCounter = 0;
        Executor.AbortExecution();
        Running = false;
    }

    /**
	 * Allows Actor to poll the interpreter for the next instruction to execute
	 * @return The next instruction to execute
	 */
    public Command PollInstruction()
    {
        return Instructions[ProgramCounter];
    }

    public int GetProgramCounter()
    {
        return ProgramCounter;
    }

    /**
	 *  Updates program counter based on current command
	 */
    public void UpdateProgramCounter(ExecutionReport report)
    {
        if (report.RuntimeErrorDetected == true) {
            StartCoroutine(HaltSimulationHelper());
        }

        var op = Instructions[ProgramCounter].Instruction;
        var jumpTarget = Instructions[ProgramCounter].Target;

        switch(op){
            case OpCode.JUMP:
                ProgramCounter = (int)jumpTarget;
                break;

            case OpCode.JUMP_IF_GREATER:
            case OpCode.JUMP_IF_LESS:
            case OpCode.JUMP_IF_NULL:
                if (report.ConditionalEvaluation == true) {
                    ProgramCounter = (int)jumpTarget;
                }
                else {
                    ProgramCounter++;
                }
                break;

            default:
                ProgramCounter++;
                break;
        }

    }

    public int GetInstructionCount() {
        return Instructions.Count - 1;
    }

    public float GetAverageRuntime() {
        int testCases = 10;


        return 0.0f;
    }

    private IEnumerator HaltSimulationHelper() {
        Running = false;
        FindObjectOfType<UIController>().HighlightUIElement("HaltButton");
        yield return new WaitForSeconds(2);
        FindObjectOfType<UIController>().StopHighlightUIElement("HaltButton");
    }
}
