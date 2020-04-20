using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardCommandDragNDrop : DragNDrop {
    public GameObject ArgLabel;
    public TextMeshProUGUI ArgumentText;
    private UIController Controller;
    public GameObject BoundCard { get; private set; }

    protected override void Awake() {
        base.Awake();
        ArgLabel.SetActive(false);
        Controller = FindObjectOfType<UIController>();
    }

    protected void Start() {

    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);

        //On a successful play of a card based instruction, trigger card instruction linker logic
        if (dragTargetValid && BoundCard == null) {
            var linker = gameObject.AddComponent<CardInstructionLinker>();
            linker.Instruction = gameObject;
            ArgLabel.SetActive(true);
        }

    }

    protected override void HandleInvalidDrop() {
        if (BoundCard != null) {
            BoundCard.GetComponent<CardUIControl>().Enable();
        }

        base.HandleInvalidDrop();
    }

    protected override void OnDestroy() {
        if (BoundCard) { 
            BoundCard.GetComponent<CardLogic>().UnlinkInstruction(gameObject);
        }
    }

    public void BindCard(GameObject go) {
        ArgLabel.SetActive(true);
        if (BoundCard != null) {
            BoundCard.GetComponent<CardLogic>().UnlinkInstruction(gameObject);
        }

        BoundCard = go;
        ArgumentText.text = BoundCard.GetComponent<CardLogic>().Address;
        
        //Update arg according to linker results
        string cleanAddress = ArgumentText.text.Remove(0, 2);
        uint? convertedAddress = (uint?)System.Convert.ToInt32(cleanAddress, 16);
        GetComponent<Command>().Arg = convertedAddress;
    }

    IEnumerator WaitForCards() {
        var playedCardsContainer = GameObject.FindGameObjectWithTag("RegisterContainer").GetComponent<CardContainer>();
        GetComponent<UIControl>().Disable();
        while (playedCardsContainer.Count < 1) {
            yield return null;
        }

        yield return StartCoroutine(WaitForNoCards());

        yield break;
    }

    IEnumerator WaitForNoCards() {
        var playedCardsContainer = GameObject.FindGameObjectWithTag("RegisterContainer").GetComponent<CardContainer>();
        GetComponent<UIControl>().Enable();

        while (playedCardsContainer.Count > 0) {
            yield return null;
        }

        yield return StartCoroutine(WaitForCards());

        yield break;
    }

}