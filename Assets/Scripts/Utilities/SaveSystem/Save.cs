using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[System.Serializable]
public class Save : ISerializationCallbackReceiver {
    [SerializeField]
    public int PlayerScore;

    [SerializeField]
    private List<PuzzleSave> PuzzleSaves;

    /**Non-serializable disctionary */
    public Dictionary<string, PuzzleSave> PuzzleSaveDictionary;

    /**
     * Constructor
     */
    public Save() {
        PlayerScore = 0;
        PuzzleSaveDictionary = new Dictionary<string, PuzzleSave>();
    }

    public void AddPuzzleSave(PuzzleSave puzzleSave) {
        PuzzleSaveDictionary.Add(puzzleSave.LevelName, puzzleSave);
    }

    public void OnBeforeSerialize() {
        PuzzleSaves = new List<PuzzleSave>();

        foreach(var entry in PuzzleSaveDictionary) {
            PuzzleSaves.Add(entry.Value);
        }
    }

    public void OnAfterDeserialize() {
        PuzzleSaveDictionary = new Dictionary<string, PuzzleSave>();

        foreach (var puzzle in PuzzleSaves) {
            PuzzleSaveDictionary.Add(puzzle.LevelName, puzzle);
        }
    }
}
