using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotStorageCounter : MonoBehaviour
{
    public TMPro.TextMeshProUGUI Readout;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var siblingCount = transform.parent.childCount - 1;
        Readout.text = (siblingCount > 0) ?  "x" + siblingCount.ToString() : "";
    }
}
