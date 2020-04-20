using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelDataDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public PuzzleData LevelData;
    public MapNode NodeData;
    public TMPro.TextMeshProUGUI LevelDescription;
    public TMPro.TextMeshProUGUI AwardDescription;
    public TMPro.TextMeshProUGUI StarsNeeded;
    public GameObject PopUpMenu;
    public StarController StarControl;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();

        if (NodeData.Locked) {
            transform.Find("StarRequirement").gameObject.SetActive(true);
            StarsNeeded.text = "x" + NodeData.ScoreRequired;
        }
        else {
            transform.Find("StarRequirement").gameObject.SetActive(false);
        }

        List<StarType> availableStars = StarControl.StarAligner(LevelData.HasEfficiency,
            LevelData.HasInstructionCount, LevelData.HasMemory);
        HashSet<StarType> earnedStars = StarControl.StarPlacer(LevelData.PuzzleName);
        string awardDescription = BuildAwardDescription(availableStars, earnedStars);
        LevelDescription.text = "<b>" + LevelData.PuzzleName + "</b>:\n" + LevelData.Description + awardDescription;
    }

    private string BuildAwardDescription(List<StarType> availableStars, HashSet<StarType> earnedStars)
    {
        string awardDescription = "\n\nStars: ";
        foreach (StarType award in availableStars)
        {
            //awardDescription += "(";

            if (earnedStars.Contains(award))
            {
                awardDescription += "<s><color=#FFD700>";
            }
            else
            {
                awardDescription += "<color=#eb3131>";
            }

            if (award == StarType.EFFICIENCY)
            {
                awardDescription += "Efficiency";
            }
            else if (award == StarType.INSTRUCTION_COUNT)
            {
                awardDescription += "Instruction Count";
            }
            else if (award == StarType.MEMORY)
            {
                awardDescription += "Memory";
            }

            awardDescription += "</color>";

            if (earnedStars.Contains(award))
            {
                awardDescription += "</s>";
            }

            awardDescription += ", ";
        }

        return awardDescription.Remove(awardDescription.Length - 2);
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
