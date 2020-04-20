using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitTutorialHandler : MonoBehaviour
{
    private Canvas canvas;

    private void Awake() {
        if (FindObjectOfType<InteractiveTutorial>() == null) {
            Destroy(gameObject);
        }
        else {
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;

        }
    }
    
    public void Update() {
        canvas.overrideSorting = true;
        canvas.sortingOrder = 9999;
    }

    public void Show() {
        canvas.enabled = true;
        canvas.overrideSorting = true;
    }

    public void QuitTutorial() {
        var tutorial = FindObjectOfType<InteractiveTutorial>();
        if (tutorial != null) {
            tutorial.StopTutorial();
            FindObjectOfType<UIController>().ClearFocus();
            var textbox = GameObject.FindGameObjectWithTag("TutorialTextBox");
            var textBoxController = textbox.GetComponent<TutorialTextBoxController>();
            textBoxController.Deactivate();
            GetComponent<Canvas>().enabled = false;
        }
    }
}
