using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// The GameState class controls the state of the game at any given moment.
/// Currently, it makes use of the PuzzleGenerator to fill the puzzle board
/// scene with necessary data.
/// </summary>
public class GameState : MonoBehaviour
{
    public PuzzleGenerator puzzleGenerator;

    private int level = 1;

    void Awake()
    {
        puzzleGenerator = GetComponent<PuzzleGenerator>();
        InitGame();
    }

    void InitGame()
    {
        puzzleGenerator.SetupBoard(level);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
