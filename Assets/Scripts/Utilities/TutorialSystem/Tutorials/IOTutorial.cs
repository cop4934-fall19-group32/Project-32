using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOTutorial : InteractiveTutorial
{

    protected override void Awake() {

        //Drag INPUT into SolutionWindow
        TutorialStep step1 = new TutorialStep();
        AddStep(step1);
        step1.Description =
            "Try out those fancy new instructions you *bzzrt* got!  " +
            "\nTo write a command, drag an INPUT instruction into the SOLUTION WINDOW.";
        
        step1.AddBeginBehavior(
            () => {
                var controller = FindObjectOfType<UIController>();
                FocusInstruction(OpCode.INPUT);
                controller.FocusUIElement("SolutionWindow");
            }
        );

        step1.AddCompletionCondition(
            () => {
                GameObject solutionWindow = GameObject.FindGameObjectWithTag("SolutionWindow");
                int numInPlay = solutionWindow.GetComponent<InstructionContainer>().Count;

                var instructionCache = GameObject.FindGameObjectWithTag("InstructionCacheContent");
                instructionCache.GetComponentInChildren<UIControl>().Focus();
                return (numInPlay > 0) && DragNDrop.CurrDragInstruction == null; 
            }
        );

        TutorialStep step2 = new TutorialStep();
        step2.Description =
            "Press the play button to run your solution whenever you'd like." +
            "\n\nTry it now to see what happens *bzzrt* when you do something wrong!";

        step2.AddBeginBehavior(
            () => {
                uiController.FocusUIElement("PlayButton");
            }
        );

        step2.AddCompletionCondition(
            () => {
                return FindObjectOfType<Interpreter>().Running;
            }
        );

        AddStep(step2);

        TutorialStep step3 = new TutorialStep();
        AddStep(step3);
        step3.AddBeginBehavior(
            () => {
                FindObjectOfType<TutorialTextBoxController>().Deactivate();
            }
        );

        step3.AddCompletionCondition(
            () => {
                return FindObjectOfType<InputBox>().Count < 1;
            }
        );

        step3.AddEndBehavior(
            () => {
                FindObjectOfType<TutorialTextBoxController>().Activate();
            }
        );
        step3.EndDelay = 1.0f;


        TutorialStep step4 = new TutorialStep();
        AddStep(step4);
        step4.Description = "When Computron runs out *bzzrt* of instructions, your output is analyzed." +
            "\n\nNow if you're ready to get to work, hit the halt button to stop the simulation and try again!";

        step4.AddBeginBehavior(
            () => {
                var controller = FindObjectOfType<UIController>();
                controller.FocusUIElement("SolutionWindow");
                controller.FocusUIElement("HaltButton");
                TextBoxController.transform.position += new Vector3(5, 0);
            }
        );

        step4.AddCompletionCondition(
            () => {
                return !FindObjectOfType<Interpreter>().Running;
            }
        );

        step4.AddEndBehavior(
            () => {
                TextBoxController.transform.position -= new Vector3(5, 0);
            }
        );

    }
}
