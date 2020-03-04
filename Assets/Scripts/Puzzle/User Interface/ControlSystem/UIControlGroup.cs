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
	protected override void Awake()
	{
		base.Awake();

		ElementGraphic = GetComponent<Image>();

	}

	public override void Focus()
	{
		GetChildControls();
		canvas.sortingLayerName = "Focus";
		foreach (var child in GetChildControls())
		{
			child.Focus();
		}
	}

	public override void Unfocus()
	{
		GetChildControls();
		canvas.sortingLayerName = "Default";
		foreach (var child in GetChildControls())
		{
			child.Unfocus();
		}
	}

	public override void Enable()
	{
		GetChildControls();
		foreach (var child in GetChildControls())
		{
			child.Enable();
		}
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;
	}

	public override void Disable()
	{
		foreach (var child in GetChildControls())
		{
			child.Disable();
		}
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.9f;
	}

	// Because child controls can be reparented through gameplay, ControlGroup should
	// ensure it gets a correct list of child controls. Setting on awake could lead to invalid operations
	private UIControl[] GetChildControls()
	{
		return GetComponentsInChildren<UIControl>();
	}
}
