using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static int MovesLeft { get; private set; }
    public static bool Paused { get; private set; }
    public static bool Busy { get; private set; }

    public static void InitializeFromLevel()
    {
        MovesLeft = GameEntry.ActiveLevel != null ? GameEntry.ActiveLevel.movesLimit : 15;
        Paused = false;
        Busy = false;
    }

    public static void ConsumeMove()
    {
        if (MovesLeft > 0) MovesLeft--;
    }

    public static void RefundMove()
    {
        MovesLeft++;
    }

    public static void SetPaused(bool p) { Paused = p; }
    public static void SetBusy(bool b) { Busy = b; }
}


