using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @class ControlGroup
 * @brief Controllable UI element that forwards enable/disable 
 *        commands to all of the controls it contains.
 */
[RequireComponent(typeof(CanvasGroup))]
public class UIControlGroup : ControllableUIElement
{
    /** Dynamically populated list of child control objects */
    private UIControl[] ChildControls;
    private CanvasGroup Group;

    private void Start() 
    {
        ChildControls = GetComponentsInChildren<UIControl>();
        ElementGraphic = GetComponent<Image>();
        Group = GetComponent<CanvasGroup>();
    }

    public override void Enable() {
        RefreshChildControls();
        foreach (var child in ChildControls) {
            child.Enable();
        }

        Group.interactable = true;
        Group.blocksRaycasts = true;
    }

    public override void Disable() {
        RefreshChildControls();
        foreach (var child in ChildControls) {
            child.Disable();
        }

        Group.interactable = false;
        Group.blocksRaycasts = false;
    }

    private void RefreshChildControls() {
        ChildControls = GetComponentsInChildren<UIControl>();
    }
}
