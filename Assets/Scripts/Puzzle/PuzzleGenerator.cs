using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The PuzzleGenerator class is used to fill the puzzle board scene with
/// data that is specific to a puzzle's intial state or it's saved state.
/// </summary>
public class PuzzleGenerator : MonoBehaviour
{
    private string puzzleName;
    private PuzzleData puzzleData;

    public bool solved;
    private bool EarnedEfficiencyStar;
    private bool EarnedInstructionCountStar;
    private bool EarnedMemoryStar;

    public GameObject UICanvas;
    public GameObject CardHandPanel;
    public GameObject CardPlayedPanel;
    public GameObject PuzzleDataParent;
    public GameObject InputBox;
    public GameObject OutputBox;
    public GameObject CachedInstruction;
    public GameObject JumpCachedInstruction;
    public GameObject JumpAnchorCachedInstruction;

    private PlayerState GetPlayerState()
    {
        GameObject playerStateObj = GameObject.Find("PlayerState");

        if (playerStateObj == null)
            return null;

        return playerStateObj.GetComponent<PlayerState>();
    }

    private void ResetPuzzleSolution(PlayerState playerState)
    {
        playerState.SavePuzzleSolution(this.puzzleName, null, null);
    }

    private void SavePuzzleSolution(PlayerState playerState)
    {
        GameObject solutionPanel = UICanvas.transform.Find("Solution Window").gameObject;
        playerState.SavePuzzleSolution(this.puzzleName, solutionPanel, CardPlayedPanel);
    }

    private void MarkPuzzleCompleted(PlayerState playerState)
    {
        // No need to do anything if this puzzle has already been completed.
        if (playerState.GetPuzzleCompleted(this.puzzleName))
            return;

        playerState.MarkPuzzleCompleted(this.puzzleName);
        playerState.AddToScore(1);
    }

    private void UpdateLevelStars(PlayerState playerState)
    {
        if (this.EarnedEfficiencyStar)
            playerState.EarnStar(this.puzzleName, StarType.EFFICIENCY);
        if (this.EarnedInstructionCountStar)
            playerState.EarnStar(this.puzzleName, StarType.INSTRUCTION_COUNT);
        if (this.EarnedMemoryStar)
            playerState.EarnStar(this.puzzleName, StarType.MEMORY);
    }

    public void UpdateActiveSave()
    {
        PlayerState playerState = GetPlayerState();

        if (playerState == null)
        {
            Debug.Log("Could not find PlayerState game object.");
            return;
        }

        ResetPuzzleSolution(playerState);
        SavePuzzleSolution(playerState);

        if (this.solved)
            MarkPuzzleCompleted(playerState);

        UpdateLevelStars(playerState);
        playerState.SaveGame();
    }

    private void LoadCachedInstructions(List<CachedCommand> CachedInstructions)
    {
        Transform solutionPanel = UICanvas.transform.Find("Solution Window");
        Transform instructionContainer = solutionPanel.Find("Scroll View/Viewport/Content");
        Vector3 instructionScale = new Vector3((float)0.9999999, (float)0.999999, (float)0.9999999);
        //Spawn instructions
        foreach (var command in CachedInstructions)
        {
            GameObject instructionObj;

            if (command.instruction == OpCode.JUMP || command.instruction == OpCode.JUMP_IF_NULL) {
                instructionObj = Instantiate(JumpCachedInstruction, new Vector3(0, 0, 0), Quaternion.identity);
            }
            else if (command.instruction == OpCode.NO_OP) {
                instructionObj = Instantiate(JumpAnchorCachedInstruction, new Vector3(0, 0, 0), Quaternion.identity);
            }
            else {
                instructionObj = Instantiate(CachedInstruction, new Vector3(0, 0, 0), Quaternion.identity);
            }

            instructionObj.GetComponent<DragNDrop>().isClonable = false;
            instructionObj.GetComponent<Command>().Instruction = command.instruction;
            instructionObj.GetComponent<Command>().Arg = (uint)command.arg;
            instructionObj.transform.SetParent(instructionContainer);
            instructionObj.GetComponent<DragNDrop>().Initialize();
            instructionObj.transform.localScale = instructionScale;
        }

        //Iterate over instructions to reconnect jump lines
        foreach (Transform child in instructionContainer) {
            var instruction = child.GetComponent<Command>();
            if (instruction.Instruction == OpCode.JUMP || instruction.Instruction == OpCode.JUMP_IF_NULL) {
                var dragNDropBehavior = child.GetComponent<JumpDragNDropBehavior>();
                dragNDropBehavior.AttachAnchor(instructionContainer.GetChild((int)instruction.Arg).gameObject);
            }
        }
    }

    private void LoadCachedCards(List<CachedCard> CachedCards)
    {
        //foreach (var card in CachedCards)
        //{
        //    // Find the corresponding card in the card hand panel and move it to
        //    // the card played panel.
        //    foreach (Transform cardObj in CardHandPanel.transform)
        //    {
        //        if (cardObj.name.Contains(card.type))
        //        {
        //            cardObj.SetParent(CardPlayedPanel.transform);
        //            cardObj.localScale = cardObj.GetComponent<CardDragBehavior>().boardScale;
        //            break;
        //        }
        //    }
        //}
    }

    public void ResetBoard()
    {
        InputBox.GetComponent<InputBox>().ResetInput(puzzleData.InputStream);
        OutputBox.GetComponent<OutputBox>().ResetOutput();
        CardPlayedPanel.GetComponent<CardContainer>().ResetCards();
    }

    public void SetupBoard(string puzzleName)
    {
        this.puzzleName = puzzleName;
        this.solved = false;
        this.EarnedEfficiencyStar = false;
        this.EarnedInstructionCountStar = false;
        this.EarnedMemoryStar = false;
        
        // Grab the correct puzzle data.
        Transform puzzleDataTransform = PuzzleDataParent.transform.Find(puzzleName);
        this.puzzleData = puzzleDataTransform.GetComponent<PuzzleData>();

        // Fill the puzzle scene with the puzzle data.
        UICanvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = puzzleData.Description;
        CardHandPanel.GetComponent<CardGenerator>().InitializeHand(puzzleData);
        InputBox.GetComponent<InputBox>().InitializeInput(puzzleData.InputStream);
        OutputBox.GetComponent<OutputBox>().InitializeOutput(puzzleData.InputStream, puzzleName);

        // Fill cached data.
        PlayerState playerState = GetPlayerState();

        if (!playerState.ContainsPuzzleSave(this.puzzleName))
            playerState.AddPuzzleSave(this.puzzleName);

        LoadCachedInstructions(playerState.GetCachedSolution(this.puzzleName));
        LoadCachedCards(playerState.GetCachedCards(this.puzzleName));
    }
}
