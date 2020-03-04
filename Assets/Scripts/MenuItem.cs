using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MainMenuTriggerEvent : UnityEvent { }

public class MenuItem : MonoBehaviour
{
    public MainMenuTriggerEvent TriggerCallback;

    public bool Active { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (TriggerCallback == null) {
            TriggerCallback = new MainMenuTriggerEvent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
