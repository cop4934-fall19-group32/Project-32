using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardDragBehavior))]
public class CardUIControl : UIControl {
	private static int CardCount = 0;

	protected override void Awake() {
		base.Awake();
		ElementName = "Card" + CardCount++;
	}

	public override void Enable() {
		if (GetComponent<CardLogic>().BoundInstructions.Count != 0) {
			return;
		}

		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;
	}

	public override void Disable() {
		canvasGroup.alpha = 0.5f;
	}
}
