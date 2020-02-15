using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardCommandDragNDrop : DragNDrop {
    public TextMeshProUGUI ArgumentText;
    private UIController Controller;

    protected override void Awake() {
        base.Awake();
        Controller = FindObjectOfType<UIController>();
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);

        if (dragTargetValid) {
            var linker = gameObject.AddComponent<CardInstructionLinker>();
            linker.Instruction = gameObject;
            StartCoroutine(AwaitLink(linker));
        }
    }

    IEnumerator AwaitLink(CardInstructionLinker linker) {
        //Wait for the linker to finish
        yield return StartCoroutine(linker.Link());

        //Update arg according to linker results
        string cleanAddress = ArgumentText.text.Remove(0, 2);
        uint? convertedAddress = (uint?)System.Convert.ToInt32(cleanAddress, 16);
        GetComponent<Command>().Arg = convertedAddress;
    }
}