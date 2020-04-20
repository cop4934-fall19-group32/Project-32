using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitPanel : MonoBehaviour
{
    public GameObject SubmitPanelObject;
    public GameObject EfficiencyScoreBoard;
    public GameObject InstructionCountScoreBoard;
    public GameObject MemoryScoreBoard;
    public GameObject StarEfficiency;
    public GameObject StarInstructionCount;
    public GameObject StarMemory;
    public GameObject EmptyStarEfficiency;
    public GameObject EmptyStarInstructionCount;
    public GameObject EmptyStarMemory;
    public TMPro.TextMeshProUGUI EfficiencyNeeded;
    public TMPro.TextMeshProUGUI InstructionCountNeeded;
    public TMPro.TextMeshProUGUI MemoryNeeded;
    public TMPro.TextMeshProUGUI EfficiencyEarned;
    public TMPro.TextMeshProUGUI InstructionCountEarned;
    public TMPro.TextMeshProUGUI MemoryEarned;

    public void FillStars(bool earnedEfficiency, bool earnedInstructionCount, bool earnedMemory)
    {
        if (earnedEfficiency)
        {
            StarEfficiency.SetActive(true);
        }

        if (earnedInstructionCount)
        {
            StarInstructionCount.SetActive(true);
        }

        if (earnedMemory)
        {
            StarMemory.SetActive(true);
        }
    }

    public void ShowScores(float EfficiencyScore, int InstructionCountScore, float MemoryScore)
    {
        EfficiencyEarned.text = EfficiencyScore.ToString();
        InstructionCountEarned.text = InstructionCountScore.ToString();
        MemoryEarned.text = MemoryScore.ToString();
    }

    public void ShowEarnableStars(PuzzleData puzzleData)
    {
        if (puzzleData.HasEfficiency)
        {
            EfficiencyScoreBoard.SetActive(true);
            EmptyStarEfficiency.SetActive(true);
            EfficiencyNeeded.text = "<= " + puzzleData.EfficiencyRequirement.ToString() + " cycles (avg.)";
        }

        if (puzzleData.HasInstructionCount)
        {
            InstructionCountScoreBoard.SetActive(true);
            EmptyStarInstructionCount.SetActive(true);
            InstructionCountNeeded.text = "<= " + puzzleData.InstructionCountRequirement.ToString() + " instructions";
        }

        if (puzzleData.HasMemory)
        {
            MemoryScoreBoard.SetActive(true);
            EmptyStarMemory.SetActive(true);
            MemoryNeeded.text = "<= " + puzzleData.MemoryRequirement.ToString() + " bytes";
        }
    }

    public void ShowSubmitPanel()
    {
        SubmitPanelObject.SetActive(true);
    }

    public void RemoveSubmitPanel()
    {
        StarEfficiency.SetActive(false);
        StarInstructionCount.SetActive(false);
        StarMemory.SetActive(false);
        EmptyStarEfficiency.SetActive(false);
        EmptyStarInstructionCount.SetActive(false);
        EmptyStarMemory.SetActive(false);
        EfficiencyScoreBoard.SetActive(false);
        InstructionCountScoreBoard.SetActive(false);
        MemoryScoreBoard.SetActive(false);
        SubmitPanelObject.SetActive(false);
    }
}