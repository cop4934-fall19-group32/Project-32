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
    }

    public void ReportCardSelection(GameObject clickedCard) {
        Card = clickedCard;
    }

    public IEnumerator Link() {
        Debug.Log("Beginning linker phase");

        var uiController = FindObjectOfType<UIController>();
        //uiController.DisableAll();
        //uiController.EnableUIElement("CardPlayArea");
        uiController.HighlightUIElement("CardPlayArea");


        while (Card == null) {
            yield return null;
        }

        Instruction.GetComponent<CardCommandDragNDrop>().ArgumentText.text = Card.GetComponent<CardLogic>().Address;
        Card.GetComponent<CardLogic>().LinkInstruction(Instruction);

        uiController.EnableAll();
        uiController.DisableUIElement("PauseButton");
        uiController.StopHighlightUIElement("CardPanel");

        Debug.Log("Instruction linked. Linker self-destructing");
        Destroy(this);
    }
}
