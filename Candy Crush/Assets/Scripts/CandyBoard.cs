using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyBoard : MonoBehaviour
{
    // Size of Board
    public int width = 8;
    public int height = 8;
    // Space of Board
    public float spacingX;
    public float spacingY;
    // Reference to Candy Prefabs
    public GameObject[] candyPrefabs;
    // Reference to Collection of Nodes in Board + GameObject
    private Node[,] candyBoard;
    public GameObject candyBoardGO;

    public static CandyBoard Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        InitializeBoard();
    }

    void InitializeBoard()
    {
        candyBoard = new Node[width, height];

        spacingX = (float)(width - 1) / 2;
        spacingY = (float)((height - 1) / 2) + 1;

        // It starts from bottom left and works up every column, ends at top right
        for(int y = 0; y <height; y++) 
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);

                int randomIndex = Random.Range(0, candyPrefabs.Length);

                GameObject candy = Instantiate(candyPrefabs[randomIndex], position, Quaternion.identity);
                candy.GetComponent<Candy>().setIndicies(x, y);
                candyBoard[x, y] = new Node(true, candy);
            } 
        }
    }
}
