using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turbo : MonoBehaviour
{
    [Header("Computron")]
    public GameObject Computron;
    Actor compScript;

    void Start()
    {
        compScript = Computron.GetComponent<Actor>();
    }

    void Update()
    {
        
    }

    public void adjustSpeed(float speed)
    {
        compScript.MoveSpeed = 0.3f - (speed * 0.1f);
        compScript.InstructionDelay = 0.3f - (speed * 0.1f);
    }
}
