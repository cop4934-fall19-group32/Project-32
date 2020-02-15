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
    private CardContainer registerContainer;

    public GameObject InputBox;
    public GameObject OutputBox;

    // variables for handling/displaying data values
    private Nullable<int> CurrentValue;
    private TextMesh Display;

    // variables for handling instructions
    private Command current;
    private Command previous;

    // used to handle coroutines
    private ACTOR_STATE currentState;


    public enum ACTOR_STATE
    {
        IDLE = 0,
        MOVING = 1,
        PROCESSING = 2,
        HALTED = 3
    }


    // at Start(), Actor is resting at the neutral waypoint
    public void Start()
    {
        // acquire Scene References
        commandStream = FindObjectOfType<Interpreter>();
        registerContainer = GameObject.FindWithTag("RegisterContainer").GetComponent<CardContainer>();
        InputBox = this.transform.parent.Find("Input").gameObject;
        OutputBox = this.transform.parent.Find("Output").gameObject;

        // initialize display
        Display = GetComponentsInChildren<TextMesh>()[0];
        Display.text = "";


        // initialize starting parameters
        transform.position = RestWaypoint.transform.position;
        current = null;
        previous = null;

        // kick off SM
        currentState = ACTOR_STATE.IDLE;
        StartCoroutine(ActorStateMachine());
    }

    public void Update() {

    }

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
            Display.text += ".";
            if (Display.text.Length > 3) 
            {
                Display.text = "";
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    // controls movement between waypoints
    IEnumerator MOVING()
    {
        while (currentState == ACTOR_STATE.MOVING)
        {
            var currentPosition = transform.position;
            var targetPosition = FindTargetWaypoint();
            var translationVector = targetPosition - currentPosition;

            UpdatePosition(currentPosition, targetPosition);

            if (translationVector.sqrMagnitude < 0.5)
            {
                currentState = ACTOR_STATE.PROCESSING;
            }

            yield return null;
        }
    }

    // simulates the current command being carried out, then calls the completion function
    IEnumerator PROCESSING() 
    {
        while (currentState == ACTOR_STATE.PROCESSING) 
        {
            FetchCommand();

            // need to ensure current position is accurate
            currentState = ACTOR_STATE.MOVING;
            yield return StartCoroutine(currentState.ToString());

            if (current == null) {
                Debug.Log("Processing aborted");
                break;
            }

            yield return StartCoroutine(current.Instruction.ToString());
        }
    }

    IEnumerator HALTED()
    {
        // depending on what the current OpCode is will dictate what message should be displayed
        if (current.Instruction == OpCode.OUTPUT)
        {
            if (CurrentValue != null)
            {
                Display.text = "INCORRECT DATA IN OUTPUT.";
                yield return new WaitForSeconds(1);
            }
            else
            {
                Display.text = "CANNOT PUT NULL INTO OUTPUT.";
                yield return new WaitForSeconds(1);
            }
        }

        while (currentState == ACTOR_STATE.HALTED)
        {
            yield return new WaitForSeconds(0.25f);    
        }
    }

    IEnumerator SUBMIT()
    {
        Display.text = "PROCESSING COMPLETE";
        yield return new WaitForSeconds(1);
        if (!OutputBox.GetComponent<OutputBox>().GradeOutput()) {
            CompleteExecution(new ExecutionReport(true));
        }

    }

    IEnumerator INPUT()
    {
        if (previous != null && previous.Instruction == OpCode.INPUT)
        {
            previous = null;
            yield return new WaitForSeconds(InstructionDelay);
        }

        CurrentValue = InputBox.GetComponent<InputBox>().Input();
        // allows Computron to "carry" the current data value
        if (CurrentValue != null)
        {
            Display.text = CurrentValue.ToString();
        }

        CompleteExecution(new ExecutionReport(false));
    }

    IEnumerator OUTPUT()
    {
        if (CurrentValue == null)
        {
            // Outputting a null value is a runtime error
            CompleteExecution(new ExecutionReport(true));
        }
        else if (!OutputBox.GetComponent<OutputBox>().Output(CurrentValue.Value))
        {
            //Outputted value was invalid
            CompleteExecution(new ExecutionReport(true));
            yield return null;
        }
        else
        {
            //Outputted value was valid
            CurrentValue = null;
            Display.text = "";
            CompleteExecution(new ExecutionReport(false));
        }
    }
    IEnumerator JUMP()
    {
        yield return new WaitForSeconds(InstructionDelay);
        CompleteExecution(new ExecutionReport(false));
    }

    IEnumerator JUMP_IF_NULL()
    {
        yield return new WaitForSeconds(InstructionDelay);
        CompleteExecution(new ExecutionReport(false, CurrentValue == null));
    }

    IEnumerator NO_OP() 
    {
        yield return new WaitForSeconds(InstructionDelay);
        CompleteExecution(new ExecutionReport(false));
    }

    // signal function for the Interpreter to let the Actor know to begin collecting commands
    public void BeginExecution()
    {
        currentState = ACTOR_STATE.PROCESSING;
    }


    public void AbortExecution() {
        //Move back to idle waypoint and clear instructions
        transform.position = RestWaypoint.transform.position;
        current = null;
        previous = null;

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


    // provides a smooth appearance of movement; is called every frame that requires movement
    private void UpdatePosition(Vector3 currentPosition, Vector3 targetPosition)
    {
        transform.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, MoveSpeed);
    }


    // depending on the current command being processed, the Actor may need to update its position
    // to one of the waypoints; otherwise, return the current position to show that no movement is necessary
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

        return transform.position;
    }

    // Allows the interpreter to halt execution, without resetting the simulation
    public void HaltSimulation()
    {
        currentState = ACTOR_STATE.HALTED;
    }

    // generates a report for the Interpreter and updates the Actor that processing has finished for that command
    private void CompleteExecution(ExecutionReport report)
    {
        commandStream.UpdateProgramCounter(report);
    }

}
