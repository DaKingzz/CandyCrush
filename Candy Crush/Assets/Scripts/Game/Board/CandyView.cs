using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class CandyView : MonoBehaviour
{
    [Header("Data")]
    public CandyType type;

    [HideInInspector] public int x;
    [HideInInspector] public int y;

    [Header("Motion")]
    public float moveDuration = 0.12f; // quick snappy swap
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public void SetGridPos(int gx, int gy) { x = gx; y = gy; }

    public void SnapToWorld(Vector3 worldPos)
    {
        transform.position = worldPos;
    }

    public IEnumerator TweenToWorld(Vector3 worldPos)
    {
        Vector3 start = transform.position;
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float k = moveCurve.Evaluate(Mathf.Clamp01(t / moveDuration));
            transform.position = Vector3.Lerp(start, worldPos, k);
            yield return null;
        }
        transform.position = worldPos;
    }
}
