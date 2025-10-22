using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MatchFinder
{
    // Returns a set of grid coords that belong to any 3+ horizontal/vertical runs.
    // Hashset is used to avoid duplicates (ex: candy both in horizontal and vertical
    public static HashSet<Vector2Int> FindMatches(System.Func<int, int, CandyView> getAt, int width, int height)
    {
        var result = new HashSet<Vector2Int>();

        // Horizontal
        for (int y = 0; y < height; y++)
        {
            int runStart = 0;
            while (runStart < width)
            {
                var a = getAt(runStart, y);
                int runLen = 1;
                int x = runStart + 1;

                while (x < width && a != null && getAt(x, y) != null && getAt(x, y).type == a.type)
                {
                    runLen++; x++;
                }

                if (a != null && runLen >= 3)
                    for (int k = 0; k < runLen; k++) result.Add(new Vector2Int(runStart + k, y));

                runStart = Mathf.Max(x, runStart + 1);
            }
        }

        // Vertical
        for (int x = 0; x < width; x++)
        {
            int runStart = 0;
            while (runStart < height)
            {
                var a = getAt(x, runStart);
                int runLen = 1;
                int y = runStart + 1;

                while (y < height && a != null && getAt(x, y) != null && getAt(x, y).type == a.type)
                {
                    runLen++; y++;
                }

                if (a != null && runLen >= 3)
                    for (int k = 0; k < runLen; k++) result.Add(new Vector2Int(x, runStart + k));

                runStart = Mathf.Max(y, runStart + 1);
            }
        }

        return result;
    }

    // ---------- NEW: grouping with type ----------
    public struct MatchGroup
    {
        public CandyType type;
        public List<Vector2Int> cells;
    }

    public static List<MatchGroup> FindGroups(System.Func<int, int, CandyView> getAt, int width, int height)
    {
        var groups = new List<MatchGroup>();

        // Horizontal groups
        for (int y = 0; y < height; y++)
        {
            int x = 0;
            while (x < width)
            {
                var a = getAt(x, y);
                if (a == null) { x++; continue; }

                int start = x;
                x++;
                while (x < width && getAt(x, y) != null && getAt(x, y).type == a.type) x++;

                int runLen = x - start;
                if (runLen >= 3)
                {
                    var cells = new List<Vector2Int>(runLen);
                    for (int k = 0; k < runLen; k++) cells.Add(new Vector2Int(start + k, y));
                    groups.Add(new MatchGroup { type = a.type, cells = cells });
                }
            }
        }

        // Vertical groups
        for (int x = 0; x < width; x++)
        {
            int y = 0;
            while (y < height)
            {
                var a = getAt(x, y);
                if (a == null) { y++; continue; }

                int start = y;
                y++;
                while (y < height && getAt(x, y) != null && getAt(x, y).type == a.type) y++;

                int runLen = y - start;
                if (runLen >= 3)
                {
                    var cells = new List<Vector2Int>(runLen);
                    for (int k = 0; k < runLen; k++) cells.Add(new Vector2Int(x, start + k));
                    groups.Add(new MatchGroup { type = a.type, cells = cells });
                }
            }
        }

        return groups;
    }
}

