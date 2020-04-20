using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayTutorialHandler : MonoBehaviour
{
    private void Awake() {
        if (FindObjectOfType<InteractiveTutorial>() == null) {
            Destroy(gameObject);
        }
    }
    public void ReplayTutorial() {
        var tutorial = FindObjectOfType<InteractiveTutorial>();
        if (tutorial != null) {
            StartCoroutine(tutorial.RunTutorial());
        }
    }
}
