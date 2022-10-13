using System;
using UnityEngine;

namespace EmptySkull.Utilities
{
    /// <summary>
    /// Collection of mathematical solutions for Unity-specific problems.
    /// </summary>
    public static class UnityMath
    {
        /// <summary>
        /// Used to determine if a ray intersects with a plane. 
        /// </summary>
        /// <param name="p">The plane to check against.</param>
        /// <param name="r">The ray.</param>
        /// <param name="intersectionPoint">The point of intersection (Will have default value when no intersection exists).</param>
        /// <param name="distance">The distance between the rays' origin and the point of intersection
        /// ('-1' when no intersection exists).</param>
        /// <returns>Only true, when the ray intersects with the plane.</returns>
        public static bool TryIntersectionPlaneRay(Plane p, Ray r, out Vector3 intersectionPoint, out float distance)
        {
            if (Math.Abs(Vector3.Dot(p.normal, r.direction)) > .001f)
            {
                Vector3 diff = r.origin - p.normal * p.distance;
                float prod1 = Vector3.Dot(diff, p.normal);
                float prod2 = Vector3.Dot(r.direction, p.normal);
                distance = prod1 / prod2;
                intersectionPoint = r.origin - r.direction * distance;

                return true;
            }

            distance = -1;
            intersectionPoint = default;
            return false;
        }

        /// <summary>
        /// Used to determine if a ray intersects with a plane. 
        /// </summary>
        /// <param name="p">The plane to check against.</param>
        /// <param name="r">The ray.</param>
        /// <param name="intersectionPoint">The point of intersection (Will have default value when no intersection exists).</param>
        /// <returns>Only true, when the ray intersects with the plane.</returns>
        public static bool TryIntersectionPlaneRay(Plane p, Ray r, out Vector3 intersectionPoint)
        {
            return TryIntersectionPlaneRay(p, r, out intersectionPoint, out _);
        }

        /// <summary>
        /// Used to determine the point of intersection from a ray and a plane. 
        /// </summary>
        /// <param name="p">The plane to check against.</param>
        /// <param name="r">The ray.</param>
        /// <param name="distance">The distance between the rays' origin and the point of intersection
        /// ('-1' when no intersection exists).</param>
        /// <returns>The point of intersection (Will have default value when no intersection exists).</returns>
        public static Vector3 IntersectionPlaneRay(Plane p, Ray r, out float distance)
        {
            TryIntersectionPlaneRay(p, r, out Vector3 result, out distance);
            return result;
        }

        /// <summary>
        /// Used to determine the point of intersection from a ray and a plane. 
        /// </summary>
        /// <param name="p">The plane to check against.</param>
        /// <param name="r">The ray.</param>
        /// <returns>The point of intersection (Will have default value when no intersection exists).</returns>
        public static Vector3 IntersectionPlaneRay(Plane p, Ray r)
        {
            return IntersectionPlaneRay(p, r, out _);
        }
    }
}

