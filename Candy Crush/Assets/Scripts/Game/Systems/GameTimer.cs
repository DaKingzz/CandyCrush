using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float startSeconds = 90f;
    public float TimeLeft { get; private set; }

    void Awake()
    {
        TimeLeft = startSeconds;
    }

    void Update()
    {
        if (GameState.Paused) return;
        if (TimeLeft <= 0f) return;

        TimeLeft -= Time.deltaTime;
        if (TimeLeft < 0f) TimeLeft = 0f;
    }
}

