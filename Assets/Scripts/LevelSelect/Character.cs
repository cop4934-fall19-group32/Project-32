using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MoveDirection { 
    NORTH,
    SOUTH,
    EAST,
    WEST
}

/**
 * @class Character
 * @brief Script to handle level selection character logic
 */
public class Character : MonoBehaviour
{
	[Header("Movement Settings")] //
	public float BaseSpeed = 3.0f;
	public float MaxMoveTime = 1.5f;
    
    public bool IsMoving { get; private set; }

	/** Editor handle for the Puzzle Scene */
    public Scene PuzzleScene;

	/** The node the character is currently resting on */
    public MapNode CurrentNode { get; private set; }

	/** Queue of move targets when moving in pathfinding mode */
    private Queue<MapNode> MoveTargetQueue = new Queue<MapNode>();

	/** Target for the active transition, if any */
	private MapNode CurrentMoveTarget;

	/** Velocity vector of the transition */
	private Vector3 MoveVelocity = new Vector3();

	/**
	 * Unity function called before any Start() functions
	 * @note This function is used to ensure only one level select character
	 *       is active after scene transitions
	 */
	private void Awake() {

	}

	// Start is called before the first frame update
	void Start()
    {
		IsMoving = false;
    }

    /**
	 * Function called every frame to execute actor logic
	 * @note This function handles the Character's movement between nodes
	 */
    void Update() 
	{
		ProcessInput();
		Move();
	}

	/**
	 * Scans user input devices to discover user commands
	 */
	private void ProcessInput() {
		if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) {
			TriggerMove(MoveDirection.NORTH);
		}
		else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) {
			TriggerMove(MoveDirection.WEST);
		}
		else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) {
			TriggerMove(MoveDirection.SOUTH);
		}
		else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) {
			TriggerMove(MoveDirection.EAST);
		}
		else if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) {
			SelectLevel();
		}
	}

	/**
	 * Attempts to move Character towards it's current targetnode, if 
	 * available
	 */
	private void Move() 
	{
		const float MIN_DIST_SQR = 0.01f;
		
		//No action necessary if actor is at rest.
		if (!IsMoving) {
			return;
		}

		//TODO @MrLever: Expose this propery in editor (#60)

		//Calculate translation vector between character and target node
		var currPosition = transform.position;
		var targetPosition = CurrentMoveTarget.transform.position;
		var translationVector = targetPosition - currPosition;

		//Check if actor is close enough to node
		if (translationVector.sqrMagnitude > MIN_DIST_SQR) {
			transform.position =
				Vector3.SmoothDamp(
					transform.position,
					CurrentMoveTarget.transform.position,
					ref MoveVelocity,
					0.3f
				);
		}
		else {
			GetNextMoveTarget();
		}
	}

	/**
	 * Function to allow MapController to give Character StartNode
	 * @param startNode the initial node Computron should appear at
	 */
	public void Initialize(MapNode startNode) 
    {
		if (startNode != null) { 
			CurrentNode = startNode;
			transform.position = startNode.transform.position;
		}
    }

	/**
	 * Function to trigger a manual move (WASD)
	 * @param direction The direction to try and move
	 */
	public void TriggerMove(MoveDirection direction) 
    {
        if (IsMoving) {
            return;
        }

        var targetNode = CurrentNode.GetNeighbor(direction);
        if (targetNode == null) {
            return;
        }

        IsMoving = true;
		CurrentMoveTarget = targetNode;
	}

	/**
	 * Function to trigger an automatic move (A route to)
	 * @param movePath The shortest path selected by the MapController 
	 *        to route to target node
	 */
	public void TriggerMove(MapNode moveTarget) {
		if (IsMoving) {
			return;
		}

		var pathFinder = GetComponent<Pathfinder>();
		if (pathFinder == null) {
			Debug.LogError("Character does not have pathfinder component. Routeto failed.");
			return;
		}

		MoveTargetQueue = pathFinder.RouteTo(CurrentNode, moveTarget);

		if (MoveTargetQueue.Count == 0) {
			return;
		}
		else { 
			IsMoving = true;
			CurrentMoveTarget = MoveTargetQueue.Dequeue();
		}
	}



	/**
	 * Logic to handle user level selection
	 */
    private void SelectLevel() 
    {
        if (IsMoving || CurrentNode.IsJunction) {
            return;
        }
        PlayerPrefs.SetString("SelectedLevel", CurrentNode.name);
		PlayerPrefs.SetString("CachedLevelPin", CurrentNode.name);

		SceneManager.LoadScene(PuzzleScene.handle);
    }

	/**
	 * Aquires next move target from MoveTargetQueue, and calculates MoveVelocity
	 */
    private void GetNextMoveTarget() 
    {
		CurrentNode = CurrentMoveTarget;
        CurrentMoveTarget = (MoveTargetQueue.Count > 0) ? MoveTargetQueue.Dequeue() : null;
		if (CurrentMoveTarget == null) { 
			IsMoving = false;
			return;
		}

		//Calculate initial velocity of actor
		//TODO @MrLever: This code should be refactored to live elsewhere as part of #60
		var translationVector = CurrentMoveTarget.transform.position - transform.position;
		float distance = (transform.position - CurrentMoveTarget.transform.position).magnitude;

		float projectedMoveTime = distance / BaseSpeed;
		float speed = BaseSpeed;
		if (projectedMoveTime > MaxMoveTime) {
			speed = distance / MaxMoveTime;
		}

		MoveVelocity = speed * translationVector;
	}
}
