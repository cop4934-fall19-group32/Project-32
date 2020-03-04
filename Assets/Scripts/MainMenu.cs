using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject[] MenuControls;
	public GameObject SelectionIcon;
	public Vector3 ControlIndicatorOffset = new Vector3(-25.0f, 0, 0);

	private int activeControl;

	public void PlayGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void ExitScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
	}

	public void QuitGame()
	{
		Debug.Log("QUIT GAME");
		Application.Quit();
	}

	private void Awake() {
		activeControl = 0;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			activeControl++;
			activeControl %= MenuControls.Length;
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			activeControl--;
			if (activeControl < 0) {
				activeControl = MenuControls.Length - 1;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			MenuControls[activeControl].GetComponent<MenuItem>().TriggerCallback.Invoke();
		}

		SelectionIcon.transform.SetParent(MenuControls[activeControl].transform);
		SelectionIcon.GetComponent<RectTransform>().anchoredPosition = ControlIndicatorOffset;
	}
}
