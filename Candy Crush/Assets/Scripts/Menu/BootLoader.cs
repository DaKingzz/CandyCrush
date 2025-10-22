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
        // This is more for good practice than anything else tbh, not really necessary
        SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}
