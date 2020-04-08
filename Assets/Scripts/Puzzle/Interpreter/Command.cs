using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * Enumeration defining the instruction available in the Computron Assembly Language (CAL)
 */
public enum OpCode
{
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
    SUBTRACT,
    SUBMIT,
    NO_OP
}

/**
 * @class Command
 * @brief A Command is the data structure sent to the Actor when it requests the next
 *        instruction from the interpreter.
 */
public class Command : MonoBehaviour
{
    [SerializeField]
    public OpCode Instruction;

    [SerializeField]
    public Nullable<uint> Arg;

    [SerializeField]
    public Nullable<uint> Target;
}

/**
 * Struct used by save system to serialze commands
 */
[Serializable]
public struct CachedCommand {
    public OpCode instruction;

    public int arg;

    public int target;
    
    public CachedCommand(OpCode i, int a, int t) {
        instruction = i;
        
        arg = a;

        target = t;
    }
}