using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public CandyType candyType;

    public int xIndex;
    public int yIndex;

    public bool isMatched;
    private Vector2 currentPos;
    private Vector2 targetPos;

    public bool isMoving;

    public Candy(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
}

public enum CandyType
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple
}
