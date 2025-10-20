using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoardManager))]
public class SwapController : MonoBehaviour
{
    [SerializeField] private LayerMask candyMask;
    [SerializeField] private float dragThreshold = 0.2f; // min world distance to consider a direction

    private BoardManager board;
    private Camera cam;
    private CandyView selected;
    private Vector3 pressWorld;

    void Awake()
    {
        board = GetComponent<BoardManager>();
        cam = Camera.main;
    }

    void Update()
    {
        if (GameState.Paused) return; // block input while paused
        if (GameState.MovesLeft <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            pressWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            pressWorld.z = 0f;
            selected = RaycastCandy(pressWorld);
        }
        else if (Input.GetMouseButton(0) && selected != null)
        {
            Vector3 current = cam.ScreenToWorldPoint(Input.mousePosition);
            current.z = 0f;
            Vector3 delta = current - pressWorld;

            if (delta.magnitude >= dragThreshold)
            {
                Vector2Int dir = DominantDirection(delta);
                TrySwap(selected, dir);
                selected = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            selected = null;
        }
    }

    CandyView RaycastCandy(Vector3 world)
    {
        var hit = Physics2D.OverlapPoint(world, candyMask);
        if (hit) return hit.GetComponent<CandyView>();
        return null;
    }

    Vector2Int DominantDirection(Vector3 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            return (delta.x > 0f) ? Vector2Int.right : Vector2Int.left;
        else
            return (delta.y > 0f) ? Vector2Int.up : Vector2Int.down;
    }

    void TrySwap(CandyView a, Vector2Int dir)
    {
        int nx = a.x + dir.x;
        int ny = a.y + dir.y;
        if (!board.IsInside(nx, ny)) return;

        var b = board.GetAt(nx, ny);
        if (b == null) return;

        StartCoroutine(DoSwap(a, b));
    }

    IEnumerator DoSwap(CandyView a, CandyView b)
    {
        // One move per swap attempt (we’re not checking matches yet)
        GameState.ConsumeMove();

        yield return board.SwapCandies(a, b);

        // Later: if no match, swap back.
        // For now we leave it as-is; matching/refill logic will come later.
    }
}
