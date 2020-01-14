using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class ExecutionReport
 * @brief An execution report is the datastructure sent back to the Interpreter
 *        after the Actor recieves and processes the current instruction. This
 *        is necessary to inform the Interpreter about the validity of the instruction
 *        as well as the result of any conditional expression contained in the instruction
 *        (to properly evaluate conditional jumps)
 */
public class ExecutionReport
{
    /** Whether or not the command that generated this report was valid */
    public bool RuntimeErrorDetected { get; }

    /** 
	 * A nullable boolean flag that represents the actor's evaluation of the
	 * conditional expression in an instruction.
	 * @note if the instruction was not conditional, this value should be null
	 */
    public Nullable<bool> ConditionalEvaluation { get; }

    /**
	 * Constructor
	 * @param runtimeErrorDetected A flag that specifies whether the instruction 
	 *        that generated this report was valid.
	 * @param conditionalEvaluation (Optional) A nullable flag that specifies the actor's
	 *        evaluation of the instructions conditional expression, if applicable.
	 */
    public ExecutionReport(bool runtimeErrorDetected, bool? conditionalEvaluation = null)
    {
        RuntimeErrorDetected = runtimeErrorDetected;
        ConditionalEvaluation = conditionalEvaluation;
    }
}
