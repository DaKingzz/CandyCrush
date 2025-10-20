using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // Static so GridUtils can read dimensions
    public static int Width { get; private set; }
    public static int Height { get; private set; }

    [Header("Setup")]
    [SerializeField] private CandyView[] candyPrefabs = new CandyView[5]; // Blue, Green, Red, Purple, Yellow
    [SerializeField] private float cellSize = 1.0f;
    [SerializeField] private Vector2 gridCenter = Vector2.zero; // world point (0,0) = screen middle with orthographic camera

    // Runtime
    private CandyView[,] grid;

    // Access for other systems (swap)
    public CandyView GetAt(int x, int y) => (IsInside(x, y) ? grid[x, y] : null);
    public Vector2Int Size => new Vector2Int(Width, Height);

    void Awake()
    {
        // Dimensions & gemTypes from the active LevelConfig
        var lvl = GameEntry.ActiveLevel;
        Width = lvl.width;
        Height = lvl.height;
        // (lvl.gemTypes is respected in GetRandomTypeExcluding)
    }

    void Start()
    {
        GenerateBoardNoMatches();
    }

    public bool IsInside(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    void GenerateBoardNoMatches()
    {
        grid = new CandyView[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                // Exclude types that would make a 3+ match with left or down
                HashSet<int> banned = new HashSet<int>();

                // Left-left same?
                if (x >= 2 && grid[x - 1, y].type == grid[x - 2, y].type)
                    banned.Add((int)grid[x - 1, y].type);

                // Down-down same?
                if (y >= 2 && grid[x, y - 1].type == grid[x, y - 2].type)
                    banned.Add((int)grid[x, y - 1].type);

                var typeIndex = GetRandomTypeExcluding(banned);
                var prefab = candyPrefabs[typeIndex];

                var worldPos = GridUtils.GridToWorld(x, y, cellSize, gridCenter);
                var cv = Instantiate(prefab, worldPos, Quaternion.identity, transform);
                cv.SetGridPos(x, y);

                grid[x, y] = cv;
            }
        }
    }

    int GetRandomTypeExcluding(HashSet<int> banned)
    {
        int gemTypes = Mathf.Clamp(GameEntry.ActiveLevel.gemTypes, 3, candyPrefabs.Length);
        // Build candidate list
        List<int> candidates = new List<int>(gemTypes);
        for (int i = 0; i < gemTypes; i++)
            if (!banned.Contains(i)) candidates.Add(i);

        // Fallback if banned removes all (rare on small gemTypes)
        if (candidates.Count == 0)
        {
            for (int i = 0; i < gemTypes; i++) candidates.Add(i);
        }

        int idx = Random.Range(0, candidates.Count);
        return candidates[idx];
    }

    public IEnumerator SwapCandies(CandyView a, CandyView b)
    {
        // swap in data
        int ax = a.x; int ay = a.y;
        int bx = b.x; int by = b.y;

        grid[ax, ay] = b;
        grid[bx, by] = a;

        a.SetGridPos(bx, by);
        b.SetGridPos(ax, ay);

        // swap in world (animate)
        Vector3 apos = GridUtils.GridToWorld(bx, by, cellSize, gridCenter);
        Vector3 bpos = GridUtils.GridToWorld(ax, ay, cellSize, gridCenter);

        IEnumerator ta = a.TweenToWorld(apos);
        IEnumerator tb = b.TweenToWorld(bpos);

        // run both tweens concurrently
        Coroutine ca = StartCoroutine(ta);
        Coroutine cb = StartCoroutine(tb);
        yield return ca;
        yield return cb;
    }
}
