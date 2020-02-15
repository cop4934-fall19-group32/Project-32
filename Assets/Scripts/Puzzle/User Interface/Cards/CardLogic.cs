using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { 
    REGISTER,
    STACK,
    QUEUE,
    HEAP
}

public class CardLogic : MonoBehaviour {

    public CardType CardType;

    public GameObject BoundInstruction { get; private set; }

    private CardDatastructure datastructure;

    private TMPro.TextMeshProUGUI PanelText;
    private TMPro.TextMeshProUGUI LabelText;

    public string Address { 
        get {
            return LabelText.text; 
        } 
        set {
            LabelText.text = value;
        } 
    }

    public bool CardInPlay {
        get {
            var container = transform.GetComponentInParent<CardContainer>();
            
            return
                container.ContainerType == CardContainerType.PLAY;
        }
    }

    private void Awake() {
        PanelText = transform.Find("Panel").GetComponentInChildren<TMPro.TextMeshProUGUI>();
        LabelText = transform.Find("Label").GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        switch (CardType) {
            case CardType.REGISTER:
                datastructure = new RegisterCard();
                break;
            case CardType.STACK:
                datastructure = new StackCard();
                break;
            case CardType.QUEUE:
                datastructure = new QueueCard();
                break;
            case CardType.HEAP:
                datastructure = new HeapCard();
                break;
            default:
                throw new System.Exception();
        }

    }

    private void Update() {
        PanelText.text = datastructure.Peek().ToString();
        if (BoundInstruction != null) {
            GetComponent<UIControl>().Disable();
        }
    }

    public void LinkInstruction(GameObject instruction) {
        BoundInstruction = instruction;
    }

    public void MoveTo(int num) {
        datastructure.MoveTo(num);
    }

    public int? MoveFrom() {
        return datastructure.MoveFrom();
    }

    public void ClearData() {
        datastructure.ClearData();
    }
}
