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
///
[RequireComponent(typeof(InstructionFactory))]
[RequireComponent(typeof(CardGenerator))]
public class PuzzleGenerator : MonoBehaviour
{
    private string puzzleName;
    private PuzzleData puzzleData;
    private PlayerState playerState;
    private GameState gameState;

    private bool FirstVisit = false;
    public GameObject UICanvas;
    public GameObject CardHandPanel;
    public GameObject CardPlayedPanel;
    public GameObject SubmitPanel;
    public GameObject InputBox;
    public GameObject OutputBox;
    public GameObject SolutionWindow;

    private bool firstFrame = true;

    private void Awake()
    {
        gameState = FindObjectOfType<GameState>();
        puzzleData = gameState.SelectedPuzzle;
        GameObject playerStateObj = GameObject.Find("PlayerState");

        if (playerStateObj == null)
        {
            Debug.Log("Could not find PlayerState game object.");
            return;
        }

        playerState = playerStateObj.GetComponent<PlayerState>();
        puzzleName = puzzleData.PuzzleName;

        SetupBoard();
    }

    protected void Update() {
        if (FirstVisit && firstFrame) {
            // Starting a coroutine allows us to use yield semantics to control the timings between tutorialization steps
            StartCoroutine(TutorializationRoutine());
            firstFrame = false;
        }
    }

    public void SetupBoard() {
        // Fill the puzzle scene with the puzzle data.
        UICanvas.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = puzzleData.Description;
        GetComponent<CardGenerator>().GenerateHand(puzzleData);
        InputBox.GetComponent<InputBox>().InitializeInput(puzzleData.InputStream);
        OutputBox.GetComponent<OutputBox>().InitializeOutput(puzzleData.OutputStream);

        if (!playerState.ContainsPuzzleSave(this.puzzleName)) {
            FirstVisit = true;
            playerState.AddPuzzleSave(this.puzzleName);
        }

        // Fill cached data.
        LoadCachedCards(playerState.GetCachedCards(this.puzzleName));
        LoadCachedInstructions(playerState.GetCachedSolution(this.puzzleName));

    }

    private PlayerState GetPlayerState()
    {
        GameObject playerStateObj = GameObject.Find("PlayerState");

        if (playerStateObj == null)
            return null;

        return playerStateObj.GetComponent<PlayerState>();
    }

    private void LoadCachedInstructions(List<CachedCommand> CachedInstructions)
    {
        Transform solutionPanel = UICanvas.transform.Find("Solution Window");
        Transform instructionContainer = solutionPanel.Find("Scroll View/Viewport/Content");
        Vector3 instructionScale = new Vector3((float)0.9999999, (float)0.999999, (float)0.9999999);
        // Spawn instructions
        foreach (var command in CachedInstructions)
        {
            GameObject instructionObj =
                GetComponent<InstructionFactory>().SpawnInstruction(
                    command.instruction,
                    instructionContainer
                );

            //Special actions must be taken for card based instructions
            if (instructionObj.GetComponent<CardCommandDragNDrop>() != null)
            {
                var address = "0x" + command.arg.ToString("x3");
                var card = GameObject.Find(address);
                instructionObj.GetComponent<CardCommandDragNDrop>().BindCard(card);
                card.GetComponent<CardLogic>().LinkInstruction(instructionObj);
            }

            instructionObj.GetComponent<DragNDrop>().isClonable = false;
            instructionObj.GetComponent<DragNDrop>().activeDynamicScrollView = SolutionWindow.GetComponent<InstructionContainer>();
            instructionObj.GetComponent<Command>().Instruction = command.instruction;
            instructionObj.GetComponent<Command>().Arg = (uint)command.arg;
            instructionObj.GetComponent<Command>().Target = (uint)command.target;
            instructionObj.GetComponent<DragNDrop>().Initialize();
            instructionObj.transform.localScale = instructionScale;
            instructionObj.GetComponent<ControllableUIElement>().Initialize();
        }

        //Iterate over instructions to reconnect jump lines
        foreach (Transform child in instructionContainer)
        {
            var instruction = child.GetComponent<Command>();
            if (
                instruction.Instruction == OpCode.JUMP ||
                instruction.Instruction == OpCode.JUMP_IF_NULL ||
                instruction.Instruction == OpCode.JUMP_IF_GREATER ||
                instruction.Instruction == OpCode.JUMP_IF_LESS
            )
            {
                var dragNDropBehavior = child.GetComponent<JumpDragNDropBehavior>();
                dragNDropBehavior.AttachAnchor(instructionContainer.GetChild((int)instruction.Target).gameObject);
            }
        }
    }

    private void LoadCachedCards(List<CachedCard> CachedCards)
    {
        var hand = CardHandPanel.GetComponent<CachedCardContainer>();
        var spawnedCards = hand.GetCards();

        foreach (var card in CachedCards)
        {
            // Find the corresponding card in the card hand panel and move it to
            // the card played panel.
            foreach (var cardObj in spawnedCards)
            {
                if (cardObj.name == card.address)
                {
                    CardPlayedPanel.GetComponent<PlayedCardContainer>().AddCard(cardObj);
                    spawnedCards.Remove(cardObj);
                    break;
                }
            }
        }
    }

    public List<OpCode> GetFilteredInstructions()
    {
        List<OpCode> filteredInstructions = new List<OpCode>();

        if (puzzleData.Input)
            filteredInstructions.Add(OpCode.INPUT);
        if (puzzleData.Output)
            filteredInstructions.Add(OpCode.OUTPUT);
        if (puzzleData.Jump)
            filteredInstructions.Add(OpCode.JUMP);
        if (puzzleData.JumpIfNull)
            filteredInstructions.Add(OpCode.JUMP_IF_NULL);
        if (puzzleData.JumpIfLess)
            filteredInstructions.Add(OpCode.JUMP_IF_LESS);
        if (puzzleData.JumpIfGreater)
            filteredInstructions.Add(OpCode.JUMP_IF_GREATER);
        if (puzzleData.MoveTo)
            filteredInstructions.Add(OpCode.MOVE_TO);
        if (puzzleData.CopyTo)
            filteredInstructions.Add(OpCode.COPY_TO);
        if (puzzleData.MoveFrom)
            filteredInstructions.Add(OpCode.MOVE_FROM);
        if (puzzleData.CopyFrom)
            filteredInstructions.Add(OpCode.COPY_FROM);
        if (puzzleData.Add)
            filteredInstructions.Add(OpCode.ADD);
        if (puzzleData.Subtract)
            filteredInstructions.Add(OpCode.SUBTRACT);

        return filteredInstructions;
    }

    public List<OpCode> GetInstructionsToAward()
    {
        List<OpCode> instructionsToAward = new List<OpCode>();
        List<OpCode> instructions = GetFilteredInstructions();
        foreach (OpCode instruction in instructions)
        {
            if (!playerState.InstructionAwarded(instruction))
            {
                instructionsToAward.Add(instruction);
                playerState.AddAwardedInstruction(instruction);
            }
        }
        return instructionsToAward;
    }

    public void ResetBoard()
    {
        InputBox.GetComponent<InputBox>().ResetInput(puzzleData.InputStream);
        OutputBox.GetComponent<OutputBox>().ResetOutput();
        CardPlayedPanel.GetComponent<CardContainer>().ResetCards();
    }

    IEnumerator TutorializationRoutine()
    {
        var controller = FindObjectOfType<UIController>();
        controller.ScanForControls();

        var awarder = UICanvas.GetComponent<NewInstructionController>();

        //Blocks execution until instructions are awarded
        if (awarder)
        {
            yield return StartCoroutine(awarder.IntroduceNewInstructions(GetInstructionsToAward()));
        }

        var tutorial = gameState.SelectedPuzzle.Tutorial;

        if (gameState != null && tutorial != null)
        {
            yield return StartCoroutine(tutorial.RunTutorial());
        }
    }
}
