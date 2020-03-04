using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CardCommandDragNDrop))]
public class CardCommandUIControl : UIControl {
	public override void Enable() {
		var container = GameObject.FindGameObjectWithTag("RegisterContainer");
		if (container.GetComponent<CardContainer>().Count > 0) {
			base.Enable();
		}
		
	}
}
