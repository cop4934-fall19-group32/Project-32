using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIControl : ControllableUIElement
{

	private Button controlButton;

	protected override void Awake()
	{
		base.Awake();

		ElementGraphic = GetComponent<Image>();
		controlButton = GetComponent<Button>();
	}

	protected void OnDestroy() {
		var controller = FindObjectOfType<UIController>();
		if (controller) { 
			controller.RemoveEntry(this);
		}
	}

	public override void Focus()
	{
		canvas.sortingLayerName = "Focus";
	}

	public override void Unfocus()
	{
		canvas.sortingLayerName = "Default";
	}


	public override void Enable()
	{
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;
	}

	public override void Disable()
	{
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.5f;
	}

}
