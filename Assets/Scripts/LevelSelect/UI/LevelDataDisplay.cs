using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelDataDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public PuzzleData LevelData;
    public MapNode NodeData;
    public TMPro.TextMeshProUGUI LevelDescription;
    public TMPro.TextMeshProUGUI StarsNeeded;
    public GameObject PopUpMenu;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();
        LevelDescription.text = LevelData.Description;
        if (NodeData.Locked) {
            transform.Find("StarRequirement").gameObject.SetActive(true);
            StarsNeeded.text = "x" + NodeData.ScoreRequired;
        }
        else {
            transform.Find("StarRequirement").gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        PopUpMenu.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        PopUpMenu.SetActive(false);
    }
}
