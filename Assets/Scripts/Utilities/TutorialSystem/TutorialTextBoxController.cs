using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialTextBoxLocation { 
    INSTRUCTION_CACHE
}

/**
 * @class TutorialTextBoxControler
 * @brief Used to allow custom Tutorial scripts to manipulate the text box
 *        used to display tutorial step prompt.
 */
public class TutorialTextBoxController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI TutorialText;

    protected void Awake() {
        Deactivate();
    }

    public void Activate() {
        GetComponent<Canvas>().enabled = true;
    }

    public void Deactivate() {
        GetComponent<Canvas>().enabled = false;
    }

    public void SetTutorialMessage(string message) {
        TutorialText.text = message;
    }

    public void SetBoxPosition(TutorialTextBoxLocation location) {
        throw new System.NotImplementedException();
    }
}
