using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : ControllableUIElement
{

    private Button ControlButton;

    private void Awake() 
    {
        ElementGraphic = GetComponent<Image>();
        ControlButton = GetComponent<Button>();
    }

    private void Update() {
        
    }

    public override void Enable() 
    {
        if (ControlButton) {
            ControlButton.interactable = true;
        }
    }

    public override void Disable() 
    {
        if (ControlButton) { 
            ControlButton.interactable = false;
        }
    }

}
