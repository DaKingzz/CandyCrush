using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    // Static so GridUtils can read dimensions
    public static int Width { get; private set; }
    public static int Height { get; private set; }
    private RefillRule activeRule;

    [Header("Setup")]
    [SerializeField] private CandyView[] candyPrefabs = new CandyView[5]; // Blue, Green, Red, Purple, Yellow
    [SerializeField] private float cellSize = 1.0f;
    [SerializeField] private Vector2 gridCenter = Vector2.zero; // world point (0,0) = screen middle

    [Header("Debug / Gizmos")]
    [SerializeField] private bool showNeighborhoodGizmos = true;
    [SerializeField] private int gizmoX = 3;
    [SerializeField] private int gizmoY = 3;

    // Runtime
    private CandyView[,] grid;

    // Access for other systems (swap)
    public CandyView GetAt(int x, int y) => (IsInside(x, y) ? grid[x, y] : null);
    public Vector2Int Size => new Vector2Int(Width, Height);
    public bool IsInside(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    void Awake()
    {
        // Dimensions & candy types from the active LevelConfig
        var lvl = GameEntry.ActiveLevel;
        Width = lvl.width;
        Height = lvl.height;
        activeRule = (lvl != null) ? lvl.refillRule : RefillRule.Level1;
    }

    void Start()
    {
        GenerateBoardNoMatches();
    }

    // =========================
    // INITIAL GENERATION
    // =========================
    void GenerateBoardNoMatches()
    {
        grid = new CandyView[Width, Height];

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                HashSet<int> banned = new HashSet<int>();

                if (x >= 2 && grid[x - 1, y].type == grid[x - 2, y].type)
                    banned.Add((int)grid[x - 1, y].type);

                if (y >= 2 && grid[x, y - 1].type == grid[x, y - 2].type)
                    banned.Add((int)grid[x, y - 1].type);

                int typeIndex = GetRandomTypeExcluding(banned);
                SpawnAt(x, y, typeIndex, instant: true);
            }
        }
    }

    int GetRandomTypeExcluding(HashSet<int> banned)
    {
        int gemTypes = Mathf.Clamp(GameEntry.ActiveLevel.gemTypes, 3, candyPrefabs.Length);
        List<int> candidates = new List<int>(gemTypes);
        for (int i = 0; i < gemTypes; i++)
            if (!banned.Contains(i)) candidates.Add(i);

        if (candidates.Count == 0)
            for (int i = 0; i < gemTypes; i++) candidates.Add(i);

        int idx = Random.Range(0, candidates.Count);
        return candidates[idx];
    }

    void SpawnAt(int x, int y, int typeIndex, bool instant)
    {
        var prefab = candyPrefabs[typeIndex];
        var pos = GridUtils.GridToWorld(x, y, cellSize, gridCenter);

        var cv = Instantiate(prefab, pos, Quaternion.identity, transform);
        cv.SetGridPos(x, y);
        if (!instant)
            cv.SnapToWorld(pos); // (could animate from above if you like)

        grid[x, y] = cv;
    }

    Vector3 WorldPos(int x, int y) => GridUtils.GridToWorld(x, y, cellSize, gridCenter);

    // =========================
    // SWAP & RESOLUTION
    // =========================
    public IEnumerator ResolveSwap(CandyView a, CandyView b)
    {
        GameState.SetBusy(true);

        // swap in data
        SwapInGrid(a, b);
        // swap anim
        yield return StartCoroutine(SwapTween(a, b));

        // find matches
        var matches = MatchFinder.FindMatches(GetAt, Width, Height);

        if (matches.Count == 0)
        {
            // swap back (invalid move)
            SwapInGrid(a, b);
            yield return StartCoroutine(SwapTween(a, b));
            GameState.RefundMove();
            GameState.SetBusy(false);
            yield break;
        }

        // resolve cascades
        yield return StartCoroutine(ResolveAllCascades(matches));

        GameState.SetBusy(false);
    }

    void SwapInGrid(CandyView a, CandyView b)
    {
        int ax = a.x, ay = a.y;
        int bx = b.x, by = b.y;

        grid[ax, ay] = b;
        grid[bx, by] = a;

        a.SetGridPos(bx, by);
        b.SetGridPos(ax, ay);
    }

    IEnumerator SwapTween(CandyView a, CandyView b)
    {
        Vector3 apos = WorldPos(a.x, a.y);
        Vector3 bpos = WorldPos(b.x, b.y);

        var ca = StartCoroutine(a.TweenToWorld(apos));
        var cb = StartCoroutine(b.TweenToWorld(bpos));
        yield return ca; yield return cb;
    }

    // =========================
    // CASCADE PIPELINE
    // =========================
    IEnumerator ResolveAllCascades(HashSet<Vector2Int> initialMatches)
    {
        var current = initialMatches;

        while (current.Count > 0)
        {
            // 1) Clear matched
            yield return StartCoroutine(ClearMatches(current));

            // 2) Gravity
            yield return StartCoroutine(ApplyGravity());

            // 3) Refill (Level 1 or Level 2 rules)
            if (activeRule == RefillRule.Level2)
                yield return StartCoroutine(RefillLevel2());
            else
                yield return StartCoroutine(RefillLevel1());

            // 4) Find new matches created by the refill (cascade)
            current = MatchFinder.FindMatches(GetAt, Width, Height);
        }
    }

    IEnumerator ClearMatches(HashSet<Vector2Int> matches)
    {
        foreach (var p in matches)
        {
            var cv = grid[p.x, p.y];
            if (cv != null)
            {
                grid[p.x, p.y] = null;
                Destroy(cv.gameObject);
            }
        }
        yield return null;
    }

    IEnumerator ApplyGravity()
    {
        // For each column, move candies down to fill nulls
        for (int x = 0; x < Width; x++)
        {
            int writeY = 0; // next filled slot from bottom upward

            for (int y = 0; y < Height; y++)
            {
                if (grid[x, y] != null)
                {
                    if (y != writeY)
                    {
                        var cv = grid[x, y];
                        grid[x, y] = null;
                        grid[x, writeY] = cv;
                        cv.SetGridPos(x, writeY);

                        // animate to new world pos
                        yield return StartCoroutine(cv.TweenToWorld(WorldPos(x, writeY)));
                    }
                    writeY++;
                }
            }
        }
        yield return null;
    }

    // =========================
    // REFILL - Level 1 Rules
    // =========================
    IEnumerator RefillLevel1()
    {
        int gemTypes = Mathf.Clamp(GameEntry.ActiveLevel.gemTypes, 3, candyPrefabs.Length);

        // For each column, count how many empties at top and fill bottom->up of that gap.
        for (int x = 0; x < Width; x++)
        {
            int y = 0;
            while (y < Height)
            {
                // skip filled
                while (y < Height && grid[x, y] != null) y++;
                if (y >= Height) break;

                // found a gap starting at y; find its size
                int gapStart = y;
                while (y < Height && grid[x, y] == null) y++;
                int gapEnd = y - 1; // inclusive

                // fill this gap bottom->top with level 1 probabilities
                bool isFirst = true;
                for (int fy = gapStart; fy <= gapEnd; fy++)
                {
                    int typeIndex = ChooseLevel1TypeFor(x, fy, gemTypes, isFirst);
                    // Avoid immediate 3 creation (check left-left and down-down)
                    typeIndex = AvoidImmediateThrees(x, fy, typeIndex, gemTypes);

                    SpawnAt(x, fy, typeIndex, instant: false);
                    isFirst = false;
                }
            }
        }

        yield return null;
    }

    // =======================================
    // REFILL - Level 2 (Neighborhood-weighted)
    // =======================================
    IEnumerator RefillLevel2()
    {
        int gemTypes = Mathf.Clamp(GameEntry.ActiveLevel.gemTypes, 3, candyPrefabs.Length);

        // For each column, fill gaps bottom->top (same as L1), but color is chosen by 8-neighborhood weights.
        for (int x = 0; x < Width; x++)
        {
            int y = 0;
            while (y < Height)
            {
                // skip filled
                while (y < Height && grid[x, y] != null) y++;
                if (y >= Height) break;

                int gapStart = y;
                while (y < Height && grid[x, y] == null) y++;
                int gapEnd = y - 1;

                // Fill gap bottom->top
                for (int fy = gapStart; fy <= gapEnd; fy++)
                {
                    int typeIndex = ChooseLevel2TypeFor(x, fy, gemTypes);
                    typeIndex = AvoidImmediateThrees(x, fy, typeIndex, gemTypes);

                    SpawnAt(x, fy, typeIndex, instant: false);
                }
            }
        }

        yield return null;
    }

    // Choose color based on counts in the 8-neighborhood (each color weight = count+1), normalized to 100%.
    // Only considers colors from [0..gemTypes-1].
    int ChooseLevel2TypeFor(int x, int y, int gemTypes)
    {
        int[] counts = new int[gemTypes]; // default 0
        foreach (var n in GetNeighbors8(x, y))
        {
            var cv = grid[n.x, n.y];
            if (cv != null)
            {
                int t = (int)cv.type;
                if (t >= 0 && t < gemTypes)
                    counts[t]++;
            }
        }

        // Weight = count + 1 (base uniform prior)
        int totalWeight = 0;
        int[] weights = new int[gemTypes];
        for (int i = 0; i < gemTypes; i++)
        {
            weights[i] = counts[i] + 1;
            totalWeight += weights[i];
        }

        // Weighted random pick
        int r = Random.Range(0, totalWeight);
        int cumulative = 0;
        for (int i = 0; i < gemTypes; i++)
        {
            cumulative += weights[i];
            if (r < cumulative)
                return i;
        }
        return Random.Range(0, gemTypes); // fallback
    }

    // Returns valid neighbor coords around (x,y)
    IEnumerable<Vector2Int> GetNeighbors8(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (IsInside(nx, ny))
                    yield return new Vector2Int(nx, ny);
            }
        }
    }

    int ChooseLevel1TypeFor(int x, int y, int gemTypes, bool isFirstInGap)
    {
        // "Bottom cell" in level description = y == 0 (board's bottom row)
        bool isBottomCell = (y == 0);
        int belowY = y - 1;
        CandyType? belowType = (belowY >= 0 && grid[x, belowY] != null) ? grid[x, belowY].type : (CandyType?)null;

        if (isBottomCell)
        {
            // Uniform
            return Random.Range(0, gemTypes);
        }

        if (isFirstInGap)
        {
            // 40% match the one below (if exists), else uniform
            if (belowType.HasValue && Random.value < 0.40f)
                return (int)belowType.Value;
            else
                return Random.Range(0, gemTypes);
        }
        else
        {
            // subsequent tiles in this column gap: 60% match below
            if (belowType.HasValue && Random.value < 0.60f)
                return (int)belowType.Value;
            else
                return Random.Range(0, gemTypes);
        }
    }

    int AvoidImmediateThrees(int x, int y, int initialType, int gemTypes)
    {
        // Try up to few attempts to avoid forming a 3 right away
        for (int attempt = 0; attempt < 5; attempt++)
        {
            int t = (attempt == 0) ? initialType : Random.Range(0, gemTypes);

            // would this create horizontal 3?
            if (x >= 2 && grid[x - 1, y] != null && grid[x - 2, y] != null)
            {
                if ((int)grid[x - 1, y].type == t && (int)grid[x - 2, y].type == t)
                    continue; // pick again
            }
            // would this create vertical 3?
            if (y >= 2 && grid[x, y - 1] != null && grid[x, y - 2] != null)
            {
                if ((int)grid[x, y - 1].type == t && (int)grid[x, y - 2].type == t)
                    continue; // pick again
            }
            return t;
        }
        return initialType; // give up, very rare
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showNeighborhoodGizmos) return;
        if (grid == null || grid.Length == 0) return;
        if (!IsInside(gizmoX, gizmoY)) return;

        Vector3 cellCenter = GridUtils.GridToWorld(gizmoX, gizmoY, cellSize, gridCenter);

        // Draw center cell (yellow)
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(cellCenter, Vector3.one * (cellSize * 0.9f));

        // Draw neighbors (color by candy type)
        foreach (var n in GetNeighbors8(gizmoX, gizmoY))
        {
            var cv = grid[n.x, n.y];
            Vector3 pos = GridUtils.GridToWorld(n.x, n.y, cellSize, gridCenter);

            Color c = Color.gray;
            if (cv != null)
            {
                switch (cv.type)
                {
                    case CandyType.Red: c = Color.red; break;
                    case CandyType.Blue: c = Color.blue; break;
                    case CandyType.Green: c = Color.green; break;
                    case CandyType.Yellow: c = Color.yellow; break;
                    case CandyType.Purple: c = new Color(0.6f, 0, 0.6f); break;
                }
            }

            Gizmos.color = c;
            Gizmos.DrawWireCube(pos, Vector3.one * (cellSize * 0.9f));
        }
    }
#endif
}

