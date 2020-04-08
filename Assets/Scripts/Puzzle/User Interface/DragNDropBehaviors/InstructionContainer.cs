using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// This script should be attached to the high level "Dynamic Scroll View" game object.


public class InstructionContainer : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	// Specify the Content gameobject of the scrollview.
	public GameObject contentPanel { get; set; }

	// Allow draggable buttons to be dropped into the content window during runtime (true for solution window, false for instruction set window)
	[SerializeField] private bool acceptsNewItems = true;

	public int Count { 
		get {
			return contentPanel.transform.childCount; 
		} 
	}

	// Store a reference to a new item slot that gets spawned
	public GameObject temporaryButtonSlot { get; set; }

	// Store a reference to the the panel that will be used to disable
	// private GameObject UILock;
	private bool locked = false;


	protected virtual void Awake()
	{
		temporaryButtonSlot = null;
		contentPanel = transform.Find("Scroll View/Viewport/Content").gameObject;
	}


	protected virtual void Start()
	{
		foreach (Transform content in contentPanel.transform)
		{
			if (content.transform.localPosition.z < 0)
			{
				content.transform.localPosition = new Vector3(content.transform.localPosition.x, content.transform.localPosition.y, 0);
			}
		}
	}


	public void OnDrop(PointerEventData eventData)
	{
		GameObject pendingInstruction = eventData.pointerDrag;

		if (pendingInstruction == null || !acceptsNewItems || locked)
		{
			return;
		}

		if (temporaryButtonSlot == null)
		{
			Debug.LogError("Button slot not created for pending instruction. Cannot add.");
			return;
		}

		pendingInstruction.transform.SetParent(temporaryButtonSlot.transform.parent, false);
		pendingInstruction.transform.SetSiblingIndex(temporaryButtonSlot.transform.GetSiblingIndex());
		pendingInstruction.GetComponent<DragNDrop>().dragTargetValid = true;

		DestroyButtonSlot();

		//Child count is checked against 2, as the placeholder slot is not destroyed until end of frame
		if (contentPanel.transform.childCount == 2) {
			StartCoroutine(HintStart());
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!acceptsNewItems || eventData.pointerDrag == null)
		{
			return;
		}
		eventData.pointerDrag.GetComponent<DragNDrop>().activeDynamicScrollView = this;
		SpawnButtonSlot(eventData);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (eventData.pointerDrag != null)
		{
			eventData.pointerDrag.GetComponent<DragNDrop>().activeDynamicScrollView = null;
			DestroyButtonSlot();
		}
	}

	public void SpawnButtonSlot(PointerEventData eventData)
	{
		if (locked || temporaryButtonSlot != null)
		{
			Debug.LogWarning("ButtonSlot Already Spawned. Ignoring request for a new one.");
			return;
		}

		// Create a new empty item slot in the scroll view content
		temporaryButtonSlot = Instantiate(eventData.pointerDrag, contentPanel.transform);

		// UpdateScrollView will remove leftover slots with name "Placeholder"
		temporaryButtonSlot.name = "Placeholder";
		temporaryButtonSlot.GetComponent<CanvasGroup>().alpha = 0f;
		temporaryButtonSlot.SetActive(true);
	}

	// Destroy the spawned item slot if it isn't used
	public void DestroyButtonSlot()
	{
		if (temporaryButtonSlot == null)
		{
			Debug.LogError("Temporary Button Slot is already destroyed. What a shame.");
			return;
		}

		Destroy(temporaryButtonSlot);
		temporaryButtonSlot = null;
	}

	public void UpdateScrollView(PointerEventData eventData)
	{
		// var camera = FindObjectOfType<Camera>();
		Camera mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		Vector2 pointerPosition = mainCamera.ScreenToWorldPoint(eventData.position);

		foreach (Transform content in contentPanel.transform)
		{
			if (content.transform.localPosition.z < 0)
			{
				content.transform.localPosition = new Vector3(content.transform.localPosition.x, content.transform.localPosition.y, 0);
			}

			if (temporaryButtonSlot == null || content.gameObject == temporaryButtonSlot)
			{
				continue;
			}
			else if (content.gameObject.name == "Placeholder")
			{
				Debug.Log("Deleting leftover placeholder");
				Destroy(content.gameObject);
			}
			var siblingIndex = content.transform.GetSiblingIndex();
			var tempSlotIndex = temporaryButtonSlot.transform.GetSiblingIndex();

			if (pointerPosition.y > content.transform.position.y && tempSlotIndex > siblingIndex )
			{
				// If the button is being dragged above the content, but the new item slot is below the content (higher index),
				// move the new item slot up.
				temporaryButtonSlot.transform.SetSiblingIndex(content.transform.GetSiblingIndex());
			}
			else if (pointerPosition.y <= content.transform.position.y && tempSlotIndex < siblingIndex)
			{
				// If the button is being dragged below the content, but the new item slot is above the content (lower index),
				// move the new item slot down.
				temporaryButtonSlot.transform.SetSiblingIndex(content.transform.GetSiblingIndex());
			}
		}
	}

	// Become child free
	public void DestroyAllChildren()
	{
		if (locked)
		{
			Debug.LogWarning("Don't destroy children while simulation is running.");
			return;
		}
		foreach (Transform child in contentPanel.transform)
		{
			if (child.gameObject.activeInHierarchy)
			{
				Destroy(child.gameObject);
			}
		}
	}

	IEnumerator HintStart() {
		yield return new WaitForSeconds(0.25f);
		var UIController = FindObjectOfType<UIController>();
		UIController.HighlightUIElement("PlayButton");
		yield return new WaitForSeconds(4.0f);
		UIController.StopHighlightUIElement("PlayButton");
	}
}
