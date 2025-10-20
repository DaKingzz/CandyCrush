using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string nextScene = "MainMenu";

    void Start()
    {
        // Initialize future systems here (Save, Audio, etc.)
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}
