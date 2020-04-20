using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTutorial : InteractiveTutorial {
    protected override void Awake() {
        var step1 = new TutorialStep();
        AddStep(step1);
        step1.Description = "You-you-you got Memory Cards! Computrons are very forgetful, so memory cards allow them " +
            "to remember more than one number at a time." +
            "\n\n Drag both of the cards to the panel below to put them in play!";
        step1.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("RegisterHandSlot");
                uiController.HighlightUIElement("RegisterHandSlot");
                uiController.FocusUIElement("CardPlayArea");
                uiController.HighlightUIElement("CardPlayArea");
            }
        );

        step1.AddCompletionCondition(
            () => {
                return FindObjectOfType<PlayedCardContainer>().Count > 1;
            }
        );

        var step2 = new TutorialStep();
        AddStep(step2);
        step2.Description = "To make use of those *bbbzrt* cards, you need to use the right instruction!" +
            "\n\nDrag a MOVE_TO instruction into the solution window.";
        step2.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("SolutionWindow");
                FocusInstruction(OpCode.MOVE_TO);
            }
        );
        step2.AddCompletionCondition(
            () => {
                var container = FindObjectOfType<InstructionContainer>();
                int numInPlay = container.Count;
                FocusInstruction(OpCode.MOVE_TO);
                return (numInPlay > 0) && DragNDrop.CurrDragInstruction == null;
            }    
        );

        var step3 = new TutorialStep();
        AddStep(step3);
        step3.Description = "Whenev-ev-ev-ever you play a card-based instruction, you must link it to a card that you played." +
            "\n\nClick on one of the cards you played to link your instruction to it!";

        step3.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("CardPlayArea");
                var cards = FindObjectOfType<PlayedCardContainer>().GetCards();
                foreach (var card in cards) {
                    card.GetComponent<UIControl>().Disable();
                }
            }
        );
        step3.AddCompletionCondition(
            () => {
                return FindObjectOfType<CardInstructionLinker>() == null;
            }
        );
        step3.AddEndBehavior(
            () => {
                var cards = FindObjectOfType<PlayedCardContainer>().GetCards();
                foreach (var card in cards) {
                    card.GetComponent<UIControl>().Enable();
                }
            }    
        );

        var step4 = new TutorialStep();
        AddStep(step4);
        step4.Description = "If you ever make an ***ERROR***, you can always click on a card instruction's argument " +
            "to relink that instruction." +
            "\n\n Give it a try!";

        GameObject boundCard = null;
        CardCommandDragNDrop boundInstruction = null;
        step4.AddBeginBehavior(
            () => {
                
                var container = FindObjectOfType<InstructionContainer>().contentPanel;
                var commands = container.GetComponentsInChildren<Command>();
                boundInstruction = commands[0].GetComponent<CardCommandDragNDrop>();
                var label = boundInstruction.ArgLabel;
                var labelUIControl = label.GetComponent<UIControl>();
                labelUIControl.Enable();
                uiController.FocusUIElement(labelUIControl);
                labelUIControl.StartHighlight();
            }
        );
        step4.AddCompletionCondition(() => { return FindObjectOfType<CardInstructionLinker>() != null; });
        step4.AddEndBehavior(
            () => {
                boundCard = boundInstruction.BoundCard;
            }    
        );

        var step5 = new TutorialStep();
        AddStep(step5);
        step5.Description = "Select another card to relink the MOVE_TO instruction";

        step5.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("CardPlayArea");
                uiController.FocusUIElement("RegisterHandSlot");
            }
        );
        step5.AddCompletionCondition(
            () => {
                return FindObjectOfType<CardInstructionLinker>() == null;
            }
        );
        step5.AddEndBehavior(
            () => {
                var cards = FindObjectOfType<PlayedCardContainer>().GetCards();
                foreach (var card in cards) {
                    card.GetComponent<UIControl>().Enable();
                }
            }
        );

        var step6 = new TutorialStep();
        AddStep(step6);
        step6.Description = "Now that you have your act together, let's see that memory card in action. " +
            "\n\nPlay an INPUT instruction, and place it above the MOVE_TO";
        step6.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("SolutionWindow");
                DisableInstructionInSolutionWindow(0);
            }    
        );

        step6.AddCompletionCondition(
            () => {
                FocusInstruction(OpCode.INPUT);
                var container = FindObjectOfType<InstructionContainer>().contentPanel;
                var commands = container.GetComponentsInChildren<Command>();
                return commands[0].Instruction == OpCode.INPUT && DragNDrop.CurrDragInstruction == null;
            }
        );

        TutorialStep step7 = new TutorialStep();
        AddStep(step7);
        step7.Description = "Hit play, and watch *bzzzrt* the magic happen.";

        step7.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("PlayButton");
            }
        );

        step7.AddCompletionCondition(
            () => {
                return FindObjectOfType<Interpreter>().Running;
            }
        );
    }

}