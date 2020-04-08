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

    [SerializeField]
    private List<OpCode> AwardedInstructions;

    [SerializeField]
    public string LastAttemptedLevel;

    /**Non-serializable disctionary */
    public Dictionary<string, PuzzleSave> PuzzleSaveDictionary;

    public HashSet<OpCode> AwardedInstructionsHashSet;

    /**
     * Constructor
     */
    public Save() {
        PlayerScore = 0;
        PuzzleSaveDictionary = new Dictionary<string, PuzzleSave>();
        AwardedInstructionsHashSet = new HashSet<OpCode>();
        LastAttemptedLevel = "";
    }

    public void AddPuzzleSave(PuzzleSave puzzleSave) {
        PuzzleSaveDictionary.Add(puzzleSave.LevelName, puzzleSave);
    }

    public void OnBeforeSerialize() {
        PuzzleSaves = new List<PuzzleSave>();
        AwardedInstructions = new List<OpCode>();

        foreach(var entry in PuzzleSaveDictionary) {
            PuzzleSaves.Add(entry.Value);
        }

        foreach (var instruction in AwardedInstructionsHashSet) {
            AwardedInstructions.Add(instruction);
        }
    }

    public void OnAfterDeserialize() {
        PuzzleSaveDictionary = new Dictionary<string, PuzzleSave>();
        AwardedInstructionsHashSet = new HashSet<OpCode>();

        foreach (var puzzle in PuzzleSaves) {
            PuzzleSaveDictionary.Add(puzzle.LevelName, puzzle);
        }

        foreach (var instruction in AwardedInstructions) {
            AwardedInstructionsHashSet.Add(instruction);
        }
    }
}
