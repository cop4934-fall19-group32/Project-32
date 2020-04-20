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
	public UIControl[] ChildControls { get { return GetComponentsInChildren<UIControl>(); } }
	protected override void Awake()
	{
		base.Awake();

		ElementGraphic = GetComponent<Image>();
		canvas.sortingLayerName = "UIControlGroup";
	}

	public override void Focus()
	{
		canvas.overrideSorting = true;
		canvas.sortingLayerName = "Focus";
		foreach (var child in ChildControls)
		{
			child.Focus();
		}
	}

	public override void Unfocus()
	{
		canvas.overrideSorting = false;
		foreach (var child in ChildControls)
		{
			child.Unfocus();
		}
	}

	public override void Enable()
	{
		foreach (var child in ChildControls)
		{
			child.Enable();
		}
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;
	}

	public override void Disable()
	{
		foreach (var child in ChildControls)
		{
			child.Disable();
		}
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.9f;
	}

	public override void StartHighlight() {
		foreach (var child in ChildControls) {
			child.StartHighlight();
		}
	}

	public override void StopHighlight() {
		foreach (var child in ChildControls) {
			child.StopHighlight();
		}
	}

}
