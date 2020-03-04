using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Interface of all elements operable by UIController
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(GraphicRaycaster))]
public abstract class ControllableUIElement : MonoBehaviour
{
	// Name used to search for the UI element
	public string ElementName;

	protected Image ElementGraphic;

	// Every controllable UI element have its own canvas component, which allows sorting layers to be used.
	public Canvas canvas;

	protected CanvasGroup canvasGroup;

	protected GraphicRaycaster raycaster;

	protected bool Pulsing;

	// Unlock UI element
	public abstract void Enable();

	// Lock UI element
	public abstract void Disable();
	
	// Bring element in front of Blur panel
	public abstract void Focus();

	// Restore sorting layer to behind blur panel
	public abstract void Unfocus();

	protected virtual void Awake() {
		Initialize();
	}

	// Controllable UI elements will use sorting layers to override the default rendering order (such as when focusing on an element)
	public void Initialize()
	{
		canvas = GetComponent<Canvas>();
		if (canvas == null)
		{
			throw new System.Exception("ControllableUIElement Requires Canvas component for sorting");
		}
		canvas.overrideSorting = true;
		canvas.sortingLayerName = "Default";
		CalculateSortingOrder();

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

	public void CalculateSortingOrder() {
		int sortOrder = 0;

		var parent = transform.parent;
		while (parent != null) {
			if (parent.GetComponent<ControllableUIElement>()) {

				sortOrder += 1;

			}

			if (parent.GetComponent<UIController>()) {
				break;
			}

			parent = parent.transform.parent;
		}

		canvas.sortingOrder = sortOrder;

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
		StopCoroutine(Pulse());
		ElementGraphic.color = new Color(1, 1, 1, 1);
	}

	protected IEnumerator Pulse()
	{
		Color from = Color.white;
		Color to = Color.grey;

		while (Pulsing)
		{
			//Approach grey color
			while (true){
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
	}
}
