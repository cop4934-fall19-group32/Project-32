using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ItemSlot : MonoBehaviour, IDropHandler
{

	// Dynamic item slots will be destroyed when item is not placed in them. For static slots, set to false.
	public bool staticSlot = false;
	public GameObject childObject = null;


	// Called when a draggable object is dropped into the current object
	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("OnDrop");

		if (eventData.pointerDrag != null)
		{
			GameObject pointerDrag = eventData.pointerDrag;

			// Prevent objects from being dropped into a slot that is already filled
			if (childObject != null && childObject != pointerDrag.gameObject)
			{
				pointerDrag.gameObject.GetComponent<DragNDrop>().objectInValidSlot = false;
				return;
			}
			else
			{
				DropItemInSlot(pointerDrag);
			}
		}
	}

	public void DropItemInSlot(GameObject pointerDrag)
	{
		childObject = pointerDrag.gameObject;
		var dndHandler = childObject.GetComponent<DragNDrop>();
		// Mark dragged object as being placed in a valid space so it doesn't destroy itself.
		dndHandler.objectInValidSlot = true;

		// Snap dragged object to center of the item slot
		pointerDrag.GetComponent<RectTransform>().transform.position = GetComponent<RectTransform>().transform.position;

		//Gather infor about current slot
		var activeItemSlot = dndHandler.currentItemSlot;
		var activeItemSlotHandler = (activeItemSlot != null) ? activeItemSlot.GetComponent<ItemSlot>() : null;

		// If the dragged object is dropped in a different item slot than the one it was previously in, then destroy the item slot it was previously in
		if (activeItemSlot == this.gameObject)
		{
			dndHandler.hasMoved = false;
		}
		else if (activeItemSlotHandler != null && activeItemSlotHandler.staticSlot != true)
		{
			// Destroy the old item slot
			activeItemSlotHandler.DestroyItemSlot();
		}

		// Update the the child's currentItemSlot to reference this item slot
		childObject.GetComponent<DragNDrop>().currentItemSlot = this.gameObject;
	}


	public void DestroyItemSlot()
	{
		Debug.Log("DestroyItemSlot");

		// Destroy item slot if it isn't marked as static
		if (staticSlot != true)
		{
			Destroy(gameObject);
		}
	}
}
