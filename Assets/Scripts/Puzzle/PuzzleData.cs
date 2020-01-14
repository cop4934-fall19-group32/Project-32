using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// The PuzzleData class contains data that is used when generating/loading
/// a specific puzzle state onto the puzzle board scene.
/// </summary>
[Serializable]
public class PuzzleData
{
    [SerializeField]
    private int _numRegisterCards;
    [SerializeField]
    private int _numStackCards;
    [SerializeField]
    private int _numQueueCards;
    [SerializeField]
    private int _numHeapCards;

    [SerializeField]
    private string _description;

    [SerializeField]
    private int[] _inputStream;

    // Each index of this array corresponds to the enum index of the OpCodes
    // found in Interpreter.cs
    // The contents at each index represent the number of instructions that are
    // available to the player.
    // For example, index 3 is the INPUT OpCode.
    // If Instructions[3] == 2, there are two total available INPUT instructions available
    // to the player.
    [SerializeField]
    private int[] _instructions;

    public int[] Instructions
    {
        get => _instructions;
        set => _instructions = value;
    }

    public int NumRegisterCards
    {
        get => _numRegisterCards;
    }

    public int NumStackCards
    {
        get => _numStackCards;
    }

    public int NumQueueCards
    {
        get => _numQueueCards;
    }

    public int NumHeapCards
    {
        get => _numHeapCards;
    }
    
    public int[] InputStream
    {
        get => _inputStream;
        set => _inputStream = value;
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }
    
    public void setRegisterCards(int numRegisterCards, int numStackCards, int numQueueCards, int numHeapCards)
    {
        _numRegisterCards = numRegisterCards;
        _numStackCards = numStackCards;
        _numQueueCards = numQueueCards;
        _numHeapCards = numHeapCards;
    }
}
