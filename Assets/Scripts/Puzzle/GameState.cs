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
    private PuzzleGenerator puzzleGenerator;

    private string defaultLevel = "Default";

    void Awake()
    {
        puzzleGenerator = GetComponent<PuzzleGenerator>();
        InitGame();
    }

    void InitGame()
    {
        if (PlayerPrefs.HasKey("SelectedLevel"))
            puzzleGenerator.SetupBoard(PlayerPrefs.GetString("SelectedLevel"));
        else
            puzzleGenerator.SetupBoard(defaultLevel);
    }
}
