using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    public void LoadGame()
    {
        if (LevelRegistry.PendingLevel == null)
        {
            Debug.LogWarning("No level selected, defaulting to Level 1 if available.");
        }
        SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

