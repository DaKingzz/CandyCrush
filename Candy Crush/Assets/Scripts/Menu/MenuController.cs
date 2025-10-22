using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Game";

    void Update()
    {
        // Allow pressing ESC to quit the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

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
        AudioManager.PlayButton(); // we hear the quit

    #if UNITY_EDITOR
            // In Editor, just stop play mode
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            // In build, quit app
            Application.Quit();
    #endif
    }
}
