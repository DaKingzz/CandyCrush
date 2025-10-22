using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameUIController ui;
    private bool wasPaused = false;

    void Start()
    {
        Show(false);
    }

    public void TogglePause()
    {
        AudioManager.PlayButton();
        Show(!GameState.Paused);
    }

    public void Show(bool show)
    {
        GameState.SetPaused(show);
        panel.SetActive(show);
        Time.timeScale = show ? 0f : 1f;
    }

    // Button functions
    public void OnResume()
    {
        AudioManager.PlayButton();
        Show(false);
    }
    public void OnRestart() 
    {
        AudioManager.PlayButton();
        Time.timeScale = 1f; ui.RestartLevel(); 
    }
    public void OnReturnToMenu() 
    {
        AudioManager.PlayButton();
        Time.timeScale = 1f; ui.ReturnToMenu(); 
    }
}

