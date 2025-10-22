using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GameUIController ui;
    [SerializeField] private int goalsPerColor = 3;

    public int Score { get; private set; }

    private Dictionary<CandyType, int> goals = new Dictionary<CandyType, int>();
    
    void Start()
    {
        int gemTypes = (GameEntry.ActiveLevel != null) ? GameEntry.ActiveLevel.gemTypes : 5;
        InitializeGoals(gemTypes);
    }

    public void InitializeGoals(int gemTypes)
    {
        goals.Clear();
        for (int i = 0; i < gemTypes; i++)
        {
            goals[(CandyType)i] = goalsPerColor;
        }
        Score = 0;

        if (ui != null)
        {
            ui.UpdateScore(Score);
            ui.UpdateGoals(goals);
        }
    }

    public void ApplyGroups(List<MatchFinder.MatchGroup> groups, int multiplier = 1)
    {
        if (groups == null || groups.Count == 0) return;

        // 100 points per group * multiplier (fixed at 2 cuz yeah)
        int add = groups.Count * 100 * Mathf.Max(1, multiplier);
        Score += add;

        // We decrement per color by number of groups of that color
        foreach (var g in groups)
        {
            if (goals.ContainsKey(g.type) && goals[g.type] > 0)
                goals[g.type] = Mathf.Max(0, goals[g.type] - 1);
        }

        if (ui != null)
        {
            ui.UpdateScore(Score);
            ui.UpdateGoals(goals);
        }
    }

    public bool GoalsComplete
    {
        get
        {
            if (goals == null || goals.Count == 0) return false;
            foreach (var kv in goals) if (kv.Value > 0) return false;
            return true;
        }
    }

}


