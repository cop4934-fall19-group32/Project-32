using System;
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

	/*
	 * Function called before the first frame of gameplay
	 */
	private void Awake() 
	{
		if (!PlayerPrefs.HasKey("Stars")) {
			PlayerPrefs.SetInt("Stars", 0);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		if (PlayerPrefs.HasKey("CachedLevelPin")) {
			var start = transform.Find(PlayerPrefs.GetString("CachedLevelPin")).GetComponent<MapNode>();
			if (start == null) {
				Debug.LogError("CachedLevelPin not found");
			}
			LevelSelectCharacter.Initialize(start);
		}
		else {
			LevelSelectCharacter.Initialize(StartNode);
		}
		//if (CachedStart != null) {
		//	LevelSelectCharacter.Initialize(CachedStart);
		//}
		//else {
		//	LevelSelectCharacter.Initialize(StartNode);
		//}
    }

    // Update is called once per frame
    void Update()
    {

    }

	/**
	 * Forwards RouteTo command to LevelSelectCharacter
	 */
	public void ReportMoveTarget(MapNode target) {
		LevelSelectCharacter.TriggerMove(target);
	}
}
