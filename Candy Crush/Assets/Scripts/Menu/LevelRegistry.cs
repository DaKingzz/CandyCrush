using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRegistry : MonoBehaviour
{
    public LevelConfig level1;
    public LevelConfig level2;

    // Keep the chosen level in a static variable for the next scene
    public static LevelConfig PendingLevel;

    public void SelectLevel1() { PendingLevel = level1; }
    public void SelectLevel2() { PendingLevel = level2; }
}

