using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOTutorial : InteractiveTutorial
{

    protected override void Awake() {

        //Drag INPUT into SolutionWindow
        TutorialStep step1 = new TutorialStep();
        step1.Description =
            "Try out those fancy new instructions you *bzzrt* got!  " +
            "\nTo write a command, drag an INPUT instruction into the SOLUTION WINDOW.";
        
        step1.AddBeginBehavior(
            () => {
                var controller = FindObjectOfType<UIController>();
                controller.FocusUIElement("INPUT");
                controller.FocusUIElement("SolutionWindow");
            }
        );

        step1.AddCompletionCondition(
            () => {
                GameObject solutionWindow = GameObject.FindGameObjectWithTag("SolutionWindow");
                int numInPlay = solutionWindow.GetComponent<InstructionContainer>().Count;

                return (numInPlay > 0) && DragNDrop.CurrDragInstruction == null; 
            }
        );

        step1.AddEndBehavior(
            () => { FindObjectOfType<UIController>().ClearFocus();  }
        );

        AddStep(step1);

        TutorialStep step2 = new TutorialStep();
        step2.Description =
            "Press the play button to run your solution whenever you'd like." +
            "\n\nTry it now to see what happens when you do something wrong!";

        step2.AddBeginBehavior(
            () => {
                var controller = FindObjectOfType<UIController>();
                controller.FocusUIElement("PlayButton");
            }
        );

        step2.AddCompletionCondition(
            () => {
                return FindObjectOfType<Interpreter>().Running;
            }
        );

        step2.AddEndBehavior(
            () => { FindObjectOfType<UIController>().ClearFocus(); }
        );

        AddStep(step2);

    }
}
