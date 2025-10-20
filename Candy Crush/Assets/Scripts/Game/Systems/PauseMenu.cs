using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel; // full-screen UI panel (parent)
    [SerializeField] private GameUIController ui; // for restart/return
    private bool wasPaused = false;

    void Start()
    {
        Show(false);
    }

    public void TogglePause()
    {
        Show(!GameState.Paused);
    }

    public void Show(bool show)
    {
        GameState.SetPaused(show);
        panel.SetActive(show);
        Time.timeScale = show ? 0f : 1f;
    }

    // Button hooks
    public void OnResume() => Show(false);
    public void OnRestart() { Time.timeScale = 1f; ui.RestartLevel(); }
    public void OnReturnToMenu() { Time.timeScale = 1f; ui.ReturnToMenu(); }
}

