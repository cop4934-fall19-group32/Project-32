using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevelButtonHandler : MonoBehaviour
{
    private GameObject PuzzleGenerator;
    public void SelectLevel()
    {
        PuzzleGenerator = GameObject.Find("PuzzleGenerator");
        PuzzleGenerator.GetComponent<SubmitPanel>().RemoveSubmitPanel();
        PuzzleGenerator.GetComponent<PuzzleGenerator>().UpdateActiveSave();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
