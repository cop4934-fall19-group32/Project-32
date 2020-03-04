using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
    public TMPro.TextMeshProUGUI ScoreLabel;

    // Start is called before the first frame update
    void Start()
    {
        ScoreLabel.text = FindObjectOfType<PlayerState>().GetScore().ToString();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
