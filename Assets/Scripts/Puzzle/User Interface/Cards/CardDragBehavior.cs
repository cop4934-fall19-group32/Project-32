using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardLogic))]
public class CardDragBehavior 
    : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
    
    public Transform ActiveContainerTransform { get; set; }
    public bool DragTargetValid { get; set; }

    public Vector3 dragScale = new Vector3(1.5f, 1.5f, 1.0f);

    private Canvas UICanvas;

    private void Start() {
        UICanvas = GetComponentInParent<Canvas>();

        if (transform.GetComponentInParent<CardContainer>()) {
            ActiveContainerTransform = transform.parent;
        }
        else {
            Debug.LogError("Card spawned outside of container. Card controller self destructing");
            Destroy(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (GetComponent<CardLogic>().BoundInstructions.Count > 0) {
            eventData.pointerDrag = null;
            return;
        }

        transform.SetParent(transform.parent.parent);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        DragTargetValid = false;
    }

    public void OnDrag(PointerEventData eventData) {
        GetComponent<RectTransform>().anchoredPosition += eventData.delta / UICanvas.scaleFactor;

        // Enlarge the card when dragged.
        this.transform.localScale = dragScale;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!DragTargetValid) {
            ActiveContainerTransform.GetComponent<CardContainer>().OnDrop(eventData);
        }

        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        Debug.Log(name + " Card Clicked!");

        var linker = FindObjectOfType<CardInstructionLinker>();

        if (GetComponent<CardLogic>().CardInPlay && linker != null) 
        {
            linker.ReportCardSelection(gameObject);
        }
    }
}
