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

    public const int NumMapNodes = 10;

    public GameObject[] MapNodes = new GameObject[NumMapNodes];

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

        if (playerStateObj == null)
            return null;

        return playerStateObj.GetComponent<PlayerState>();
    }

    void SetupMapNodes()
    {
        PlayerState playerState = GetPlayerState();
        // what if none found

        for (int i = 0; i < MapNodes.Length; i++)
        {
            bool solved = playerState.GetPuzzleCompleted(MapNodes[i].name);

            if (playerState.GetScore() < MapNodes[i].GetComponent<MapNode>().ScoreRequired)
            {
                // This level is locked. Lock it on the map and color it red.
                MapNodes[i].GetComponent<MapNode>().Locked = true;
                ParticleSystem ps = MapNodes[i].GetComponent<ParticleSystem>();
                ParticleSystem.MainModule ma = ps.main;

                var gradient = new ParticleSystem.MinMaxGradient(
                    new Color(0.25f, 0.25f, 0.25f),
                    new Color(1, 0, 0)
                );

                gradient.mode = ParticleSystemGradientMode.TwoColors;

                ma.startColor = gradient;
            }
            else if (solved)
            {
                // This level is solved. Color it green on the map.
                ParticleSystem ps = MapNodes[i].GetComponent<ParticleSystem>();
                ParticleSystem.MainModule ma = ps.main;

                var gradient = new ParticleSystem.MinMaxGradient(
                    new Color(0.25f, 0.25f, 0.25f),
                    new Color(0, 1, 0)
                );

                gradient.mode = ParticleSystemGradientMode.TwoColors;

                ma.startColor = gradient;
            }
            else
            {
                // Update the start node to the last unlocked level that is not solved.
                StartNode = MapNodes[i].GetComponent<MapNode>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupMapNodes();
        LevelSelectCharacter.Initialize(StartNode);
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
