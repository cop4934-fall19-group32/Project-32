using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// The LevelData class contains holds data that the LevelManager uses to create the scene for a chosen level.
/// Members here are pieces of data that variate from level to level.
/// </summary>
[Serializable]
public class LevelData
{
    public int numRegisterCards;
    public int numStackCards;
    public int numQueueCards;
    public int numHeapCards;

    public LevelData(int numRegisterCards, int numStackCards, int numQueueCards, int numHeapCards)
    {
        this.numRegisterCards = numRegisterCards;
        this.numStackCards = numStackCards;
        this.numQueueCards = numQueueCards;
        this.numHeapCards = numHeapCards;
    }
}
