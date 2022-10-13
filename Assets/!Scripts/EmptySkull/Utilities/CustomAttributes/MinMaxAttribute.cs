using System;
using UnityEngine;

namespace EmptySkull.Utilities.Attributes
{
    /// <summary>
    /// Replaces the default (or custom) property-drawer with a min-max-slider.
    /// Only works for Vector2, Vector2Int, Range and IntRange properties.
    /// Note that, some calculations like shifting and scaling of Range and IntRange 
    /// might get interfered by the editor-limits.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class MinMaxAttribute : PropertyAttribute
    {
        /// <summary>
        /// The minimum assignable limit.
        /// </summary>
        public float MinLimit;

        /// <summary>
        /// The maximum assignable limit.
        /// </summary>
        public float MaxLimit;

        /// <summary>
        /// When true, the default fields for the from- and to-values are hidden.
        /// </summary>
        public bool HideValues;

        /// <summary>
        /// When true, the limits are shown next to the slider.
        /// </summary>
        public bool ShowLimits;

        /// <summary>
        /// When true, the total selected size is shown beneath the slider.
        /// </summary>
        public bool ShowSize;


        /// <summary>
        /// Replaces the default (or custom) property-drawer with a min-max-slider.
        /// Only works for Vector2, Vector2Int, Range and IntRange properties.
        /// Note that, some calculations like shifting and scaling of Range and IntRange 
        /// might get interfered by the editor-limits.
        /// </summary>
        /// <param name="min">The minimum assignable limit.</param>
        /// <param name="max">The maximum assignable limit.</param>
        public MinMaxAttribute(float min, float max)
        {
            MinLimit = min;
            MaxLimit = max;
        }
    }
}