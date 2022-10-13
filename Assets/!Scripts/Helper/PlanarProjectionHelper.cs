using EmptySkull.Utilities;
using UnityEngine;

public static class PlanarProjectionHelper
{
    public static Vector2 AsPlanarVector(Vector3 v)
        => v.xz();

    public static Vector3 FromPlanarVector(Vector2 vPlanar, float yOffset = 0)
        => new Vector3(vPlanar.x, yOffset, vPlanar.y);


    public static Vector2 Between(Vector2 a, Vector2 b)
        => b - a;

    public static Vector2 Between(Vector3 a, Vector2 b)
        => Between(AsPlanarVector(a), b);

    public static Vector2 Between(Vector2 a, Vector3 b)
        => Between(a, AsPlanarVector(b));

    public static Vector2 Between(Vector3 a, Vector3 b)
        => Between(AsPlanarVector(a), AsPlanarVector(b));


    public static float DistanceSqr(Vector2 a, Vector2 b)
        => Between(a, b).sqrMagnitude;

    public static float DistanceSqr(Vector3 a, Vector2 b)
        => Between(a, b).sqrMagnitude;

    public static float DistanceSqr(Vector2 a, Vector3 b)
        => Between(a, b).sqrMagnitude;

    public static float DistanceSqr(Vector3 a, Vector3 b)
        => Between(a, b).sqrMagnitude;


    public static float Distance(Vector2 a, Vector2 b)
        => Mathf.Sqrt(DistanceSqr(a, b));

    public static float Distance(Vector3 a, Vector2 b)
        => Mathf.Sqrt(DistanceSqr(a, b));

    public static float Distance(Vector2 a, Vector3 b)
        => Mathf.Sqrt(DistanceSqr(a, b));

    public static float Distance(Vector3 a, Vector3 b)
        => Mathf.Sqrt(DistanceSqr(a, b));
}
