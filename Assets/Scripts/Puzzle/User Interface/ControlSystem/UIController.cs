using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    private Dictionary<string, ControllableUIElement> ControllableUIElements;

    /**
     * Scans scene hierarchy for controllable child components
     */
    private void Awake() {
        ControllableUIElements = new Dictionary<string, ControllableUIElement>();
        var controlsList = FindObjectsOfType<ControllableUIElement>();
        foreach (var control in controlsList) {
            if (ControllableUIElements.ContainsKey(control.ElementName)) {
                Debug.LogError(
                    "UIController encountered duplicate control name: "
                    + control.ElementName
                    + ". Plase fix your shit"
                );
            }

            ControllableUIElements.Add(control.ElementName, control);
        }
    }

    /**
     * Enables a ControllableUIElement with the matching name
     * @return Whether the element was found and enabled
     */
    public bool EnableUIElement(string elementName) {
        if (!ControllableUIElements.ContainsKey(elementName)) {
            Debug.LogWarning("Element " + elementName + " not found. Enable failed");
            return false;
        }

        ControllableUIElements[elementName].Enable();
        return true;
    }

    public void EnableAll() {
        foreach (var entry in ControllableUIElements) {
            entry.Value.Enable();
        }
    }

    /**
     * Disables a ControllableUIElement with the matching name
     * @return Whether the element was found and disabled
     */
    public bool DisableUIElement(string elementName) {
        if (!ControllableUIElements.ContainsKey(elementName)) {
            Debug.LogWarning("Element " + elementName + " not found. Disable failed");
            return false;
        }

        ControllableUIElements[elementName].Disable();
        return true;
    }

    public void DisableAll() {
        foreach (var entry in ControllableUIElements) {
            entry.Value.Disable();
        }
    }

    public bool FocusUIElement(string elementName) {
        throw new System.NotImplementedException();
    }

    public void ClearFocus() {
        throw new System.NotImplementedException();
    }

    public bool HighlightUIElement(string elementName) {
        if (!ControllableUIElements.ContainsKey(elementName)) {
            Debug.LogWarning("Element " + elementName + " not found. Highlight failed");
            return false;
        }

        ControllableUIElements[elementName].StartHighlight();

        return true;
    }

    public bool StopHighlightUIElement(string elementName) {
        if (!ControllableUIElements.ContainsKey(elementName)) {
            Debug.LogWarning("Element " + elementName + " not found. Highlight termination failed");
            return false;
        }

        ControllableUIElements[elementName].StopHighlight();

        return true;
    }

    public GameObject GetElement(string elementName) {
        if (!ControllableUIElements.ContainsKey(elementName)) {
            return null;
        }

        return ControllableUIElements[elementName].gameObject;
    }

}
