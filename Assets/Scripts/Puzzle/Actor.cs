using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    // waypoints for Actor to move to
    [Header("Waypoints")]
    public Transform RestWaypoint;
    public Transform InputWaypoint;
    public Transform OutputWaypoint;
    public Transform RegisterWaypoint;

    // variables for movement
    [Header("Movement Controls")]
    public float MoveSpeed;
    public float InstructionDelay;
    private Vector3 velocity = Vector3.zero;

    // components that Actor needs to interface with
    private Interpreter commandStream;
    private CardContainer cardContainer;

    public GameObject InputBox;
    public GameObject OutputBox;

    // variables for handling/displaying data values
    private Nullable<int> CurrentValue;
    private Nullable<int> SecondValue;
    private Nullable<uint> CurrentArg;
    private TextMesh Display;

    // variables for handling instructions
    private Command current;
    private Command previous;

    // used for building the error reports at the end of each instruction
    private Boolean error;
    private Nullable<Boolean> conditionalResult;

    // used when Computron is stepping through solutions
    public Boolean stepping;    // currently stepping
    public Boolean step;        // step used at all 
    private GameObject StepButton;

    // used to handle coroutines
    private ACTOR_STATE currentState;

    public enum ACTOR_STATE
    {
        IDLE,
        MOVING,
        PROCESSING,
        REPORTING,
        HALTED
    }

    // initial state of Computron
    public void Start()
    {
        // acquire Scene References
        commandStream = FindObjectOfType<Interpreter>();
        cardContainer = GameObject.FindWithTag("RegisterContainer").GetComponent<CardContainer>();
        InputBox = this.transform.parent.Find("Input").gameObject;
        OutputBox = this.transform.parent.Find("Output").gameObject;
        StepButton = GameObject.FindWithTag("Step");

        // initialize display
        Display = GetComponentsInChildren<TextMesh>()[0];
        Display.text = "";

        // initialize starting parameters
        transform.position = RestWaypoint.transform.position;
        current = null;
        previous = null;

        // kick off the state machine
        currentState = ACTOR_STATE.IDLE;
        error = false;
        conditionalResult = null;
        CurrentValue = null;
        SecondValue = null;
        stepping = false;
        step = false;
        StartCoroutine(ActorStateMachine());
    }

    //*** BEGINNING OF STATE MACHINE ***//

    IEnumerator ActorStateMachine()
    {
        while (true)
        {
            yield return StartCoroutine(currentState.ToString());
        }
    }

    // default state; Computron is waiting for instructions
    IEnumerator IDLE() 
    {
        while (currentState == ACTOR_STATE.IDLE) 
        {
            // idle "animation"
            Display.text += ".";
            if (Display.text.Length > 3) 
            {
                Display.text = "";
            }

            yield return new WaitForSeconds(0.25f);
        }

        // reset text displayed to empty string when exiting the IDLE state
        Display.text = "";
    }

    // controls movement between waypoints
    IEnumerator MOVING()
    {
        while (currentState == ACTOR_STATE.MOVING)
        {
            // initialize values needed for movement
            var currentPosition = transform.position;
            var targetPosition = FindTargetWaypoint();
            var translationVector = targetPosition - currentPosition;

            UpdatePosition(currentPosition, targetPosition);

            // stop within a certain distance and move to processing state
            if (translationVector.sqrMagnitude < 0.5)
            {
                currentState = ACTOR_STATE.PROCESSING;
            }

            yield return null;
        }
    }

    // simulates the current command being carried out
    IEnumerator PROCESSING() 
    {
        while (currentState == ACTOR_STATE.PROCESSING) 
        {
            // grab the next instruction
            FetchCommand();

            // need to ensure current position is accurate
            currentState = ACTOR_STATE.MOVING;
            yield return StartCoroutine(currentState.ToString());

            if (current == null) {
                Debug.Log("Processing aborted");
                break;
            }

            // execute the instruction
            yield return StartCoroutine(current.Instruction.ToString());
        }
    }

    // generates a report for the instruction that was executed
    IEnumerator REPORTING()
    {
        if (error)
        {
            // halt the simulation if an error was encountered in processing
            CompleteExecution(new ExecutionReport(error, conditionalResult));
            currentState = ACTOR_STATE.HALTED;
            yield return StartCoroutine(currentState.ToString());
        }

        // if the player is stepping through a solution, we need to hold here
        while (stepping)
        {
            stepping = false;
            StepButton.GetComponent<UIControl>().Enable();
            while (currentState == ACTOR_STATE.REPORTING)
            {
                yield return new WaitForSeconds(0.25f);
            }
            break;
        }

        // generate a report based on the current values
        CompleteExecution(new ExecutionReport(error, conditionalResult));

        // reset values
        conditionalResult = null;
        error = false;

        // if the simulation is complete
        while (current.Instruction == OpCode.SUBMIT)
        {
            Display.text = "CONGRATULATIONS!";
            yield return new WaitForSeconds(1);
        }
        // else, continue processing
        currentState = ACTOR_STATE.PROCESSING;
    }

    // if an error is encountered, inform the player what caused it 
    IEnumerator HALTED()
    {
        // depending on what the current OpCode is will dictate what message should be displayed
        // all error codes are self explanatory

        if (current.Instruction == OpCode.OUTPUT)
        {
            if (CurrentValue == null)
            {
                Display.text = "CANNOT PUT NULL INTO OUTPUT.";
                yield return new WaitForSeconds(1);
            }
            else
            {
                Display.text = "INCORRECT DATA IN OUTPUT.";
                yield return new WaitForSeconds(1);
            }
        }
        
        if (current.Instruction == OpCode.MOVE_TO || current.Instruction == OpCode.COPY_TO)
        {
            Display.text = "CANNOT STORE A NULL VALUE.";
            yield return new WaitForSeconds(1);
        }
        
        if (current.Instruction == OpCode.JUMP_IF_LESS || current.Instruction == OpCode.JUMP_IF_GREATER)
        {
            Display.text = "CANNOT COMPARE NULL.";
            yield return new WaitForSeconds(1);
        }

        if (current.Instruction == OpCode.ADD)
        {
            Display.text = "CANNOT ADD NULL.";
            yield return new WaitForSeconds(1);
        }

        if (current.Instruction == OpCode.SUBTRACT)
        {
            Display.text = "CANNOT SUBTRACT NULL.";
            yield return new WaitForSeconds(1);
        }

        if (current.Instruction == OpCode.SUBMIT)
        {
            Display.text = "OUTPUT IS INCORRECT.";
            yield return new WaitForSeconds(1);
        }


        // hold in the halted state until the player halts the simulation (calls AbortExecution)
        while (currentState == ACTOR_STATE.HALTED)
        {
            yield return new WaitForSeconds(InstructionDelay);
        }
    }

    // final instruction of every solution
    IEnumerator SUBMIT()
    {
        Display.text = "PROCESSING COMPLETE.";
        yield return new WaitForSeconds(1);
        
        // check the solution
        Boolean res = OutputBox.GetComponent<OutputBox>().GradeOutput();
        if (!res)
        {
            error = true;
        }

        currentState = ACTOR_STATE.REPORTING;
    }

    // pull a value from Input
    IEnumerator INPUT()
    {
        // insert a pause if grabbing consecutive inputs so the action is visible to the player
        if (previous != null && previous.Instruction == OpCode.INPUT)
        {
            yield return new WaitForSeconds(InstructionDelay);
        }

        // Computron picks up the top item from Input
        CurrentValue = InputBox.GetComponent<InputBox>().Input();
        Display.text = CurrentValue.ToString();
        if (CurrentValue == null)
        {
            Display.text = "NULL";
        }

        currentState = ACTOR_STATE.REPORTING;
    }

    // place the currently held data in Output
    IEnumerator OUTPUT()
    {
        // detect possible errors first
        if (CurrentValue == null)
        {
            // cannot output null
            error = true;
            currentState = ACTOR_STATE.REPORTING;
            yield return StartCoroutine(currentState.ToString());
        }
        else if (!OutputBox.GetComponent<OutputBox>().Output(CurrentValue.Value))
        {
            // current value is not correct for this solution
            error = true;
            currentState = ACTOR_STATE.REPORTING;
            yield return StartCoroutine(currentState.ToString());
        }

        // current value is correct
        CurrentValue = null;
        Display.text = "";
        yield return new WaitForSeconds(InstructionDelay);
        currentState = ACTOR_STATE.REPORTING;
    }

    // unconditional jump; nothing to check
    IEnumerator JUMP()
    {
        yield return new WaitForSeconds(InstructionDelay);
        currentState = ACTOR_STATE.REPORTING;
    }

    // if current value is null, jump; else, continue sequential execution
    IEnumerator JUMP_IF_NULL()
    {
        yield return new WaitForSeconds(InstructionDelay);
        conditionalResult = (CurrentValue == null);
        currentState = ACTOR_STATE.REPORTING;
    }

    // if current value is less than value at destination, jump; else, continue sequential execution
    IEnumerator JUMP_IF_LESS()
    {
        // grab card reference
        CurrentArg = current.Arg;
        CardLogic card = cardContainer.GetCard((int)CurrentArg);

        // ensure neither value is null
        if (CurrentValue == null || card.datastructure.Peek() == null)
        {
            error = true;
            currentState = ACTOR_STATE.REPORTING;
            yield return StartCoroutine(currentState.ToString());
        }

        yield return new WaitForSeconds(InstructionDelay);
        conditionalResult = (CurrentValue < card.datastructure.Peek());
        currentState = ACTOR_STATE.REPORTING;
    }

    // if current value is greater than value at destination, jump; else, continue sequential execution
    IEnumerator JUMP_IF_GREATER()
    {
        // grab card reference
        CurrentArg = current.Arg;
        CardLogic card = cardContainer.GetCard((int)CurrentArg);

        // ensure neither value is null
        if (CurrentValue == null || card.datastructure.Peek() == null)
        {
            error = true;
            currentState = ACTOR_STATE.REPORTING;
            yield return StartCoroutine(currentState.ToString());
        }

        yield return new WaitForSeconds(InstructionDelay);
        conditionalResult = (CurrentValue > card.datastructure.Peek());
        currentState = ACTOR_STATE.REPORTING;
    }

    // used only as a landing spot for jump instructions
    IEnumerator NO_OP() 
    {
        yield return new WaitForSeconds(InstructionDelay);
        currentState = ACTOR_STATE.REPORTING;
    }

    // place the currently held value in the specified Card
    IEnumerator MOVE_TO()
    {
        // grab card reference
        CurrentArg = current.Arg;
        CardLogic card = cardContainer.GetCard((int)CurrentArg);

        // trying to store null causes a runtime error
        if (CurrentValue == null)
        {
            error = true;
            currentState = ACTOR_STATE.REPORTING;
            yield return StartCoroutine(currentState.ToString());
        }
        else
        {
            // put down the value held, and hands are now empty
            card.MoveTo((int)CurrentValue);
            CurrentValue = null;
            Display.text = "NULL";
            yield return new WaitForSeconds(InstructionDelay);
        }

        currentState = ACTOR_STATE.REPORTING;
    }

    // pull the top value from the specified Card
    IEnumerator MOVE_FROM()
    {
        // grab card reference
        CurrentArg = current.Arg;
        CardLogic card = cardContainer.GetCard((int)CurrentArg);

        // insert a pause if grabbing consecutive inputs so the action is visible to the player
        if (previous != null && previous.Instruction == OpCode.MOVE_FROM)
        {
            yield return new WaitForSeconds(InstructionDelay);
        }

        // Computron picks up the top item from the specified Card
        CurrentValue = card.MoveFrom();
        if (CurrentValue != null)
        {
            Display.text = CurrentValue.ToString();
        }
        else
        {
            // if the Card is empty, pick up NULL
            Display.text = "NULL";
        }

        currentState = ACTOR_STATE.REPORTING;
    }

    // place a copy of the currently held value in the specified Card
    // Computron still keeps the held value
    IEnumerator COPY_TO()
    {
        // get card reference
        CurrentArg = current.Arg;
        CardLogic card = cardContainer.GetCard((int)CurrentArg);

        // trying to store null causes a runtime error
        if (CurrentValue == null)
        {
            error = true;
            currentState = ACTOR_STATE.REPORTING;
            yield return StartCoroutine(currentState.ToString());
        }
        else
        {
            // put down a copy of the value held
            card.CopyTo((int)CurrentValue);
            yield return new WaitForSeconds(InstructionDelay);
        }

        currentState = ACTOR_STATE.REPORTING;
    }

    // get a copy of the top value from the specified Card
    // the Card still keeps the value
    IEnumerator COPY_FROM()
    {
        // get card reference
        CurrentArg = current.Arg;
        CardLogic card = cardContainer.GetCard((int)CurrentArg);

        // insert a pause if grabbing consecutive inputs so the action is visible to the player
        if (previous != null && previous.Instruction == OpCode.COPY_FROM)
        {
            yield return new WaitForSeconds(InstructionDelay);
        }

        // Computron gets a copy of the top item from the specified Card
        CurrentValue = card.CopyFrom();
        if (CurrentValue != null)
        {
            Display.text = CurrentValue.ToString();
        }
        else
        {
            // if the Card is empty, pick up NULL
            Display.text = "NULL";
        }

        currentState = ACTOR_STATE.REPORTING;
    }

    // add a copy of a value from the specified Card to the currently held value
    IEnumerator ADD()
    {
        // get card reference
        CurrentArg = current.Arg;
        CardLogic card = cardContainer.GetCard((int)CurrentArg);

        // Computron gets a copy of the top item from the specified Card
        SecondValue = card.CopyFrom();

        // ensure neither value is null
        if (CurrentValue == null || SecondValue == null)
        {
            error = true;
            currentState = ACTOR_STATE.REPORTING;
            yield return StartCoroutine(currentState.ToString());
        }

        // show the two values being added together
        Display.text += " + ";
        yield return new WaitForSeconds(InstructionDelay);
        Display.text += SecondValue.ToString();
        yield return new WaitForSeconds(InstructionDelay);
        CurrentValue += SecondValue;
        SecondValue = null;
        Display.text = CurrentValue.ToString();
        yield return new WaitForSeconds(InstructionDelay);

        currentState = ACTOR_STATE.REPORTING;
    }

    // subtract a copy of a value from the specified Card from the currently held value
    IEnumerator SUBTRACT()
    {
        // get card reference
        CurrentArg = current.Arg;
        CardLogic card = cardContainer.GetCard((int)CurrentArg);

        // Computron gets a copy of the top item from the specified Card
        SecondValue = card.CopyFrom();

        // ensure neither value is null
        if (CurrentValue == null || SecondValue == null)
        {
            error = true;
            currentState = ACTOR_STATE.REPORTING;
            yield return StartCoroutine(currentState.ToString());
        }

        // show the two values being subtracted
        Display.text += " - ";
        yield return new WaitForSeconds(InstructionDelay);
        Display.text += SecondValue.ToString();
        yield return new WaitForSeconds(InstructionDelay);
        CurrentValue -= SecondValue;
        SecondValue = null;
        Display.text = CurrentValue.ToString();
        yield return new WaitForSeconds(InstructionDelay);

        currentState = ACTOR_STATE.REPORTING;
    }

    //*** END OF STATE MACHINE ***//

    //*** INTERPRETER INTERACTIONS ***//

    // signal function for the Interpreter to let the Actor know to begin collecting commands
    public void BeginProcessing()
    {
        currentState = ACTOR_STATE.PROCESSING;
    }

    // called via Interpreter when the player hits the Halt button
    public void AbortExecution() {
        // snap back to idle waypoint and reset all state machine values
        transform.position = RestWaypoint.transform.position;
        current = null;
        previous = null;
        stepping = false;
        CurrentValue = null;
        SecondValue = null;
        conditionalResult = null;
        error = false;

        // halt all processes and start a new instance of the state machine
        StopAllCoroutines();
        currentState = ACTOR_STATE.IDLE;
        StartCoroutine(ActorStateMachine());
        Display.text = "";
    }

    // will only be accessed if the Actor is ready for the next command
    private void FetchCommand()
    {
        previous = current;
        current = commandStream.PollInstruction();
    }

    // generates a report for the Interpreter and updates the Actor that processing has finished for that command
    private void CompleteExecution(ExecutionReport report)
    {
        commandStream.UpdateProgramCounter(report);
    }

    //*** MOVEMENT FUNCTIONS ***//

    // provides a smooth appearance of movement; is called every frame that requires movement
    private void UpdatePosition(Vector3 currentPosition, Vector3 targetPosition)
    {
        transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, MoveSpeed);
    }

    // depending on the current command being processed, the Actor may need to update its position to move
    // towards one of the waypoints; otherwise, return the current position to show that no movement is necessary
    private Vector3 FindTargetWaypoint()
    {
        if (current.Instruction == OpCode.INPUT)
        {
            return InputWaypoint.transform.position;
        }
        else if (current.Instruction == OpCode.OUTPUT)
        {
            return OutputWaypoint.transform.position;
        }
        else if (current.Instruction == OpCode.MOVE_TO || current.Instruction == OpCode.MOVE_FROM ||
                 current.Instruction == OpCode.COPY_TO || current.Instruction == OpCode.COPY_FROM ||
                 current.Instruction == OpCode.ADD || current.Instruction == OpCode.SUBTRACT)
        {
            // retrieve the waypoint of the current card
            CurrentArg = current.Arg;
            CardLogic card = cardContainer.GetCard((int)CurrentArg);
            var waypoint = card.GetWaypoint();
            waypoint.z = transform.position.z;
            return waypoint;
        }

        return transform.position;
    }
}
