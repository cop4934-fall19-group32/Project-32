using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCacher : MonoBehaviour
{
    public GameObject UICanvas;
    public GameObject CardPlayedPanel;
    private string PuzzleName;
    public bool EarnedEfficiencyStar = false;
    public bool EarnedInstructionCountStar = false;
    public bool EarnedMemoryStar = false;
    public bool solved;
    public PuzzleData puzzleData;
    private PlayerState playerState;

    private const float REGISTER_COST = 1f;
    private const float QUEUE_COST = 3f;
    private const float STACK_COST = 3f;
    private const float HEAP_COST = 5f;

    private float EfficiencyScore;
    private int InstructionCountScore;
    private float MemoryScore;

    private void Awake()
    {
        puzzleData = FindObjectOfType<GameState>().SelectedPuzzle;
        GameObject playerStateObj = GameObject.Find("PlayerState");

        if (playerStateObj == null)
        {
            Debug.Log("Could not find PlayerState game object.");
            return;
        }

        this.playerState = playerStateObj.GetComponent<PlayerState>();
        this.PuzzleName = puzzleData.PuzzleName;
    }

    private void ResetPuzzleSolution()
    {
        this.playerState.SavePuzzleSolution(this.PuzzleName, null, null);
    }

    private void SavePuzzleSolution()
    {
        GameObject solutionPanel = UICanvas.transform.Find("Solution Window").gameObject;
        this.playerState.SavePuzzleSolution(this.PuzzleName, solutionPanel, CardPlayedPanel);
    }

    private void MarkPuzzleCompleted()
    {
        // No need to do anything if this puzzle has already been completed.
        if (this.playerState.GetPuzzleCompleted(this.PuzzleName))
            return;

        this.playerState.MarkPuzzleCompleted(this.PuzzleName);
        this.playerState.AddToScore(1);
    }

    private void UpdateLevelStars()
    {
        if (EarnedEfficiencyStar)
        {
            if (!playerState.GetStarEarned(PuzzleName, StarType.EFFICIENCY))
            {
                playerState.EarnStar(PuzzleName, StarType.EFFICIENCY);
                playerState.AddToScore(1);
            }
        }
        if (EarnedInstructionCountStar)
        {
            if (!playerState.GetStarEarned(PuzzleName, StarType.INSTRUCTION_COUNT))
            {
                playerState.EarnStar(PuzzleName, StarType.INSTRUCTION_COUNT);
                playerState.AddToScore(1);
            }
        }
        if (EarnedMemoryStar)
        {
            if (!playerState.GetStarEarned(PuzzleName, StarType.MEMORY))
            {
                playerState.EarnStar(PuzzleName, StarType.MEMORY);
                playerState.AddToScore(1);
            }
        }
    }

    private float GetMemoryCost()
    {
        float sum = 0f;
        foreach (Transform cardObj in CardPlayedPanel.transform)
        {
            CardLogic cardLogic = cardObj.GetComponent<CardLogic>();
            sum += cardLogic.Cost;
        }
        return sum;
    }

    public void AwardStars()
    {
        GameObject interpreterObj = GameObject.Find("Interpreter");
        Interpreter interpreter = interpreterObj.GetComponent<Interpreter>();

        if (puzzleData.HasEfficiency)
        {
            EfficiencyScore = interpreter.GetAverageRuntime();
            if (EfficiencyScore <= puzzleData.EfficiencyRequirement)
            {
                this.EarnedEfficiencyStar = true;
            }
        }

        if (puzzleData.HasInstructionCount)
        {
            InstructionCountScore = interpreter.GetInstructionCount();
            if (InstructionCountScore <= puzzleData.InstructionCountRequirement)
            {
                this.EarnedInstructionCountStar = true;
            }
        }

        if (puzzleData.HasMemory)
        {
            MemoryScore = GetMemoryCost();
            if (MemoryScore <= puzzleData.MemoryRequirement)
            {
                this.EarnedMemoryStar = true;
            }
        }
    }

    public void ShowAwardPanel()
    {
        // Display SubmitPanel.
        GameObject PuzzleCacherObj = GameObject.Find("PuzzleCacher");
        PuzzleCacher PuzzleCacher = PuzzleCacherObj.GetComponent<PuzzleCacher>();
        AwardStars();
        this.gameObject.GetComponent<SubmitPanel>().ShowSubmitPanel();
        this.gameObject.GetComponent<SubmitPanel>().ShowEarnableStars(puzzleData);
        this.gameObject.GetComponent<SubmitPanel>().FillStars(EarnedEfficiencyStar, EarnedInstructionCountStar, EarnedMemoryStar);
        this.gameObject.GetComponent<SubmitPanel>().ShowScores(this.EfficiencyScore, this.InstructionCountScore,
            this.MemoryScore);
        PuzzleCacher.solved = true;
    }

    public void UpdateActiveSave()
    {
        var GarbageLevelData = FindObjectOfType<PuzzleData>().gameObject;
        Destroy(GarbageLevelData);
        ResetPuzzleSolution();
        SavePuzzleSolution();

        if (this.solved)
            MarkPuzzleCompleted();

        UpdateLevelStars();
        this.playerState.SaveGame();
    }


    public bool CheckIfPuzzleDataExists()
    {
        // Check if the puzzle data actually exists.
        // If you play the game from the Unity editor starting from the puzzle scene, then there will be no puzzle data.
        // The pause menu checks if this is the case so it won't throw an error during ExitScene() and trap the player in the puzzle scene.
        return this.puzzleData != null;
    }
}
