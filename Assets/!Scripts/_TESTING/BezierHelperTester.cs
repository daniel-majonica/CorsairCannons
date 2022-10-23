using EmptySkull.Utilities;
using UnityEngine;

public class BezierHelperTester : MonoBehaviour
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointA2;
    [SerializeField] private Transform _pointB;
    [SerializeField] private Transform _pointB2;

    [SerializeField] private int _samplePoints;

    protected virtual void OnDrawGizmos()
    {
        if (_pointA == null || _pointA2 == null || _pointB == null || _pointB2 == null)
            return;

        Vector2 from = PlanarProjectionHelper.AsPlanarVector(_pointA.position);
        Vector2 fromDir = PlanarProjectionHelper.AsPlanarVector(_pointA2.position) - from;
        Vector2 to = PlanarProjectionHelper.AsPlanarVector(_pointB.position);
        Vector2 toDir = PlanarProjectionHelper.AsPlanarVector(_pointB2.position) - to;

        using (new GizmoColorSwitcher(Color.green))
        {
            for (float delta = 0; delta <= 1; delta += 1f / _samplePoints)
            {
                Vector2 point = Bezier2DHelper.Sample(from, fromDir, to, toDir, delta);
                Gizmos.DrawSphere(PlanarProjectionHelper.FromPlanarVector(point), .025f);
            }

            //Vector2 point = Bezier2DHelper.Sample(from, fromDir, to, toDir, .5f);
            //Gizmos.DrawSphere(PlanarProjectionHelper.FromPlanarVector(point), .025f);
        }
    }
}
