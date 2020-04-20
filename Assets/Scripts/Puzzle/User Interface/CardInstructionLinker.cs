using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class CardInstructionLinker
 * @brief The CardInstructionLinker a transient behavior spawned after
 *        an instruction that must be linked to a card is played.
 */
public class CardInstructionLinker : MonoBehaviour
{
    public GameObject Instruction { get; set; }
    public GameObject Card { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        if (Instruction == null) {
            Debug.LogError("CardInstructionLinker cannot link null instruction. Terminating");
            Destroy(this);
        }

        StartCoroutine(Link());
    }

    public void ReportCardSelection(GameObject clickedCard) {
        Card = clickedCard;
    }

    private IEnumerator Link() {
        Debug.Log("Beginning linker phase");
        var uiController = FindObjectOfType<UIController>();
        uiController.FocusUIElement("CardHandArea");
        uiController.FocusUIElement("CardPlayArea");
        if (FindObjectOfType<PlayedCardContainer>().Count < 1) {
            uiController.HighlightUIElement("CardHandArea");
        }

        while (Card == null) {
            yield return null;
        }

        GetComponent<AudioCue>().Play();
        Instruction.GetComponent<CardCommandDragNDrop>().BindCard(Card);
        Card.GetComponent<CardLogic>().LinkInstruction(Instruction);

        uiController.ClearFocus();
        uiController.StopAllHighlights();
        Debug.Log("Instruction linked. Linker self-destructing");
        Destroy(this);
    }
}
