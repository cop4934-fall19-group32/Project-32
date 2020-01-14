using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class DragNDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

	// Store a reference to the item slot that the button is currently held in
	public GameObject currentItemSlot;

	// If gameobject is clonable, it will leave a copy of itself in it's place after being dragged away.
	public bool clonable;

	[HideInInspector]
	// Reference the dynamic scroll view that the object is currently in.
	public DynamicScrollView dynamicScrollView;

	// Track if object is dropped outside of any valid spaces.
	[HideInInspector]
	public bool objectInValidSlot { get; set; }

	// Track if object is in the same position it was in before being dragged
	[HideInInspector]
	public bool hasMoved { get; set; }

	// Must reference (in Unity editor) the canvas that this item is in
	[SerializeField] public Canvas canvas;

	private RectTransform rectTransform;

	// Gameobject being dragged must contain a Canvas Group component
	private CanvasGroup canvasGroup;

	// If the button clones itself, store a reference to the duplicate.
	private GameObject clone;

	protected virtual void Awake()
	{
		Debug.Log("Awake");
		hasMoved = false;
		objectInValidSlot = true;

		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();

		var buttonText = GetComponentInChildren<TextMeshProUGUI>();
		buttonText.SetText(GetComponent<Command>().Instruction.ToString());

	}


	// Called when pressing mouse down on object
	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("OnPointerDown");
		hasMoved = false;
	}

	// Called when beginning to drag object
	public void OnBeginDrag(PointerEventData eventData)
	{
		Debug.Log("OnBeginDrag");

		// When dragging the object from its original position, create a hidden clone of the object.
		// On end of drag, the object will either be dropped elsewhere or destroyed, and the clone will appear.
		// This is useful for objects that should respawn, such as items in the instruction set that may be used multiple times.
		if(clonable)
		{
			clone = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);
			clone.SetActive(false);
		}

		hasMoved = true;
		objectInValidSlot = false;
		canvasGroup.alpha = .8f;

		// raycast should go through object and land on item slot
		canvasGroup.blocksRaycasts = false;

		transform.SetParent(canvas.transform);

		currentItemSlot.GetComponent<ItemSlot>().childObject = null;
		DestroyItemSlot(currentItemSlot);
	}

	// Called once per frame while dragging object
	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

		if (dynamicScrollView != null)
		{
			dynamicScrollView.GetComponent<DynamicScrollView>().UpdateScrollView(eventData);

			if (dynamicScrollView.GetComponent<DynamicScrollView>().newItemSlot == null)
			{
				dynamicScrollView.GetComponent<DynamicScrollView>().SpawnItemSlot();
			}
		}
	}

	// Called when ending the dragging of an object
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		Debug.Log("OnEndDrag");
		canvasGroup.alpha = 1f;

		// raycast should land on item so it can be dragged again
		canvasGroup.blocksRaycasts = true;

		// If the object is dragged from one item slot and dropped in a different slot, then set the new slot as the object's parent.
		// If the object is dropped outside of a valid item slot, then destroy the slot it was previously in.
		if (hasMoved)
		{
			if (objectInValidSlot)
			{
				this.transform.SetParent(currentItemSlot.transform);
			}
			else
			{
				DestroyItemSlot(currentItemSlot);
				DestroySelf();
			}
		}

		// Removed empty item slots that were spawned but not used
		if (dynamicScrollView != null)
		{
			dynamicScrollView.GetComponent<DynamicScrollView>().DespawnItemSlot();
		}

		if (clone != null)
		{
			clone.SetActive(true);
			clonable = false;
		}
	}

	// Destroy this gameobject
	public void DestroySelf()
	{
		Debug.Log("DestroySelf");
		Destroy(gameObject);
	}


	// Destroy the item slot the gameobject was in before being dragged
	public void DestroyItemSlot(GameObject itemSlot)
	{
		// Destroy item slot if it isn't marked as static
		if (itemSlot != null && itemSlot.GetComponent<ItemSlot>().staticSlot != true)
		{
			itemSlot.GetComponent<ItemSlot>().DestroyItemSlot();
			// Destroy(itemSlot);
			itemSlot = null;
		}

	}


}
