using System;
using UnityEngine;

public static class Bezier2DHelper
{
    //TODO Cleanup
    public static Vector2 Sample(Vector2 from, Vector2 fromDirection, Vector2 to, Vector2 toDirection, float delta, out Vector2 normal)
    {
        if (delta < 0f || delta > 1f)
            throw new ArgumentException("Delta must be between 0 and 1");

        Vector2 p0 = from;
        Vector2 p1 = from + fromDirection;
        Vector2 p2 = to - toDirection;
        Vector2 p3 = to;

        //Debug.DrawLine(Vector3.zero, PlanarProjectionHelper.FromPlanarVector(p0), Color.red);
        //Debug.DrawLine(Vector3.zero, PlanarProjectionHelper.FromPlanarVector(p1), Color.green);
        //Debug.DrawLine(Vector3.zero, PlanarProjectionHelper.FromPlanarVector(p2), Color.blue);
        //Debug.DrawLine(Vector3.zero, PlanarProjectionHelper.FromPlanarVector(p3), Color.yellow);

        Vector2 v1 = p0 + (p1 - p0) * delta;
        Vector2 v2 = p1 + (p2 - p1) * delta;
        Vector2 v3 = p2 + (p3 - p2) * delta;

        //Debug.DrawLine(Vector3.zero, PlanarProjectionHelper.FromPlanarVector(v1), Color.red);
        //Debug.DrawLine(Vector3.zero, PlanarProjectionHelper.FromPlanarVector(v2), Color.green);
        //Debug.DrawLine(Vector3.zero, PlanarProjectionHelper.FromPlanarVector(v3), Color.blue);

        Vector2 tempOrigin = v1 + (v2 - v1) * delta;
        Vector2 tempDirection = (v2 + (v3 - v2) * delta) - tempOrigin;

        //curvature = 1 - ((Vector2.Dot((to - from).normalized, tempDirection.normalized) + 1) / 2);

        normal = Vector2.Perpendicular(tempDirection);

        return tempOrigin + tempDirection * delta;
    }

    public static Vector2 Sample(Vector2 from, Vector2 fromDirection, Vector2 to, Vector2 toDirection, float delta)
        => Sample(from, fromDirection, to, toDirection, delta, out _);
}
