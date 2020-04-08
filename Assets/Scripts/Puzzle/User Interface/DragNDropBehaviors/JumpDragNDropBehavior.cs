using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(JumpLineDrawer))]
public class JumpDragNDropBehavior : DragNDrop {
	
	public GameObject JumpTarget;
	public GameObject childAnchor { get; set; }

	private Command instruction;

	private JumpLineDrawer jumpLineDrawer;

	protected override void Awake() 
	{
		base.Awake();
		childAnchor = null;
		instruction = GetComponent<Command>();
		jumpLineDrawer = GetComponent<JumpLineDrawer>();
		jumpLineDrawer.instructionTransform = JumpTarget.GetComponent<RectTransform>();
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

		instruction.Target = (uint)childAnchor.transform.GetSiblingIndex();
	}

	public override void OnBeginDrag(PointerEventData eventData) {
		base.OnBeginDrag(eventData);
		if (childAnchor != null) {
			var anchorBehavior = childAnchor.GetComponent<AnchorDragNDropBehavior>();
			anchorBehavior.HighlightArrow(true);
		}
	}

	public override void OnEndDrag(PointerEventData eventData) {
		base.OnEndDrag(eventData);
		if (childAnchor != null) {
			var anchorBehavior = childAnchor.GetComponent<AnchorDragNDropBehavior>();
			anchorBehavior.HighlightArrow(false);
		}
		if (childAnchor == null && dragTargetValid) {
			SpawnAnchor();
		}
	}

	public void AttachAnchor(GameObject anchor) {
		childAnchor = anchor;
		childAnchor.GetComponent<AnchorDragNDropBehavior>().activeDynamicScrollView = 
			activeDynamicScrollView;
		jumpLineDrawer.anchorTransform = 
			childAnchor.GetComponent<AnchorDragNDropBehavior>().JumpTarget.GetComponent<RectTransform>();
	}

	private void SpawnAnchor() {
		//Spawn anchor
		childAnchor = Instantiate(FindObjectOfType<InstructionFactory>().JumpAnchorPrefab, transform.parent);
		AttachAnchor(childAnchor);
		childAnchor.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
	}

	protected override void HandleInvalidDrop() {
		base.HandleInvalidDrop();

		if (childAnchor != null) {
			Destroy(childAnchor);
		}
	}

}
