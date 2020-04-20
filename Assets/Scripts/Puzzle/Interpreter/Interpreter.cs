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
    public bool Halted { get; private set; }

    public int TestCases = 10;

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
        Halted = false;
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

        // if the next instruction is the last one, 
        if (Executor.stepCount+1 == GetInstructionCount())
        {
            // we may need to implicitly submit their solution after this command
            Executor.implicitSubmit = true;
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
            Halted = true;
            FindObjectOfType<UIController>().HighlightUIElement("HaltButton");
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
            case OpCode.JUMP_IF_EQUAL:
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
        CASVM virtualMachine = new CASVM();
        
        //Feed vm player instructions
        virtualMachine.Instructions = Instructions;
        
        var puzzleData = FindObjectOfType<GameState>().SelectedPuzzle;
        
        //Feed vm input setttings
        virtualMachine.Inputs = puzzleData.InputStream;
        if (puzzleData.GenerateRandomInput) {
            virtualMachine.GenerateRandomInputs = true;
        }

        //Feed vm cards
        if (FindObjectOfType<PlayedCardContainer>() != null) { 
            var playedCards = FindObjectOfType<PlayedCardContainer>().GetComponentsInChildren<CardLogic>();
            foreach (var card in playedCards) {
                virtualMachine.MemoryCards.Add(card.Address, card.datastructure);
            }
        }

        //Feed vm output generator
        virtualMachine.OutputGenerator = puzzleData.OutputGenerator;

        int cycles = 0;

        for (int i = 0; i < TestCases; i++) {
            var cyclesForRun = virtualMachine.Run();
            if (cyclesForRun == null) {
                return 999999;
            }
            cycles += (int)cyclesForRun;
        }


        return ((float)cycles) / TestCases;
    }
}
