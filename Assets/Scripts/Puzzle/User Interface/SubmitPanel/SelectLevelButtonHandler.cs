using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelButtonHandler : MonoBehaviour
{
    private GameObject PuzzleCacher;
    public void SelectLevel()
    {
        PuzzleCacher = GameObject.Find("PuzzleCacher");
        PuzzleCacher.GetComponent<SubmitPanel>().RemoveSubmitPanel();
        PuzzleCacher.GetComponent<PuzzleCacher>().UpdateActiveSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
