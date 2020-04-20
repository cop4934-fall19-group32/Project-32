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
		//canvas.sortingLayerName = "UIControl";
	}

	public override void Focus()
	{
		canvas.overrideSorting = true;
		canvas.sortingLayerName = "Focus";
	}

	public override void Unfocus()
	{
		canvas.overrideSorting = false;
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
