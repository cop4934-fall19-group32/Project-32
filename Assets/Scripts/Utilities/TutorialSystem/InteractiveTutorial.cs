using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveTutorial : MonoBehaviour {
    public TutorialTextBoxController TextBoxController;

    private List<TutorialStep> Steps { get; set; } = new List<TutorialStep>();

    public UIController uiController { get { return FindObjectOfType<UIController>(); } }

    private bool running;

    /**
     * Function overriden in subclasses to initialize Steps
     */
    protected abstract void Awake();

    protected void AddStep(TutorialStep step) {
        Steps.Add(step);
    }

    public void StopTutorial() {
        running = false;

        //In case the player disabled something mid tutorial
        EnableAllInSolutionWinow();
    }

    public IEnumerator RunTutorial() {
        GameObject solutionWindow = GameObject.FindGameObjectWithTag("SolutionWindow");
        solutionWindow.GetComponent<InstructionContainer>().DestroyAllChildren();
        
        var cardContainer = FindObjectOfType<PlayedCardContainer>();
        if (cardContainer) {
            yield return StartCoroutine(cardContainer.ReturnCards());
        }
        var quitButton = FindObjectOfType<QuitTutorialHandler>();
        quitButton.Show();
        running = true;
        yield return null;

        var textbox = GameObject.FindGameObjectWithTag("TutorialTextBox");
        TextBoxController = textbox.GetComponent<TutorialTextBoxController>();
        TextBoxController.Activate();
        foreach (var step in Steps) {
            if (!running) {
                yield break;
            }

            TextBoxController.SetTutorialMessage(step.Description);
            step.Begin();

            while (!step.Complete) {
                if (!running) {
                    yield break;
                }
                yield return null;
            }

            if (!running) {
                yield break;
            }

            if (step.EndDelay > 0) {
                yield return new WaitForSeconds(step.EndDelay);
            }
            step.End();

            //Clean up tutorial state to dissallow steps from affecting each other
            EnableAllInSolutionWinow();
            uiController.ClearFocus();
            uiController.StopAllHighlights();
            TextBoxController.Activate();
        }
        TextBoxController.Deactivate();
        quitButton.GetComponent<Canvas>().enabled = false;
    }

    protected void FocusInstruction(OpCode op) {
        var instructionCache = GameObject.FindGameObjectWithTag("InstructionCacheContent");
        var instructions = instructionCache.GetComponentsInChildren<Command>();
        foreach (var instruction in instructions) {
            if (instruction.Instruction == op) {
                instruction.GetComponent<UIControl>().Focus();
            }
        }
    }

    protected void EnableAllInSolutionWinow() {
        var container = FindObjectOfType<InstructionContainer>().contentPanel;
        var commands = container.GetComponentsInChildren<Command>();
        foreach (var command in commands) {
            command.GetComponent<UIControl>().Enable();
        }
    }

    protected void EnableInstructionInSolutionWindow(int num) {
        var container = FindObjectOfType<InstructionContainer>().contentPanel;
        var commands = container.GetComponentsInChildren<Command>();

        commands[num].GetComponent<UIControl>().Enable();
    }

    protected void DisableInstructionInSolutionWindow(int num) {
        var container = FindObjectOfType<InstructionContainer>().contentPanel;
        var commands = container.GetComponentsInChildren<Command>();

        commands[num].GetComponent<UIControl>().Disable();
    }

    protected void DisableAllInInstructionInSolutionWindow() {
        var container = FindObjectOfType<InstructionContainer>().contentPanel;
        var commands = container.GetComponentsInChildren<Command>();
        foreach (var command in commands) {
            command.GetComponent<UIControl>().Disable();
        }
    }
}
