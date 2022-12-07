using UnityEngine;

public static class TransformExtensions
{
    public static Ray GetForwardRay(this Transform transform, float lenght = 1f)
        => new Ray(transform.position, transform.forward * lenght);
}
