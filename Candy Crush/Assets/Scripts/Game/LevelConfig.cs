using UnityEngine;

public enum RefillRule
{
    Level1 = 1,
    Level2 = 2
}

[CreateAssetMenu(menuName = "Match3/Level Config", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("Board")]
    public int width = 8;
    public int height = 8;

    [Tooltip("How many different gem types this level uses.")]
    public int gemTypes = 5;

    [Header("Goals / Difficulty")]
    public int movesLimit = 15;
    public int targetScore = 1000;

    [Header("RNG")]
    public int randomSeed = 0; // 0 = use system random

    [Header("Refill / Generation Rules")]
    public RefillRule refillRule = RefillRule.Level1; // set Level2.asset to Level2
}
