using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputronHintHandler : MonoBehaviour
{
    public GameObject HintPanel;
    public TMPro.TextMeshProUGUI HintText;
    private List<string> hints = new List<string>();
    int hintIndex = 0;
    private bool hovering;

    // Start is called before the first frame update
    void Start()
    {
        hovering = false;
        var gameState = FindObjectOfType<GameState>();
        var puzzleData = gameState.SelectedPuzzle;
        if (puzzleData.Hints.Count > 0) { 
            hints = puzzleData.Hints;
        }

        if (hints.Count == 0) {
            hints = new List<string> { "**bzzt* Sorry - no hints *bzzzzt* here", "Have you tried *bbzzt* ERROR: HINT NOT FOUND *bzzt*"};
        }

        if (HintText == null) {
            throw new System.Exception("Computron Prefab misconfigured. Please link HintHanlder to hint text");
        }

        StartCoroutine(FlickerText());
    }

    public void ShowHint() {
        HintPanel.SetActive(true);
        HintText.text = hints[hintIndex++];
        hintIndex %= hints.Count;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && !hovering) {
            HintPanel.SetActive(false);
        }
    }

    private void OnMouseOver() {
        hovering = true;
        if (Input.GetMouseButtonDown(0)) {
            ShowHint();
        }
    }

    private void OnMouseExit() {
        hovering = false;
    }

    private void OnEnable() {
        StartCoroutine(FlickerText());
    }

    IEnumerator FlickerText() {
        while (true) {
            //Flicker
            if (gameObject.activeSelf) {
                HintText.alpha = 0.8f;
                yield return new WaitForSeconds(0.05f);
                HintText.alpha = 1f;
                yield return new WaitForSeconds(0.05f);
                HintText.alpha = .7f;
                yield return new WaitForSeconds(0.05f);
                HintText.alpha = 1f;

                //Delay
                yield return new WaitForSeconds(Random.Range(0.5f, 5f));
            }
            yield return null;
        }
    }
}
