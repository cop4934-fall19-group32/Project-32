using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * @class MapController
 * @brief Script to controll higher order map operations in puzzle select scene 
 *        like map loading and pathfinding
 */
public class MapController : MonoBehaviour
{
	[Header("Character")] //
	public Character LevelSelectCharacter;

	[Header("Initialization")] //
	public MapNode StartNode;

    private List<GameObject> MapNodes = new List<GameObject>();

    /*
	 * Function called before the first frame of gameplay
	 */
    private void Awake() 
	{
		if (!PlayerPrefs.HasKey("Stars")) {
			PlayerPrefs.SetInt("Stars", 0);
		}
	}

    private PlayerState GetPlayerState()
    {
        GameObject playerStateObj = GameObject.Find("PlayerState");

        if (!playerStateObj) {
            var newPlayerState = new GameObject();
            return newPlayerState.AddComponent<PlayerState>();
        }
        else { 
            return playerStateObj.GetComponent<PlayerState>();
        }

    }

    void FindStartNode()
    {
        foreach (var node in MapNodes) {
            var data = node.GetComponent<MapNode>();
            if (!data.Locked && data.ScoreRequired > StartNode.ScoreRequired)
                StartNode = data;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FindStartNode();
        LevelSelectCharacter.Initialize(StartNode);
    }

    // Update is called once per frame
    void Update()
    {

    }

	/**
	 * Forwards RouteTo command to LevelSelectCharacter
	 */
	public void ReportNodeSelection(MapNode target) {
		LevelSelectCharacter.HandleNodeSelect(target);
	}

    public void RegisterMapNode(GameObject node) {
        MapNodes.Add(node);
    }
}
