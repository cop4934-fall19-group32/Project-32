using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{

	// public static bool paused = false;
	public GameObject pauseMenu;
	public bool isPaused = false;

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
		// bool pause = !pauseMenu.activeInHierarchy;
		// pauseMenu.SetActive(pause);
		// Time.timeScale = pause ? 0f : 1f;


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

	public void QuitGame()
	{
		Debug.Log("QUIT GAME");
		Application.Quit();
	}

}
