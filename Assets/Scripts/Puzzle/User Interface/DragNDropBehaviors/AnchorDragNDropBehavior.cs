using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnchorDragNDropBehavior : DragNDrop, IPointerClickHandler
{
	public GameObject JumpTarget;
	public JumpLineDrawer lineDrawer { private get; set; }
	private int OldSiblingIndex;
	private Transform SourceContainer;
	public Image pointerGraphic;

	public void Start() {

	}

	public override void OnBeginDrag(PointerEventData eventData) {
		JumpLineDrawer.DeactivateAll();
		lineDrawer.Active = true;
		OldSiblingIndex = transform.GetSiblingIndex();
		SourceContainer = transform.parent;
		base.OnBeginDrag(eventData);
	}

	protected override void HandleInvalidDrop() {
		transform.SetParent(SourceContainer, false);
		transform.SetSiblingIndex(OldSiblingIndex);
	}

	public void HighlightArrow(bool highlight) {
		if (highlight) {
			var color = new Color(pointerGraphic.color.r, pointerGraphic.color.g, pointerGraphic.color.b);
			pointerGraphic.color = color;
		}
		else {
			var color = new Color(pointerGraphic.color.r, pointerGraphic.color.g, pointerGraphic.color.b);
			color.a = 0.5f;
			pointerGraphic.color = color;
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		JumpLineDrawer.DeactivateAll();
		lineDrawer.Active = true;
	}
}
