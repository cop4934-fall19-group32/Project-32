using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardMenu : MonoBehaviour
{
	public GameObject[] MenuControls;
	public GameObject SelectionIconPrefab;
	private GameObject SelectionIcon;

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
		SelectionIcon = Instantiate(SelectionIconPrefab, transform);
	}

	public void OnEnable() {
		if (SelectionIcon == null) {
			SelectionIcon = Instantiate(SelectionIconPrefab, transform);
		}
	}

	protected void OnDisable() {
		Destroy(SelectionIcon);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
			activeControl++;
			activeControl %= MenuControls.Length;
		}
		else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
			activeControl--;
			if (activeControl < 0) {
				activeControl = MenuControls.Length - 1;
			}
		}
		else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
			MenuControls[activeControl].GetComponent<MenuItem>().TriggerCallback.Invoke();
		}

		var currControl = MenuControls[activeControl].GetComponent<MenuItem>();

		SelectionIcon.transform.SetParent(MenuControls[activeControl].transform);
		SelectionIcon.GetComponent<RectTransform>().anchoredPosition = currControl.ControlIndicatorOffset;
		SelectionIcon.transform.localEulerAngles = currControl.ControlIndicatorOrientation;
	}
}
