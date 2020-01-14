using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelButtonHandler : MonoBehaviour
{
    public GameState PuzzleElements;
    public void SelectLevel()
    {
        PuzzleElements.GetComponent<SubmitPanel>().RemoveSubmitPanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
