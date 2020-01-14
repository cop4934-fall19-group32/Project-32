using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// This script should be attached to the high level "Dynamic Scroll View" game object.



public class DynamicScrollView : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{


	// Specify the Content gameobject of the scrollview.
	[SerializeField] public GameObject contentPanel;


	// Allow draggable buttons to be dropped into the content window during runtime (true for solution window, false for instruction set window)
	[SerializeField] private bool acceptNewItems = true;


	// Store a reference to a blank item slot that the button can duplicate.
	[SerializeField] private GameObject ItemSlotTemplate;

	// Store a reference to a new item slot that gets spawned
	public GameObject newItemSlot = null;


	// Initialize components at start
	public void Start()
	{
		foreach (Transform itemSlot in contentPanel.transform)
		{
			if (itemSlot.childCount > 0)
			{
				itemSlot.gameObject.GetComponent<ItemSlot>().childObject = itemSlot.GetChild(0).gameObject;
				itemSlot.GetChild(0).gameObject.GetComponent<DragNDrop>().currentItemSlot = itemSlot.gameObject;
				itemSlot.GetChild(0).gameObject.GetComponent<DragNDrop>().dynamicScrollView = this;
			}
			else
			{
				itemSlot.gameObject.GetComponent<ItemSlot>().childObject = null;
			}
		}
	}

	public void OnDrop(PointerEventData eventData)
	{
		Debug.Log("object dropped in dynamic scroll view");

		// If an object is dropped into the dynamic scroll view, but NOT into an item slot, place the object into the empty slot that was spawned.
		if (eventData.pointerDrag != null && acceptNewItems)
		{
			newItemSlot.GetComponent<ItemSlot>().DropItemInSlot(eventData.pointerDrag);
			newItemSlot = null;
		}
	}


	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("OnPointerEnter dynamic scroll view");

		if (eventData.pointerDrag != null)
		{
			// Debug.Log("Dragging object");

			eventData.pointerDrag.gameObject.GetComponent<DragNDrop>().dynamicScrollView = this;

			if (acceptNewItems)
			{
				SpawnItemSlot();
			}

		}
	}


	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("OnPointerExit dynamic scroll view");

		if (eventData.pointerDrag != null)
		{
			eventData.pointerDrag.gameObject.GetComponent<DragNDrop>().dynamicScrollView = null;
			DespawnItemSlot();
		}
	}


	public void SpawnItemSlot()
	{
		// Create a new empty item slot in the scroll view content
		newItemSlot = Object.Instantiate(ItemSlotTemplate, ItemSlotTemplate.transform.position, ItemSlotTemplate.transform.rotation, ItemSlotTemplate.transform.parent);
		newItemSlot.SetActive(true);
	}


	// Destroy the spawned item slot if it isn't used
	public void DespawnItemSlot()
	{
		if (newItemSlot != null && newItemSlot.GetComponent<ItemSlot>().childObject == null)
		{
			newItemSlot.GetComponent<ItemSlot>().DestroyItemSlot();
		}
		newItemSlot = null;
	}


	public void UpdateScrollView(PointerEventData eventData)
	{
		// Start with newItemSlot at the last hierarchy index in the scroll view content.
		// As the button is dragged, check if the button is higher or lower than the new item slot
		// while button is higher than the button above the empty slot, move the empty slot up.
		// while button is lower than the button below the empty slot, move the empty slot down.

		// foreach (Transform sibling in contentPanel.transform)
		// {
		// 	if (sibling.gameObject != newItemSlot && sibling.gameObject.activeInHierarchy && sibling.gameObject.GetComponent<ItemSlot>().childObject == null)
		// 	{
		// 		sibling.gameObject.GetComponent<ItemSlot>().DestroyItemSlot();
		// 	}
		// }


		Vector2 pointerPosition = eventData.position;

		foreach (Transform sibling in contentPanel.transform)
		{
			if(sibling != null && sibling.gameObject != newItemSlot)
			{
				// Clean up empty item slots
				if (sibling.gameObject.activeInHierarchy && sibling.gameObject.GetComponent<ItemSlot>().childObject == null)
				{
					sibling.gameObject.GetComponent<ItemSlot>().DestroyItemSlot();
				}

				// If the button is being dragged above the sibling, but the new item slot is below the sibling (higher index), move the new item slot up.
				else if (pointerPosition.y > sibling.transform.position.y
					&& newItemSlot.transform.GetSiblingIndex() > sibling.transform.GetSiblingIndex())
				{
					Debug.Log("move empty item slot up");
					newItemSlot.transform.SetSiblingIndex(sibling.transform.GetSiblingIndex());
				}
				// If the button is being dragged below the sibling, but the new item slot is above the sibling (lower index), move the new item slot down.
				else if (pointerPosition.y <= sibling.transform.position.y
					&& newItemSlot.transform.GetSiblingIndex() < sibling.transform.GetSiblingIndex())
				{
					Debug.Log("move empty item slot down");
					newItemSlot.transform.SetSiblingIndex(sibling.transform.GetSiblingIndex());
				}
				// else if (sibling != null)
				// {
				// 	if (!(pointerPosition.y > sibling.transform.position.y))
				// 	{
				// 		if (newItemSlot != null && newItemSlot.transform.GetSiblingIndex() < sibling.GetSiblingIndex())
				// 		{
				// 			Debug.Log("move empty item slot down");
				// 			newItemSlot.transform.SetSiblingIndex(sibling.transform.GetSiblingIndex());
				// 		}
				// 	}
				// }

			}
		}
	}
}
