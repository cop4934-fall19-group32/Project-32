using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeleteButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public SaveButtonHandler Handler;
    private Sprite CachedGraphic;

    public void OnPointerEnter(PointerEventData eventData) {
        CachedGraphic = Handler.SlotImage.sprite;
        Handler.SlotImage.sprite = Handler.DeleteSlotGraphic;
    }

    public void OnPointerExit(PointerEventData eventData) {
        Handler.SlotImage.sprite = CachedGraphic;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
