using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    Transform RestWaypoint;
    [SerializeField]
    Transform InputWaypoint;
    [SerializeField]
    Transform OutputWaypoint;
    [SerializeField]
    Transform RegisterWaypoint;

    [SerializeField]
    Interpreter commandStream;

    [SerializeField]
    GameState PuzzleElements;

    [SerializeField]
    float MoveSpeed = 2f;
    private Vector3 velocity = Vector3.zero;

    // flag to control when the Actor begins executing commands
    private bool running;
    // flag to control the Actor handling one command at a time
    private bool processing;

    private Nullable<int> CurrentValue;

    private Command current;
    private TextMesh Display;

    // at Start(), Actor is resting at the neutral waypoint and no commands are ready to be processed
    public void Start()
    {
        running = false;
        processing = false;
        transform.position = RestWaypoint.transform.position;

        Display = GetComponentsInChildren<TextMesh>()[0];
        Display.text = "";
    }

    // signal function for the Interpreter to let the Actor know to begin collecting commands
    public void BeginExecution()
    {
        running = true;
    }

    // signal function for the Interpreter to let the Actor know to stop processing
    public void HaltSimulation()
    {
        running = false;
        processing = false;
        // depending on what the current OpCode is will dictate what message should be displayed
        if (current.Instruction == OpCode.OUTPUT)
        {
            if (CurrentValue != null)
            {
                Display.text = "INCORRECT DATA IN OUTPUT.";
                PuzzleElements.GetComponent<OutputBox>().GradeOutput();
            }
            else
            {
                Display.text = "CANNOT PUT NULL INTO OUTPUT.";
            }
        }
    }

    // will only be accessed if the Actor is ready for the next command
    private void FetchCommand()
    {
        current = commandStream.PollInstruction();
    }

    void Update()
    {
        // only update at each frame if the simulation is currently running
        if (!running) 
        {
            return;
        }

        // if no command is currently processing, fetch the next command
        if (!processing)
        {
            FetchCommand();
            processing = true;
        }

        var currentPosition = transform.position;
        var targetPosition = FindTargetWaypoint();
        var translationVector = targetPosition - currentPosition;

        // either need to update the current position OR we are at the proper location and need to execute
        if (translationVector.sqrMagnitude > 0.5) 
        {
            UpdatePosition(currentPosition, targetPosition);
        }
        else 
        {
            Execute();
        }
    }

    // simulates the current command being carried out, then calls the completion function
    private void Execute() 
    {
        if (current.Instruction == OpCode.NO_OP)
        {
            ;
        }
        else if (current.Instruction == OpCode.INPUT) 
        {
            CurrentValue = PuzzleElements.GetComponent<InputBox>().Input();
            // allows Computron to "carry" the current data value
            if (CurrentValue != null) 
            { 
                Display.text = CurrentValue.ToString();
            }

            CompleteExecution(new ExecutionReport(false));
        }
        else if (current.Instruction == OpCode.OUTPUT) 
        {
            // will cause a runtime error
            if (CurrentValue == null) 
            {
                CompleteExecution(new ExecutionReport(true));
                return;
            }

            // otherwise, drop off current value being held
            // Output() returns 0 for success and 2 for output mismatch
            bool check = PuzzleElements.GetComponent<OutputBox>().Output(CurrentValue.Value);
            if (!check)
            {
                CompleteExecution(new ExecutionReport(true));
                return;
            }
            else
            {
                CurrentValue = null;
                Display.text = "";
                CompleteExecution(new ExecutionReport(false));
            }
        }   
        else if (current.Instruction == OpCode.JUMP) 
        {
            CompleteExecution(new ExecutionReport(false));
        }
        else if (current.Instruction == OpCode.JUMP_IF_NULL) 
        {
            CompleteExecution(new ExecutionReport(false, CurrentValue == null));
        }
        else if (current.Instruction == OpCode.SUBMIT) 
        {
            // turn off all control switches and finalize execution
            running = false;
            processing = false;
            Display.text = "PROCESSING COMPLETE";
            PuzzleElements.GetComponent<OutputBox>().GradeOutput();
        }
    }

    // generates a report for the Interpreter and updates the Actor that processing has finished for that command
    private void CompleteExecution(ExecutionReport report) 
    {
        processing = false;
        commandStream.UpdateProgramCounter(report);
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
}
