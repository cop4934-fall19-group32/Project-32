using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


// Interface of all elements operable by UIController
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(GraphicRaycaster))]
public abstract class ControllableUIElement : MonoBehaviour, IPointerClickHandler
{
	// Name used to search for the UI element
	public string ElementName;

	protected Image ElementGraphic;

	// Every controllable UI element have its own canvas component, which allows sorting layers to be used.
	public Canvas canvas;

	protected CanvasGroup canvasGroup;

	protected GraphicRaycaster raycaster;

	private Color StartColor;

	protected bool Pulsing;
	protected virtual void Awake() {
		Initialize();
		StartColor = GetComponent<Image>().color;
	}

	public void OnPointerClick(PointerEventData eventData) {
		StopHighlight();
	}

	protected void OnDestroy() {
		var controller = FindObjectOfType<UIController>();
		if (controller) {
			controller.RemoveEntry(this);
		}
	}

	// Unlock UI element
	public abstract void Enable();

	// Lock UI element
	public abstract void Disable();
	
	// Bring element in front of Blur panel
	public abstract void Focus();

	// Restore sorting layer to behind blur panel
	public abstract void Unfocus();

	// Controllable UI elements will use sorting layers to override the default rendering order (such as when focusing on an element)
	public void Initialize()
	{
		canvas = GetComponent<Canvas>();
		if (canvas == null)
		{
			throw new System.Exception("ControllableUIElement Requires Canvas component for sorting");
		}
		//canvas.overrideSorting = true;

		raycaster = GetComponent<GraphicRaycaster>();
		if (raycaster == null)
		{
			throw new System.Exception("ControllableUIElement Requires graphic raycaster for interactivity");
		}

		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null) {
			throw new System.Exception("UIControl Requires CanvasGroup component");
		}
		
		if (transform.localPosition.z < 0) {
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
		}
	}

	// Draw attention to control
	public virtual void StartHighlight()
	{
		if (!Pulsing)
		{
			Pulsing = true;
			StartCoroutine(Pulse());
		}
	}

	// Stop drawing attention to control
	public virtual void StopHighlight()
	{
		Pulsing = false;
	}

	protected IEnumerator Pulse()
	{
		Color from = Color.white;
		Color to = Color.grey;

		while (Pulsing)
		{
			//Approach grey color
			while (true){
				if (!Pulsing) break;
				ElementGraphic.color = Color.Lerp(ElementGraphic.color, Color.gray, 0.10f);

				var colorDifference = Mathf.Abs(
					ElementGraphic.color.maxColorComponent - Color.gray.maxColorComponent
				);

				if (colorDifference < 0.001f)
				{
					break;
				}

				yield return null;
			}

			yield return null;

			//Approach white color
			while (true)
			{
				if (!Pulsing) break;
				ElementGraphic.color = Color.Lerp(ElementGraphic.color, Color.white, 0.10f);

				var colorDifference = Mathf.Abs(
					ElementGraphic.color.maxColorComponent - Color.white.maxColorComponent
				);

				if (colorDifference < 0.001f)
				{
					break;
				}

				yield return null;
			}

			yield return null;
		}
		ElementGraphic.color = StartColor;
	}

}
