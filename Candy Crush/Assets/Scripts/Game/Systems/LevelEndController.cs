using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndController : MonoBehaviour
{
    [SerializeField] private GameTimer timer;                 // drag HUD GameTimer here
    [SerializeField] private EndLevelUIController endUI;      // drag EndCanvas (with EndLevelUIController) here

    private ScoreManager score;
    private LevelConfig cfg;
    private bool ended;
    private bool armed;

    void Awake()
    {
        score = FindObjectOfType<ScoreManager>();
        cfg = GameEntry.ActiveLevel;
    }

    void Start()
    {
        // Arm after one frame to let GameEntry/ScoreManager initialize
        StartCoroutine(ArmNextFrame());
    }

    System.Collections.IEnumerator ArmNextFrame()
    {
        yield return null;
        armed = true;
    }

    void Update()
    {
        if (!armed || ended || cfg == null) return;

        bool goalsDone = (score != null) && score.GoalsComplete;
        bool outOfMoves = GameState.MovesLeft <= 0;
        bool outOfTime = (timer != null && timer.TimeLeft <= 0f);

        if (goalsDone)
        {
            End(true);
        }
        else if (outOfMoves || outOfTime)
        {
            End(false);
        }
    }

    void End(bool win)
    {
        if (ended) return;
        StartCoroutine(ShowWhenIdle(win));
    }

    System.Collections.IEnumerator ShowWhenIdle(bool win)
    {
        ended = true;

        // Wait for current clears/cascades to finish
        while (GameState.Busy) yield return null;

        // Freeze gameplay
        GameState.SetPaused(true);
        Time.timeScale = 0f;

        int finalScore = (score != null) ? score.Score : 0;
        int s1 = cfg != null ? cfg.star1Score : 0;
        int s2 = cfg != null ? cfg.star2Score : 0;
        int s3 = cfg != null ? cfg.star3Score : 0;

        if (win)
        {
            if (endUI != null)
            {
                endUI.ShowVictory(finalScore, s1, s2, s3, onContinue: () =>
                {
                    Time.timeScale = 1f;
                    GameState.SetPaused(false);
                    SceneManager.LoadScene("MainMenu");
                });
            }
        }
        else
        {
            if (endUI != null)
            {
                endUI.ShowFail(finalScore, s1, s2, s3,
                    onRetry: () =>
                    {
                        Time.timeScale = 1f;
                        GameState.SetPaused(false);
                        SceneManager.LoadScene("Game");
                    },
                    onMenu: () =>
                    {
                        Time.timeScale = 1f;
                        GameState.SetPaused(false);
                        SceneManager.LoadScene("MainMenu");
                    });
            }
        }
    }
}
