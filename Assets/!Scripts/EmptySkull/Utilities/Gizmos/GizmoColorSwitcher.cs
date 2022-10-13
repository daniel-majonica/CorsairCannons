using System;
using UnityEngine;

namespace EmptySkull.Utilities
{
    public class GizmoColorSwitcher : IDisposable
    {
        private readonly Color _oldColor;

        public GizmoColorSwitcher(Color color)
        {
            _oldColor = Gizmos.color;
            Gizmos.color = color;
        }

        public void Dispose()
        {
            Gizmos.color = _oldColor;
        }
    }
}