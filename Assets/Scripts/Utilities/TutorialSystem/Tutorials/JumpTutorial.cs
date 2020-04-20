using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTutorial : InteractiveTutorial 
{
    protected override void Awake() {
        TutorialStep step1 = new TutorialStep();
        AddStep(step1);
        step1.Description = "Computron doesn't have legs, but it can still jump! " +
            "\nDrag a jump instruction into the solution window.";
        
        step1.AddBeginBehavior(
            () => {
                var controller = FindObjectOfType<UIController>();
                controller.FocusUIElement("JUMP");
                controller.FocusUIElement("SolutionWindow");
            }
        );

        step1.AddCompletionCondition(
            () => {
                var container = FindObjectOfType<InstructionContainer>();
                int numInPlay = container.Count;
                FocusInstruction(OpCode.JUMP);
                return (numInPlay > 0) && DragNDrop.CurrDragInstruction == null;
            }
        );

        TutorialStep step2 = new TutorialStep();
        step2.Description = "Fantas-tas-tas- *bzzrt* tastic. Let's break Computron and make an infinite loop!" +
            "\n\n Drag the jump anchor to be above the jump instruction!";

        AddStep(step2);

        step2.AddBeginBehavior(
            () => {
                DisableInstructionInSolutionWindow(0);
                uiController.FocusUIElement("SolutionWindow");
            }
        );

        step2.AddCompletionCondition(
           () => {
               var container = FindObjectOfType<InstructionContainer>().contentPanel;
               var commands = container.GetComponentsInChildren<Command>();

               return commands.Length > 0 && 
                    commands[0].Instruction == OpCode.NO_OP &&
                    DragNDrop.CurrDragInstruction == null;
           }
       );
        
        TutorialStep step3 = new TutorialStep();
        AddStep(step3);
        step3.Description = "Now, place an INPUT instruction between the JUMP instruction, and the unlabeled jump anchor. ";

        step3.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("SolutionWindow");
                DisableInstructionInSolutionWindow(0);
                DisableInstructionInSolutionWindow(1);
            }    
        );

        step3.AddCompletionCondition(
            () => {
                
                FocusInstruction(OpCode.INPUT);
                var container = FindObjectOfType<InstructionContainer>().contentPanel;
                var commands = container.GetComponentsInChildren<Command>();
                return commands.Length > 2 &&
                    commands[0].Instruction == OpCode.NO_OP &&
                    commands[1].Instruction == OpCode.INPUT && 
                    commands[2].Instruction == OpCode.JUMP &&
                    DragNDrop.CurrDragInstruction == null;
            }    
        );

        TutorialStep step4 = new TutorialStep();
        AddStep(step4);
        step4.Description = "Hit play, and notice how-how-how *bzzrt* the game will never end.";
        
        step4.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("PlayButton");
            }
        );

        step4.AddCompletionCondition(
            () => {
                return FindObjectOfType<Interpreter>().Running;
            }    
        );

        TutorialStep step5 = new TutorialStep();
        AddStep(step5);
        step5.Description = "When you get dizzy, hit HALT";
        step5.AddBeginBehavior(
            () => {
                uiController.ClearFocus();
                uiController.HighlightUIElement("HaltButton");
                TextBoxController.gameObject.transform.position += new Vector3(5, 0);
            }
        );
        step5.AddCompletionCondition(
            () => {
                return !FindObjectOfType<Interpreter>().Running;
            }
        );
        step5.AddEndBehavior(
            () => {
                TextBoxController.gameObject.transform.position += new Vector3(-5, 0);
            }    
        );

        TutorialStep step6 = new TutorialStep();
        AddStep(step6);
        step6.Description = "Now, let's tell Computron how to break the loop." +
            "\n\nDrag a JUMP IF NULL instruction into the solution window, and place it in between the INPUT and JUMP instructions.";
        //step6.EndDelay = 0.25f;
        step6.AddBeginBehavior(
            () => {
                FocusInstruction(OpCode.JUMP_IF_NULL);
                uiController.FocusUIElement("SolutionWindow");
                DisableInstructionInSolutionWindow(0);
                DisableInstructionInSolutionWindow(1);
                DisableInstructionInSolutionWindow(2);
            }
        );

        step6.AddCompletionCondition(
            () => {
                var container = FindObjectOfType<InstructionContainer>().contentPanel;
                var commands = container.GetComponentsInChildren<Command>();

                bool inputSeen = false;
                if (DragNDrop.CurrDragInstruction != null) {
                    return false;
                }

                for (int i = 0; i < commands.Length; i++) {
                    if (commands[i].Instruction == OpCode.INPUT) {
                        inputSeen = true;
                        continue;
                    }

                    if (commands[i].Instruction == OpCode.JUMP_IF_NULL) {
                        if (!inputSeen) {
                            return false;
                        }
                        else if (i + 1 < commands.Length && commands[i + 1].Instruction == OpCode.JUMP) {
                            return true;
                        }
                        else if (i + 2 < commands.Length && commands[i + 2].Instruction == OpCode.JUMP) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                }
                return false;
            }
        );

        TutorialStep step7 = new TutorialStep();
        AddStep(step7);
        step7.Description = "Now, take the jump anchor tied to the JUMP IF NULL instruction, " +
            "and drag it below the JUMP instruction";
        step7.AddBeginBehavior(
            () => {
                var container = FindObjectOfType<InstructionContainer>().contentPanel;
                var commands = container.GetComponentsInChildren<Command>();
                uiController.FocusUIElement("SolutionWindow");
                DisableAllInInstructionInSolutionWindow();
                foreach (var command in commands) {
                    if (command.Instruction == OpCode.JUMP_IF_NULL) {
                        var dndBehavior = command.GetComponent<JumpDragNDropBehavior>();
                        var anchor = dndBehavior.childAnchor;
                        var uiControl = anchor.GetComponent<UIControl>();
                        uiControl.Enable();
                        break;
                    }
                }
            }
        );

        step7.AddCompletionCondition(
            () => {
                var container = FindObjectOfType<InstructionContainer>().contentPanel;
                var commands = container.GetComponentsInChildren<Command>();
                return commands[commands.Length - 1].Instruction == OpCode.NO_OP &&
                    DragNDrop.CurrDragInstruction == null;
            }
        );

        TutorialStep step8 = new TutorialStep();
        AddStep(step8);
        step8.Description = "Now hit p-p-p-p-play, and observe how computron breaks out of that infinite loop!";

        step8.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("PlayButton");
            }
        );

        step8.AddCompletionCondition(() => { return FindObjectOfType<Interpreter>().Running; });

        TutorialStep step9 = new TutorialStep();
        AddStep(step9);
        step9.AddBeginBehavior(
            () => {
                TextBoxController.Deactivate();
                uiController.GetControllableUIElement("HaltButton").Disable();
            }    
        );

        step9.AddCompletionCondition(
            () => {
                return FindObjectOfType<Interpreter>().Halted;
            }    
        );

        step9.AddEndBehavior(
            () => {
                uiController.GetControllableUIElement("HaltButton").Enable();
            }    
        );

        TutorialStep step10 = new TutorialStep();
        AddStep(step10);
        step10.Description = "Whenever you're ready, hit *bzzzrt* HALT and solve the rest of the puzzle!";
        step10.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("HaltButton");
                uiController.HighlightUIElement("HaltButton");
            }
        );

        step10.AddCompletionCondition(
            () => {
                return !FindObjectOfType<Interpreter>().Running;
            }
        );
    }
}
