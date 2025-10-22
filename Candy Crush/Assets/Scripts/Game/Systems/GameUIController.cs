using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameTimer timer;
    [SerializeField] private TextMeshProUGUI movesText;
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [System.Serializable]
    public struct CandyCounter
    {
        public CandyType type;
        public TextMeshProUGUI label; // e.g., "x3"
    }

    [Header("Side Panel Counters")]
    [SerializeField] private CandyCounter[] counters;

    // ---- single declaration (was duplicated before) ----
    private Dictionary<CandyType, TextMeshProUGUI> counterMap;

    void Awake()
    {
        EnsureInit();
    }

    void OnEnable()
    {
        EnsureInit();
    }

    private void EnsureInit()
    {
        if (counterMap != null) return;
        counterMap = new Dictionary<CandyType, TextMeshProUGUI>();
        foreach (var c in counters)
        {
            if (!counterMap.ContainsKey(c.type) && c.label != null)
                counterMap.Add(c.type, c.label);
        }
    }

    void Update()
    {
        if (movesText != null)
            movesText.text = "Moves: " + GameState.MovesLeft;

        if (timeText != null && timer != null)
            timeText.text = "Time: " + Mathf.CeilToInt(timer.TimeLeft) + "s";
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    public void UpdateGoals(Dictionary<CandyType, int> goals)
    {
        EnsureInit();
        if (goals == null || counterMap == null) return;

        foreach (var kv in goals)
        {
            if (counterMap.TryGetValue(kv.Key, out var label) && label != null)
                label.text = "x" + kv.Value;
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Game");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
