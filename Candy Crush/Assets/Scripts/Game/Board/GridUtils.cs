using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtils
{
    public static Vector3 GridToWorld(int x, int y, float cellSize, Vector2 gridCenter)
    {
        // Grid origin so the board is centered on gridCenter
        float widthOffset = (x - 0.5f);
        float heightOffset = (y - 0.5f);
        return new Vector3(
            gridCenter.x + (widthOffset - (BoardManager.Width / 2f - 0.5f)) * cellSize,
            gridCenter.y + (heightOffset - (BoardManager.Height / 2f - 0.5f)) * cellSize,
            0f
        );
    }
}

