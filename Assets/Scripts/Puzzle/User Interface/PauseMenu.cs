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
		GameObject PuzzleGenerator = GameObject.Find("PuzzleGenerator");
		if (PuzzleGenerator != null) {
			// Update the active save file.
			PuzzleGenerator.GetComponent<PuzzleGenerator>().UpdateActiveSave();
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
