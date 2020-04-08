using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractiveTutorial : MonoBehaviour
{
    public TutorialTextBoxController TextBoxController;

    public List<TutorialStep> Steps { get; private set; } = new List<TutorialStep>();

    /**
     * Function overriden in subclasses to initialize Steps
     */
    protected abstract void Awake();

    protected void AddStep(TutorialStep step) {
        Steps.Add(step);
    }

    public IEnumerator RunTutorial() {
        var textbox = GameObject.FindGameObjectWithTag("TutorialTextBox");
        TextBoxController = textbox.GetComponent<TutorialTextBoxController>();
        TextBoxController.Activate();
        foreach (var step in Steps) {
            TextBoxController.SetTutorialMessage(step.Description);
            step.Begin();

            while (!step.Complete) {
                yield return null;
            }

            step.End();
        }
        TextBoxController.Deactivate();
    }

}
