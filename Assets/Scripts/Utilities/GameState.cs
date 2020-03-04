using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    /* Used to ensure singletonness */
    private static GameState instance;

    public PuzzleData SelectedPuzzle { get; set; }
 
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
