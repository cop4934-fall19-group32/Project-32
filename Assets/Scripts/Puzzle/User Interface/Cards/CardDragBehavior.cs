using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardLogic))]
public class CardDragBehavior 
    : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, 
      IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler 
{
    
    public CardContainer CardHand { get; set; }
    public bool DragTargetValid { get; set; }

    private Canvas UICanvas;

    public AnimationCurve CardResizeCurve;

    [Header("Grow/Shrink Controls")]
    public float GrowFactor = 1.0f;
    public float GrowTime = 0.15f;

    [Header("Swing-out Controls")]
    public float SwingTime = 0.25f;

    [Header("Fly-back Controls")]
    /** Total amount of time spent for the card to return to it's slot */
    public float TotalFlyTime = .75f;

    /** Percent of the total time to be used for */
    [Range(0, 0.95f)]
    public float VerticalMoveTimePercent = 0.3f;
    
    /** Time it takes for the card to swing back into position while flying back to slot */
    public float FlyingSwingTime = 0.125f;

    /** Percent of total time to delay the start of the swing operation */
    [Range(0, 0.95f)]
    public float FlyingSwingDelay = 0.65f;

    private Coroutine ActiveGrow;
    private Coroutine ActiveShrink;

    /** Static variables to ensure grow/shrink behavior doesn't get out of control */
    private static bool Dragging;
    private static bool Growing;
    private bool Flying;

    private AudioCue CardDragAudio;
    private AudioCue CardDropValidAudio;
    private AudioCue CardDropInvalidAudio;

    private void Start() {
        UICanvas = GameObject.FindGameObjectWithTag("MainUICanvas").GetComponent<Canvas>();
        CardHand = FindObjectOfType<CachedCardContainer>();

        if(GetComponentInParent<CardContainer>() == null) {
            Debug.LogError("Card spawned outside of container. Card will be destroyed");
            Destroy(gameObject);
        }
        InitializeAudioCues();
    }

    private void InitializeAudioCues() {
        AudioCue[] audioCues = GetComponents<AudioCue>();
        foreach (AudioCue audioCue in audioCues)
        {
            if (audioCue.Name == "Card Drag Audio")
            {
                CardDragAudio = audioCue;
            }
            else if (audioCue.Name == "Card Drop Valid Audio")
            {
                CardDropValidAudio = audioCue;
            }
            else if (audioCue.Name == "Card Drop Invalid Audio")
            {
                CardDropInvalidAudio = audioCue;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData) {
        CardDragAudio.Play();
        StartCoroutine(ShrinkCard());
        
        var myCanvas = GetComponent<Canvas>();
        myCanvas.overrideSorting = true;
        myCanvas.sortingLayerName = "Focus";
        myCanvas.sortingOrder = 30000;

        if (GetComponent<CardLogic>().BoundInstructions.Count > 0 || Flying) {
            eventData.pointerDrag = null;
            return;
        }

        Dragging = true;
        if (!GetComponent<CardLogic>().CardInPlay) {
            StartCoroutine(SwingCard(SwingTime, Quaternion.identity));
        }

        transform.SetParent(UICanvas.transform);
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        DragTargetValid = false;
    }

    public void OnDrag(PointerEventData eventData) {
        GetComponent<RectTransform>().anchoredPosition += eventData.delta / UICanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData) {
        Dragging = false; 

        if (!DragTargetValid) {
            CardDropInvalidAudio.Play();
            StartCoroutine(FlyBack());
        }
        else {
            CardDropValidAudio.Play();
        }

        var myCanvas = GetComponent<Canvas>();
        myCanvas.overrideSorting = false;

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

    public void OnPointerEnter(PointerEventData eventData) {
        if (Growing || Dragging) return;

        var myCanvas = GetComponent<Canvas>();
        myCanvas.overrideSorting = true;
        myCanvas.sortingLayerName = "Focus";
        myCanvas.sortingOrder = 1000;

        if (ActiveShrink != null) {
            StopCoroutine(ActiveShrink);
        }
        
        transform.localScale = new Vector3(1, 1, 1);
        ActiveGrow = StartCoroutine(GrowCard());
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (Dragging) {
            return;
        }
        ActiveShrink = StartCoroutine(ShrinkCard());
        var myCanvas = GetComponent<Canvas>();
        myCanvas.overrideSorting = false;
    }

    public IEnumerator GrowCard() {
        var targetScale = new Vector3(GrowFactor, GrowFactor, 1);

        Growing = true;

        float elapsedTime = 0.0f;
        while (elapsedTime < GrowTime) {

            float curvedPercentage = CardResizeCurve.Evaluate(elapsedTime / GrowTime);

            transform.localScale = 
                Vector3.LerpUnclamped(
                    transform.localScale, 
                    targetScale,
                    curvedPercentage
                );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Growing = false;
    }

    public IEnumerator ShrinkCard() {
        var targetScale = new Vector3(1, 1, 1);

        float elapsedTime = 0.0f;
        while (elapsedTime < GrowTime) {

            float curvedPercentage = CardResizeCurve.Evaluate(elapsedTime / GrowTime);

            transform.localScale =
                Vector3.LerpUnclamped(
                    transform.localScale,
                    targetScale,
                    curvedPercentage
                );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = new Vector3(1, 1, 1);
    }

    public IEnumerator SwingCard(float duration, Quaternion targetWorldRotation) {
        float elapsedTime = 0.0f;

        while (elapsedTime < duration) {

            transform.rotation =
                Quaternion.Slerp(
                    transform.rotation,
                    targetWorldRotation,
                    elapsedTime / duration
                );

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator FlyBack() {
        Flying = true;
        var swingRoutine = StartCoroutine(SwingDelay());

        var cardType = GetComponent<CardLogic>().CardType;
        var target = (CardHand as CachedCardContainer).GetWaypoint(cardType);
        float elapsedTime = 0.0f;

        var heightTarget = new Vector3(transform.position.x, target.y, transform.position.z);

        //Match Y
        var matchYTime = TotalFlyTime * VerticalMoveTimePercent;
        while (elapsedTime < matchYTime) {
            transform.position =
                Vector3.Lerp(
                    transform.position,
                    heightTarget,
                    elapsedTime / matchYTime
                );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Match X
        elapsedTime = 0.0f;
        var matchXTime = TotalFlyTime * (1 - VerticalMoveTimePercent);
        while (elapsedTime < matchXTime) {
            transform.position = 
                Vector3.Lerp(
                    transform.position, 
                    target, 
                    elapsedTime / matchXTime
                );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (swingRoutine != null) StopCoroutine(swingRoutine);
        Flying = false;
        CardHand.AddCard(gameObject);
    }

    public IEnumerator SwingDelay() {
        yield return new WaitForSeconds(TotalFlyTime * FlyingSwingDelay);
        StartCoroutine(SwingCard(FlyingSwingTime, Quaternion.Euler(0, 0, 90)));
    }

}
