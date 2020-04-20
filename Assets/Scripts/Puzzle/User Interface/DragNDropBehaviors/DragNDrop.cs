using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class DragNDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

	// Reference the dynamic scroll view that the object is currently in.
	public InstructionContainer activeDynamicScrollView { get; set; }

	static public DragNDrop CurrDragInstruction { get; private set; }

	// Reference to the first canvas parent of this object
	protected Canvas UICanvas;

	// Gameobject being dragged must contain a Canvas Group component
	private CanvasGroup canvasGroup;

	private RectTransform rectTransform;

	// Track if object is dropped outside of any valid spaces.
	public bool dragTargetValid { get; set; }

	// If gameobject is clonable, it will leave a copy of itself in it's place after being dragged away.
	public bool isClonable;

	// If the button clones itself, store a reference to the duplicate.
	private GameObject clone;

    private AudioCue InstructionDragAudio;
    private AudioCue InstructionDropValidAudio;
    private AudioCue InstructionDropInvalidAudio;

	public void Initialize()
	{
		dragTargetValid = true;


		GameObject mainUICanvas = GameObject.FindWithTag("MainUICanvas");
		if (mainUICanvas == null)
		{
			Debug.LogWarning("Couldn't find game object with tag MainUICanvas.");
		}

		UICanvas = mainUICanvas.GetComponent<Canvas>();
		rectTransform = GetComponent<RectTransform>();
		canvasGroup = GetComponent<CanvasGroup>();

		GetComponent<UIControl>().ElementName = GetComponent<Command>().Instruction.ToString();
	}

	protected virtual void Awake()
	{
		Initialize();
        InitializeAudioCues();
	}

    private void InitializeAudioCues()
    {
        AudioCue[] audioCues = GetComponents<AudioCue>();
        foreach (AudioCue audioCue in audioCues)
        {
            if (audioCue.Name == "Instruction Drag Audio")
            {
                InstructionDragAudio = audioCue;
            }
            else if (audioCue.Name == "Instruction Drop Valid Audio")
            {
                InstructionDropValidAudio = audioCue;
            }
        }

        Transform content = this.transform.parent;
        Transform viewport = content.parent;
        Transform scrollView = viewport.parent;
        Transform window = scrollView.parent;
        InstructionDropInvalidAudio = window.GetComponent<AudioCue>();
    }

	protected virtual void OnDestroy() {
		;
	}

	// Called when beginning to drag object
	public virtual void OnBeginDrag(PointerEventData eventData)
	{
        InstructionDragAudio.Play();

		CurrDragInstruction = this;
		if (isClonable) {
			// When dragging the object from its original position, create a clone of the object.
			clone = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);
			clone.transform.SetSiblingIndex(transform.GetSiblingIndex());
			isClonable = false;
		}

		var myCanvas = GetComponent<Canvas>();
		myCanvas.overrideSorting = true;
		myCanvas.sortingLayerName = "Focus";
		myCanvas.sortingOrder = 1000;

		dragTargetValid = false;
		canvasGroup.alpha = .8f;

		if (activeDynamicScrollView != null) {
			activeDynamicScrollView.SpawnButtonSlot(eventData);
		}

		// raycast should go through object and land on item slot
		canvasGroup.blocksRaycasts = false;

		//Set button parent to UI canvas to allow free dragging
		transform.SetParent(UICanvas.transform);
	}

	// Called once per frame while dragging object
	public virtual void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / UICanvas.scaleFactor;

		if (activeDynamicScrollView != null)
		{
			activeDynamicScrollView.UpdateScrollView(eventData);
		}
	}


	// Called when ending the dragging of an object
	public virtual void OnEndDrag(PointerEventData eventData)
	{
		var myCanvas = GetComponent<Canvas>();
		myCanvas.overrideSorting = false;
		CurrDragInstruction = null;
		canvasGroup.alpha = 1f;

		// raycasts should land on item so it can be dragged again
		canvasGroup.blocksRaycasts = true;

		// If the object is dropped outside of a valid item slot, handle an invalid drop
		if (!dragTargetValid)
		{
            InstructionDropInvalidAudio.Play();
            HandleInvalidDrop();
		}
        else
        {
            InstructionDropValidAudio.Play();
        }
	}

	// Destroy this gameobject
	protected virtual void HandleInvalidDrop()
	{
		Destroy(gameObject);
	}
}
