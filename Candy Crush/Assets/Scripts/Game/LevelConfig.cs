using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

