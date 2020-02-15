using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpDragNDropBehavior : DragNDrop {
	public GameObject AnchorPrefab;

	public GameObject childAnchor { get; set; }

	private Command instruction;

	private JumpLineDrawer jumpLineDrawer;

	protected override void Awake() 
	{
		base.Awake();
		childAnchor = null;
		instruction = GetComponent<Command>();
		jumpLineDrawer = GetComponent<JumpLineDrawer>();
		jumpLineDrawer.instructionTransform = GetComponent<RectTransform>();
		jumpLineDrawer.anchorTransform = null;
		StartCoroutine(jumpLineDrawer.DrawJumpLine());
	}

	public void Start() 
	{
		
	}

	public void Update() 
	{
		if (childAnchor == null) {
			return;
		}

		instruction.Arg = (uint)childAnchor.transform.GetSiblingIndex();
	}

	public override void OnEndDrag(PointerEventData eventData) {
		base.OnEndDrag(eventData);
		if (childAnchor == null && dragTargetValid) {
			SpawnAnchor();
		}
	}

	public void AttachAnchor(GameObject anchor) {
		childAnchor = anchor;
		childAnchor.GetComponent<AnchorDragNDropBehavior>().activeDynamicScrollView = activeDynamicScrollView;
		GetComponent<JumpLineDrawer>().anchorTransform = childAnchor.GetComponent<RectTransform>();
	}

	private void SpawnAnchor() {
		//Spawn anchor
		childAnchor = Instantiate(AnchorPrefab, transform.parent);
		childAnchor.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
		childAnchor.GetComponent<AnchorDragNDropBehavior>().activeDynamicScrollView = activeDynamicScrollView;
		
		//Configure line drawer and kick off draw coroutine
		GetComponent<JumpLineDrawer>().anchorTransform = childAnchor.GetComponent<RectTransform>();
	}

	protected override void HandleInvalidDrop() {
		base.HandleInvalidDrop();

		if (childAnchor != null) {
			Destroy(childAnchor);
		}
	}

}
