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
	public float TopMoveSpeed = 15.0f;
	public float MoveTime = 0.5f;

	[Header("Dependencies")] //
	public LevelSelectCameraController CameraController;

	/** Editor handle for the Puzzle Scene */
	public Scene PuzzleScene;

	/** The node the character is currently resting on */
    public MapNode CurrentNode { get; private set; }

	/** Allows systems to query if Character is in motion */
    public bool IsMoving { get; private set; }

	/** Queue of move targets when moving in pathfinding mode */
    private Queue<MapNode> MoveTargetQueue = new Queue<MapNode>();

	/** The Pointer gameobject used to direct players to the next level in sequence */
	public GameObject NextLevelPointer;

	private Vector3 velocity = Vector3.zero;

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

		MoveTargetQueue.Enqueue(targetNode);

		StartCoroutine(Move());
	}

	public void HandleNodeSelect(MapNode node) {
		if (node == CurrentNode) {
			SelectLevel();
		}
		else {
			TriggerMove(node);
		}
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

		StartCoroutine(Move());
	}

	/**
	 * Logic to handle user level selection
	 */
    private void SelectLevel() 
    {
		if (IsMoving || CurrentNode.IsJunction) {
            return;
        }

		FindObjectOfType<PlayerState>().LastAttemptedLevel = CurrentNode.name;
        PlayerPrefs.SetString("SelectedLevel", CurrentNode.name);
		PlayerPrefs.SetString("CachedLevelPin", CurrentNode.name);

		var currentPuzzleData = CurrentNode.GetComponentInChildren<PuzzleData>();
		FindObjectOfType<GameState>().SetPuzzle(currentPuzzleData);

		SceneManager.LoadScene(PuzzleScene.handle);
    }

	IEnumerator Move() {
		NextLevelPointer.SetActive(false);
		const float MIN_DIST_SQR = 0.01f;
		IsMoving = true;

		yield return StartCoroutine(CameraController.PanToTarget(gameObject));

		while (MoveTargetQueue.Count > 0) {
			var target = MoveTargetQueue.Dequeue();
			while (Vector3.SqrMagnitude(transform.position - target.transform.position) > MIN_DIST_SQR) {
				Debug.Log("MoveTowards");

				transform.position = Vector3.SmoothDamp(
					transform.position, 
					target.transform.position, 
					ref velocity,
					MoveTime, 
					TopMoveSpeed
				);

				yield return null;
			}
			CurrentNode = target;
		}

		IsMoving = false;
		yield break;
	}
}
