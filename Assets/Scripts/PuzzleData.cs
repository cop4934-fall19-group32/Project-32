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
    private int[] _inputStream;

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
    }
    
    public PuzzleData(int[] inputStream, int numRegisterCards, int numStackCards, int numQueueCards, int numHeapCards)
    {
        _numRegisterCards = numRegisterCards;
        _numStackCards = numStackCards;
        _numQueueCards = numQueueCards;
        _numHeapCards = numHeapCards;
        _inputStream = inputStream;
    }
}
