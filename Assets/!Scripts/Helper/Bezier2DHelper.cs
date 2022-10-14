using UnityEngine;

public static class Bezier2DHelper
{
    //TODO Cleanup
    public static Vector2 Sample(Vector2 from, Vector2 fromDirection, Vector2 to, Vector2 toDirection, float delta, out float curvature)
    {
        Vector2 p0 = from;
        Vector2 p1 = from + fromDirection;
        Vector2 p2 = to - toDirection;
        Vector2 p3 = to;

        Vector2 v1 = p0 + (p1 * delta);
        Vector2 v2 = p1 + (p2 - p1) * delta;
        Vector2 v3 = p2 + (p3 - p2) * delta;

        Vector2 tempOrigin = v1 + (v2 - v1) * delta;
        Vector2 tempDirection = (v2 + (v3 - v2) * delta) - tempOrigin;

        curvature = (Vector2.Dot(toDirection.normalized, tempDirection.normalized) + 1) / 2;

        return tempOrigin + tempDirection * delta;
    }

    public static Vector2 Sample(Vector2 from, Vector2 fromDirection, Vector2 to, Vector2 toDirection, float delta)
        => Sample(from, fromDirection, to, toDirection, delta, out _);
}
