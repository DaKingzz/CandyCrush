using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public static LevelConfig ActiveLevel { get; private set; }

    void Awake()
    {
        ActiveLevel = LevelRegistry.PendingLevel;
        if (ActiveLevel == null)
        {
            Debug.LogWarning("ActiveLevel was null; loading a default ScriptableObject from Resources as fallback.");
            ActiveLevel = Resources.Load<LevelConfig>("Levels/Level1");
        }

        GameState.InitializeFromLevel();
        
    }
}

