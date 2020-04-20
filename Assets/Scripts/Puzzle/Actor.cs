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

    [Header("Display Settings")]
    [SerializeField]
    private GameObject DataCube;
    [SerializeField]
    private TMPro.TextMeshProUGUI DataCubeText;
    
    [SerializeField]
    private GameObject Display;
    [SerializeField]
    private TMPro.TextMeshProUGUI DisplayText;


    // variables for handling instructions
    private Command current;
    private Command previous;

    // used for building the error reports at the end of each instruction
    private Boolean error;
    private Nullable<Boolean> conditionalResult;

    // used when Computron is stepping through solutions
    public Boolean stepping;    // currently stepping
    public Boolean step;        // was step used at all 
    private GameObject StepButton;
    public Boolean implicitSubmit;
    public int stepCount;

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
        implicitSubmit = false;
        stepCount = 0;

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
            yield return new WaitForSeconds(0.25f);
        }
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

            // stop within a certain distance and move to processing state
            if (translationVector.sqrMagnitude < 0.5) {
                currentState = ACTOR_STATE.PROCESSING;
            }
            else { 
                UpdatePosition(currentPosition, targetPosition);
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
            if (!(current.Instruction == OpCode.JUMP) && !(current.Instruction == OpCode.JUMP_IF_NULL) && 
                !(current.Instruction == OpCode.NO_OP) && !(current.Instruction == OpCode.SUBMIT))
            {
                currentState = ACTOR_STATE.MOVING;
                yield return StartCoroutine(currentState.ToString());
            }

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
            stepCount++;

            // if we have a Jump or Conditional Jump command, we need to adjust the stepCount accordingly
            if (current.Instruction == OpCode.JUMP || conditionalResult == true)
            {
                // update stepCount
                stepCount = (int)current.Target;

                // if this puts us on the second to last command, we need to implicitly submit the solution
                if (stepCount+1 == commandStream.GetInstructionCount())
                {
                    implicitSubmit = true;
                }
                // otherwise, assert false
                else
                {
                    implicitSubmit = false;
                }
            }

            // moves to the SUBMIT command without requiring the player to click step again
            if (implicitSubmit)
            {
                BeginProcessing();
            }

            // wait for the player to click step again
            stepping = false;
            StepButton.GetComponent<UIControl>().Enable();
            while (currentState == ACTOR_STATE.REPORTING)
            {
                yield return null;
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
            yield return null;
        }
        // else, continue processing
        currentState = ACTOR_STATE.PROCESSING;
    }

    // if an error is encountered, inform the player what caused it 
    IEnumerator HALTED()
    {
        // depending on what the current OpCode is will dictate what message should be displayed
        // all error codes are self explanatory
        OutputBox.GetComponent<OutputBox>().IncorrectAudio.Play();

        if (current.Instruction == OpCode.OUTPUT)
        {
            if (CurrentValue == null)
            {
                DisplayMessage("CANNOT PUT NULL INTO OUTPUT.");
                yield return new WaitForSeconds(1);
            }
            else
            {
                DisplayMessage("INCORRECT DATA IN OUTPUT.");
                yield return new WaitForSeconds(1);
            }
        }
        
        if (current.Instruction == OpCode.MOVE_TO || current.Instruction == OpCode.COPY_TO)
        {
            DisplayMessage("CANNOT STORE A NULL VALUE.");
            yield return new WaitForSeconds(1);
        }
        else if (current.Instruction == OpCode.JUMP_IF_LESS || current.Instruction == OpCode.JUMP_IF_GREATER ||
            current.Instruction == OpCode.JUMP_IF_EQUAL)
        {
            DisplayMessage("CANNOT COMPARE NULL.");
            yield return new WaitForSeconds(1);
        }
        else if (current.Instruction == OpCode.ADD)
        {
            DisplayMessage("CANNOT ADD NULL.");
            yield return new WaitForSeconds(1);
        }
        else if (current.Instruction == OpCode.SUBTRACT)
        {
            DisplayMessage("CANNOT SUBTRACT NULL.");
            yield return new WaitForSeconds(1);
        }
        else if (current.Instruction == OpCode.SUBMIT)
        {
            DisplayMessage("OUTPUT IS INCORRECT.");
            yield return new WaitForSeconds(1);
        }


        // hold in the halted state until the player halts the simulation (calls AbortExecution)
        while (currentState == ACTOR_STATE.HALTED)
        {
            yield return new WaitForSeconds(0.25f);
        }
    }

    // final instruction of every solution
    IEnumerator SUBMIT()
    {
        DisplayMessage(".");
        yield return new WaitForSeconds(0.5f);
        DisplayMessage("..");
        yield return new WaitForSeconds(0.5f);
        DisplayMessage("...");
        yield return new WaitForSeconds(InstructionDelay);
        HideTextBox();
        // check the solution
        bool res = OutputBox.GetComponent<OutputBox>().GradeOutput();
        if (!res) {
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
        ShowDataCube(CurrentValue);

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
        HideDataCube();
        CurrentValue = null;
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

    // if current value is equal to value at destination, jump; else, continue sequential execution
    IEnumerator JUMP_IF_EQUAL()
    {
        // grab card reference
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

        yield return new WaitForSeconds(InstructionDelay);

        conditionalResult = (CurrentValue == SecondValue);

        // show the two values being added together
        DisplayMessage(CurrentValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " == ");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + SecondValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " ?");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(conditionalResult.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        ShowDataCube(CurrentValue);
        HideTextBox();

        currentState = ACTOR_STATE.REPORTING;
    }

    // if current value is less than value at destination, jump; else, continue sequential execution
    IEnumerator JUMP_IF_LESS()
    {
        // grab card reference
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

        yield return new WaitForSeconds(InstructionDelay);

        conditionalResult = (CurrentValue < SecondValue);

        // show the two values being compared
        DisplayMessage(CurrentValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " < ");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + SecondValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " ?");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(conditionalResult.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        ShowDataCube(CurrentValue);
        HideTextBox();

        currentState = ACTOR_STATE.REPORTING;
    }

    // if current value is greater than value at destination, jump; else, continue sequential execution
    IEnumerator JUMP_IF_GREATER()
    {
        // grab card reference
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

        yield return new WaitForSeconds(InstructionDelay);

        conditionalResult = (CurrentValue > SecondValue);

        // show the two values being compared
        DisplayMessage(CurrentValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " > ");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + SecondValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " ?");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(conditionalResult.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        ShowDataCube(CurrentValue);
        HideTextBox();
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
        HideDataCube();

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
        ShowDataCube(CurrentValue);

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
        CurrentValue = (int)card.CopyFrom();
        ShowDataCube(CurrentValue);

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
        DisplayMessage(CurrentValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " + ");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + SecondValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " = ");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " ? ");
        yield return new WaitForSeconds(InstructionDelay);
        CurrentValue += SecondValue;
        ShowDataCube(CurrentValue);
        HideTextBox();

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
        DisplayMessage(CurrentValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " - ");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + SecondValue.ToString());
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " = ");
        yield return new WaitForSeconds(InstructionDelay);
        DisplayMessage(DisplayText.text + " ? ");
        yield return new WaitForSeconds(InstructionDelay);
        CurrentValue -= SecondValue;
        ShowDataCube(CurrentValue);
        HideTextBox();

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
        implicitSubmit = false;
        stepCount = 0;

        MoveSpeed = 0.3f;
        InstructionDelay = 0.3f;

        // halt all processes and start a new instance of the state machine
        StopAllCoroutines();
        currentState = ACTOR_STATE.IDLE;
        HideTextBox();
        HideDataCube();
        StartCoroutine(ActorStateMachine());
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
                 current.Instruction == OpCode.ADD || current.Instruction == OpCode.SUBTRACT ||
                 current.Instruction == OpCode.JUMP_IF_EQUAL || current.Instruction == OpCode.JUMP_IF_GREATER ||
                 current.Instruction == OpCode.JUMP_IF_LESS)
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

    public void ShowDataCube(int? num) {
        DataCube.SetActive(true);
        if (num == null) {
            DataCubeText.text = "NULL";
        }
        else {
            DataCubeText.text = num.ToString();
        }

    }

    private void HideDataCube() {
        DataCube.SetActive(false);
    }

    private void DisplayMessage(string text) {
        Display.SetActive(true);
        DisplayText.text = text;
    }

    private void HideTextBox() {
        Display.SetActive(false);
    }
}
