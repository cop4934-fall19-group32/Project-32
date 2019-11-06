using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// The GameManager class controls the state of the game at any given moment.
/// It makes use of the LevelManager class in order to build level scenes
/// for the player.
/// </summary>
public class GameManager : MonoBehaviour
{
    public LevelManager levelScript;

    private int level = 1;

    void Awake()
    {
        levelScript = GetComponent<LevelManager>();
        InitGame();
    }

    // This method is temporarily being used to initialize a sample scene containing:
    //     - 2 register cards
    //     - 2 stack cards
    //     - 2 queue cards
    //     - 2 heap cards
    void CreateSampleLevel()
    {
        string levelPath = "Assets/Resources/LevelSaves/level-1.json";
        using (StreamWriter stream = new StreamWriter(levelPath))
        {
            LevelData sampleLevel = new LevelData(2, 2, 2, 2);
            string levelDataJson = JsonUtility.ToJson(sampleLevel);
            stream.Write(levelDataJson);
        }
    }

    void InitGame()
    {
        CreateSampleLevel();
        levelScript.SetupScene(level);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
