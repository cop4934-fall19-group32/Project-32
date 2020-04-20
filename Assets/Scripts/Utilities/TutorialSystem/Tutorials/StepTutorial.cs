using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepTutorial : InteractiveTutorial {
    // Start is called before the first frame update
    protected override void Awake() {
        var step1 = new TutorialStep();
        AddStep(step1);
        step1.Description = "There's still some things you should know!" +
            "\n\nBefore we get st-st-arted, drag two INPUT instructions into the Solution Window.";
        step1.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("SolutionWindow");
            }    
        );
        step1.AddCompletionCondition(
            () => {
                FocusInstruction(OpCode.INPUT);
                var container = FindObjectOfType<InstructionContainer>();
                int numInPlay = container.Count;
                return (numInPlay > 2) && DragNDrop.CurrDragInstruction == null;
            }
        );

        var step2 = new TutorialStep();
        AddStep(step2);
        step2.Description = "Fantastic! Did you know that Computrons know how to step?" +
            "\n\nHit that big blue step button to make Computron execute a single instuction";
        step2.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("StepButton");
            }
        );

        step2.AddCompletionCondition(
            () => {
                var actor = FindObjectOfType<Actor>();
                return actor.step == true; 
            }
        );



        var step3 = new TutorialStep();
        AddStep(step3);
        step3.Description = "Go ahead and give the button another thwak";
        step3.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("StepButton");
                var actor = FindObjectOfType<Actor>();
                //fuckin cheat
                actor.step = false;
            }
        );
        step3.AddCompletionCondition(
            () => {
                var actor = FindObjectOfType<Actor>();
                return actor.step == true;
            }
        );
        step3.AddEndBehavior(
            () => {
                
            }
        );

        var step4 = new TutorialStep();
        AddStep(step4);
        step4.Description = "One more fun fact, Computrons love to be helpful. " +
            "\n\nClick on yours during any level and receive a fun fact! Give it a try now!";
        step4.AddBeginBehavior(
            () => {
                TextBoxController.gameObject.transform.position += new Vector3(5, 0);
            }
        );
        step4.AddCompletionCondition(
            () => {
                return GameObject.FindGameObjectWithTag("HintBox") != null;
            }
        );
        step4.AddEndBehavior(
            () => {
                TextBoxController.gameObject.transform.position -= new Vector3(5, 0);
            }
        );
    }

}
