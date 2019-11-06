using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// The LevelManager class is used to build scenes that associated to different levels
/// by instantiating needed game objects.
/// It also has the ability to serialize and deserialize LevelData objects.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public LevelData levelData;
    public GameObject registerCard;
    public GameObject stackCard;
    public GameObject queueCard;
    public GameObject heapCard;

    private Transform LevelHolder;

    void DeserializeLevelData(int levelId)
    {
        string levelPath = "Assets/Resources/LevelSaves/level-" + levelId + ".json";
        using (StreamReader stream = new StreamReader(levelPath))
        {
            string levelDataJson = stream.ReadToEnd();
            this.levelData = JsonUtility.FromJson<LevelData>(levelDataJson);
        }
    }
    void SerializeLeveData(int levelId)
    {
        string levelPath = "Assets/Resources/LevelSaves/level-" + levelId + ".json";
        using (StreamWriter stream = new StreamWriter(levelPath))
        {
            LevelData sampleLevel = new LevelData(2, 2, 2, 2);
            string levelDataJson = JsonUtility.ToJson(levelData);
            stream.Write(levelDataJson);
        }
    }

    void SetupCards()
    {
        for (int i = 0; i < levelData.numRegisterCards; i++)
            Instantiate(registerCard, new Vector3(i+1, 1), Quaternion.identity);

        for (int i = 0; i < levelData.numStackCards; i++)
            Instantiate(stackCard, new Vector3(i+1, 3), Quaternion.identity);

        for (int i = 0; i < levelData.numQueueCards; i++)
            Instantiate(queueCard, new Vector3(i+1, 5), Quaternion.identity);

        for (int i = 0; i < levelData.numQueueCards; i++)
            Instantiate(heapCard, new Vector3(i+1, 7), Quaternion.identity);
    }

    public void SetupScene(int level)
    {
        DeserializeLevelData(level);
        SetupCards();
    }
}
