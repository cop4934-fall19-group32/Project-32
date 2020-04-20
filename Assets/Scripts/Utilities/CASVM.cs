using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CASVM 
{
    public List<Command> Instructions { get; set; }
    public int[] Inputs { get; set; }
    public bool GenerateRandomInputs { get; set; }
    public Dictionary<string, CardDatastructure> MemoryCards { get; set; }

    public SolutionGenerator OutputGenerator { get; set; }

    private List<int> expectedOutput;

    private List<int> output = new List<int>();

    private int programCounter;

    public int RandomSeed = -1;

    private int? computronValue;

    private int inputPtr = 0;

    private bool error = false;

    private int steps = 0;

    public CASVM() {
        programCounter = 0;
        MemoryCards = new Dictionary<string, CardDatastructure>();
    }

    public int? Run() {
        programCounter = 0;
        SetupRun();
        while (programCounter < Instructions.Count && !error) {
            if (error) return null;

            steps++;

            switch (Instructions[programCounter].Instruction){
                case OpCode.INPUT:
                    Input();
                    break;
                case OpCode.OUTPUT:
                    Output();
                    break;
                case OpCode.JUMP:
                    Jump();
                    break;
                case OpCode.JUMP_IF_NULL:
                    JumpIfNull();
                    break;
                case OpCode.JUMP_IF_LESS:
                    JumpIfLess();
                    break;
                case OpCode.JUMP_IF_GREATER:
                    JumpIfGreater();
                    break;
                case OpCode.JUMP_IF_EQUAL:
                    JumpIfEqual();
                    break;
                case OpCode.MOVE_TO:
                    MoveTo();
                    break;
                case OpCode.MOVE_FROM:
                    MoveFrom();
                    break;
                case OpCode.COPY_TO:
                    CopyTo();
                    break;
                case OpCode.COPY_FROM:
                    CopyFrom();
                    break;
                case OpCode.ADD:
                    Add();
                    break;
                case OpCode.SUBTRACT:
                    Subtract();
                    break;
                case OpCode.SUBMIT:
                    programCounter++;
                    steps--;
                    ValidateOutput();
                    break;
                default:
                    programCounter++;
                    break;
            }
        }

        return steps;
    }

    private void Input() {
        programCounter++;
        if (inputPtr >= Inputs.Length) {
            computronValue = null;
            return;
        }
        computronValue = Inputs[inputPtr++];
    }

    private void Output() {
        if (computronValue == null) {
            error = true;
            return;
        }

        output.Add((int)computronValue);
        computronValue = null;
        ValidateOutput();
        programCounter++;
    }

    private void Jump() {
        programCounter = (int)Instructions[programCounter].Target;
    }

    private void JumpIfNull() {
        if (computronValue == null) {
            programCounter = (int)Instructions[programCounter].Target;
        }
        else {
            programCounter++;
        }
    }

    private void JumpIfGreater() {
        string address = "0x" + System.Convert.ToString((int)Instructions[programCounter].Arg, 16);
        var cardValue = MemoryCards[address].Peek();

        if (computronValue == null || cardValue == null) {
            error = true;
            return;
        }

        if (computronValue > cardValue) {
            programCounter = (int)Instructions[programCounter].Target;
        }
        else {
            programCounter++;
        }
    }

    private void JumpIfLess() {
        string address = "0x" + System.Convert.ToString((int)Instructions[programCounter].Arg, 16);
        var cardValue = MemoryCards[address].Peek();

        if (computronValue == null || cardValue == null) {
            error = true;
            return;
        }

        if (computronValue < cardValue) {
            programCounter = (int)Instructions[programCounter].Target;
        }
        else {
            programCounter++;
        }
    }

    private void JumpIfEqual() {
        string address = "0x" + System.Convert.ToString((int)Instructions[programCounter].Arg, 16);
        var cardValue = MemoryCards[address].Peek();

        if (computronValue == null || cardValue == null) {
            error = true;
            return;
        }

        if (computronValue == cardValue) {
            programCounter = (int)Instructions[programCounter].Target;
        }
        else {
            programCounter++;
        }
    }

    private void MoveTo() {
        if (computronValue == null) {
            programCounter++;
            error = true;
            return;
        }
        else { 
            string address = "0x" + System.Convert.ToString((int)Instructions[programCounter++].Arg, 16);
            MemoryCards[address].Add((int)computronValue);
            computronValue = null;
        }
    }

    private void MoveFrom() {
        string address = "0x" + System.Convert.ToString((int)Instructions[programCounter++].Arg, 16);
        computronValue = MemoryCards[address].Remove();
    }

    private void CopyTo() {
        if (computronValue == null) {
            programCounter++;
            error = true;
            return;
        }
        else {
            string address = "0x" + System.Convert.ToString((int)Instructions[programCounter++].Arg, 16);
            MemoryCards[address].Add((int)computronValue);
        }
    }

    private void CopyFrom() {
        string address = "0x" + System.Convert.ToString((int)Instructions[programCounter++].Arg, 16);
        computronValue = MemoryCards[address].Peek();
    }

    private void Add() {
        string address = "0x" + System.Convert.ToString((int)Instructions[programCounter++].Arg, 16);
        if (computronValue == null || MemoryCards[address].Peek() == null) {
            error = true;
            return;
        }

        computronValue += MemoryCards[address].Peek();
    }

    private void Subtract() {
        string address = "0x" + System.Convert.ToString((int)Instructions[programCounter++].Arg, 16);
        if (computronValue == null || MemoryCards[address].Peek() == null) {
            error = true;
            return;
        }

        computronValue -= MemoryCards[address].Peek();
    }


    private void ValidateOutput() {
        for (int i = 0; i < output.Count; i++) {
            if (output[i] == expectedOutput[i]) {
                continue;
            }
            else {
                error = true;
                return;
            }
        }
    }

    private void SetupRun() {
        if (GenerateRandomInputs) {
            Inputs = new int[Inputs.Length];
            System.Random random = (RandomSeed == -1) ? new System.Random() : new System.Random(RandomSeed);

            for (int i = 0; i < Inputs.Length; i++) {
                Inputs[i] = random.Next(0, 99);
            }
        }

        inputPtr = 0;
        output = new List<int>();
        computronValue = null;
        steps = 0;
        expectedOutput = OutputGenerator.GenerateSolution(Inputs);
        //Reset memory cards
        foreach (var entry in MemoryCards) {
            entry.Value.ClearData();
        }
    }
}
