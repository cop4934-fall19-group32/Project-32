using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * @class MapNode
 * @brief Script to represent operations of a MapNode in the level selection scene
 */
public class MapNode : MonoBehaviour, IPointerClickHandler 
{
	/** Per node options to control node representation */
	[Header("Options")] //
	public int LevelCode;
	public int ScoreRequired;
    public bool NodeVisible;
    public bool IsJunction;

	/** Member variables to allow neigbor specificaiton in editor */
    [Header("Neighbors")] //
    public MapNode North;
    public MapNode South;
    public MapNode East;
    public MapNode West;

	/** Captures neighbors as adjacency list to make pathfinding algorithms easier */
	public List<MapNode> AdjacencyList { get; private set; }

	public bool Locked { get; set; }

    // Start is called before the first frame update
    void Start() {
		//Disable development sprite visible in the editor
		if (!NodeVisible) {
			var sprite = GetComponent<SpriteRenderer>();
			if (sprite != null) {
				sprite.enabled = false;
			}
		}

		if (IsJunction && ScoreRequired != 0) {
			Debug.LogWarning("Junctions do not have score requirements. Editor value discarded");
			ScoreRequired = 0;
		}

		//Construct an adjacency list from editor neighbor settings
		//This list is used in path finding algorithms in place of the individual variables
		AdjacencyList = new List<MapNode>();
		if (North) {
			AdjacencyList.Add(North);
		}
		if (South) {
			AdjacencyList.Add(South);
		}
		if (East) {
			AdjacencyList.Add(East);
		}
		if (West) {
			AdjacencyList.Add(West);
		} 

		//Lines between nodes are drawn at initialization
		DrawLevelPaths();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	/**
	 * OnClick event handler to trigger Computron pathfinding in LevelSelect scene
	 * @param data Information about the click event
	 */
	public void OnPointerClick(PointerEventData data) {
		if (transform.parent == null) {
			Debug.LogError("Pin not contained in map, clicking disabled.");
			return;
		}

		var map = transform.parent.gameObject;
		if (!map) {
			Debug.LogError("MapNode does not have a parent map. Click movement will not work.");
			return;	
		}
		
		var mapcontroller = map.GetComponent<MapController>();
		if (!mapcontroller) {
			Debug.LogError("Map does not have a MapController. How did you manage that?");
			return;
		}

		mapcontroller.ReportMoveTarget(this);
	}

	/**
	 * Draws debug adjacency lines between map nodes at edit times
	 * @note Function cannot utilize AdjacencyList, as it is not initialized until runtime
	 */
	private void OnDrawGizmos() {
        if (North != null) {
            DrawDebugLine(North);
        }
        if (South != null) {
            DrawDebugLine(South);
        }
        if (East != null) {
            DrawDebugLine(East);
        }
        if (West != null) { 
            DrawDebugLine(West);
        }
    }

	/**
	 * Draws lines between connected nodes at runtime
	 */
	private void DrawLevelPaths() {
		var lineRenderer = GetComponent<LineRenderer>();
		int positionIndex = 0;

		if (!lineRenderer) {
			return;
		}

		lineRenderer.enabled = true;

		//Drawing only north and east edges still ensures every node appears connected
		if (North) {
			lineRenderer.positionCount += 3;
			lineRenderer.SetPosition(positionIndex++, transform.position);
			lineRenderer.SetPosition(positionIndex++, North.transform.position);
			lineRenderer.SetPosition(positionIndex++, transform.position);
		}
		if (East) {
			lineRenderer.positionCount += 3;
			lineRenderer.SetPosition(positionIndex++, transform.position);
			lineRenderer.SetPosition(positionIndex++, East.transform.position);
			lineRenderer.SetPosition(positionIndex++, transform.position);
		}
	}

	/**
	 * Converts MoveDirection input into a neighbor node
	 * @param direction The desired direction of the neighbor
	 * @return The neighbor node in the specified direction
	 */
	public MapNode GetNeighbor(MoveDirection direction) {
        MapNode target = null;
        
        switch (direction) {
            case MoveDirection.NORTH:
				if (!North.Locked) { 
					target = North;
				}
                break;
            case MoveDirection.SOUTH:
				if (!South.Locked) { 
					target = South;
				}
                break;
            case MoveDirection.EAST:
				if (!East.Locked) { 
					target = East;
				}
                break;
            case MoveDirection.WEST:
				if (!West.Locked) { 
					target = West;
				}
                break;
        }

        return target;
    }

	/**
	 * Helper function to draw a debug line between this and target pin
	 * @param pin The target pin
	 */
    private void DrawDebugLine(MapNode pin) {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, pin.transform.position);
    }

}
