using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

	public GameObject pauseMenu;
	public bool isPaused = false;
	public bool turbo = false;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePause();
		}
	}

	public void TogglePause()
	{
		// Check if pause menu is currently active in scene, toggle the active state, and pause/resume time
		if (isPaused)
		{
			Time.timeScale = 1;
			pauseMenu.SetActive(false);
			isPaused = false;
		}
		else
		{
			Time.timeScale = 0;
			pauseMenu.SetActive(true);
			isPaused = true;
		}
	}

	public void ExitScene()
	{
		GameObject pc = GameObject.Find("PuzzleCacher");

		if (pc != null)
		{
			var puzzleCacher = pc.GetComponent<PuzzleCacher>();

			if (puzzleCacher == null)
			{
				Debug.LogWarning("Gameobject \"PuzzleCacher\" does not have a component \"Puzzle Cacher\" (script).");
			}
			else if (!puzzleCacher.CheckIfPuzzleDataExists())
			{
				Debug.LogWarning("puzzleCacher.puzzleData is null. Probably because you launched the puzzle scene from the editor and haven't loaded an actual level yet.");
			}
			else
			{
				// Update the active save file.
				puzzleCacher.UpdateActiveSave();
			}

		}

		Time.timeScale = 1;
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}

	// Quit will only work when playing an actual build. Nothing happens when playing in the Unity editor
	public void QuitGame()
	{
		Debug.Log("QUIT GAME");
		Application.Quit();
	}


	// When you gotta go fast
	public void TurboSpeed()
	{
		if (!turbo) {
			Time.timeScale = 100;
			turbo = true;
		}
		else {
			Time.timeScale = 1;
			turbo = false;
		}
	}
}
