using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameTimer timer;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timeText;

    void Update()
    {
        movesText.text = $"Moves: {GameState.MovesLeft}";
        timeText.text = $"Time: {Mathf.CeilToInt(timer.TimeLeft)}s";
    }

    public void RestartLevel()
    {
        // Reload Game scene, keep same rules (new board)
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
