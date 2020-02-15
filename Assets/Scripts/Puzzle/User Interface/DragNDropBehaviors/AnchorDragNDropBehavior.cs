using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorDragNDropBehavior : DragNDrop
{
	private int OldSiblingIndex;
	private Transform SourceContainer;
	public void Start() {

	}

	public override void OnBeginDrag(PointerEventData eventData) {
		OldSiblingIndex = transform.GetSiblingIndex();
		SourceContainer = transform.parent;
		base.OnBeginDrag(eventData);
	}

	protected override void HandleInvalidDrop() {
		transform.SetParent(SourceContainer, false);
		transform.SetSiblingIndex(OldSiblingIndex);
	}
}
