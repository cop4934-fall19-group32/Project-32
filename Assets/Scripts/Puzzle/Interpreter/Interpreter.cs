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
        Instructions = new List<Command>();
    }

    public void SubmitSolution()
    {
        if (Instructions.Count != 0) {
            return;
        }

        ProgramCounter = 0;
        Instructions = new List<Command>(SolutionPanel.GetComponentsInChildren<Command>());

        if (Instructions.Count < 1) {
            return;
        }

        Instructions.Add(GetComponent<Command>());
        Executor.BeginExecution();
    }

    public void HaltSimulation() {
        Instructions.Clear();
        ProgramCounter = 0;
        Executor.AbortExecution();
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
            StartCoroutine(HaltSimulationHelper());
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

    private IEnumerator HaltSimulationHelper() {
        FindObjectOfType<UIController>().HighlightUIElement("HaltButton");
        yield return new WaitForSeconds(2);
        FindObjectOfType<UIController>().StopHighlightUIElement("HaltButton");
    }
}
