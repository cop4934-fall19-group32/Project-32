using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpDragNDropBehavior : DragNDrop
{
	public GameObject AnchorPrefab;

	public GameObject ChildAnchor = null;
	
	protected override void Awake() {
		base.Awake();
		//AnchorPrefab.SetActive(false);	
	}

	public override void OnEndDrag(PointerEventData eventData) 
	{
		base.OnEndDrag(eventData);
		Debug.Log("Spawning Jump Anchor");
		if (ChildAnchor == null) 
		{
			var anchorSlot = Instantiate(AnchorPrefab, transform.parent.parent);
			ChildAnchor = anchorSlot.transform.GetChild(0).gameObject;
			ChildAnchor.GetComponent<AnchorDragNDropBehavior>().JumpCmdRef = GetComponent<Command>();

			anchorSlot.GetComponentInChildren<DragNDrop>().canvas = canvas;
			anchorSlot.transform.SetSiblingIndex(gameObject.transform.parent.GetSiblingIndex() + 1);
			anchorSlot.GetComponent<ItemSlot>().DropItemInSlot(ChildAnchor.transform.GetChild(0).gameObject);
		}
	}

	public void Update() {
		var CurveDrawer = GetComponent<BezierCurveDrawer>();
		if (ChildAnchor == null) {
			CurveDrawer.A = new Vector3();
			CurveDrawer.B = new Vector3();
			CurveDrawer.C = new Vector3();
			return;
		}

		CurveDrawer.A = transform.position;
		CurveDrawer.B = transform.position + new Vector3(-100, 0, 0);
		CurveDrawer.C = ChildAnchor.transform.position;
	}
}
