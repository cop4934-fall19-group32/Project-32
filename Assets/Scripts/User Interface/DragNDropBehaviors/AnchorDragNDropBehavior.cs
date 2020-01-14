using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorDragNDropBehavior : DragNDrop
{
	public Command JumpCmdRef;

	public void Start() {

	}

	public void Update() {
		uint index = (uint)transform.parent.GetSiblingIndex();
		JumpCmdRef.Arg = index;
	}

	public override void OnEndDrag(PointerEventData eventData) {
		base.OnEndDrag(eventData);		
	}


}
