using System;

namespace EmptySkull.Utilities
{
    /// <summary>
    /// Collection of mathematical operations and solutions.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Rounds a given float-value to an integer by a specific round-operation.
        /// </summary>
        /// <param name="number">The number to round.</param>
        /// <param name="round">The type of operation to round by.</param>
        /// <returns>The rounded number as an integer.</returns>
        public static int RoundByRoundType(float number, RoundType round)
        {
#if UNITY_2020_2_OR_NEWER
            return round switch
            {
                RoundType.None => (int)number,
                RoundType.Floor => (int)Math.Floor(number),
                RoundType.Ceil => (int)Math.Ceiling(number),
                RoundType.RoundToEven => (int)Math.Round(number, MidpointRounding.ToEven),
                RoundType.RoundAwayFromZero => (int)Math.Round(number, MidpointRounding.AwayFromZero),
                _ => throw new ArgumentOutOfRangeException(nameof(round), round, null)
            };
#else
            switch(round)
            {
                case RoundType.None:
                    return (int)number;
                case RoundType.Floor:
                    return (int)Math.Floor(number);
                case RoundType.Ceil:
                    return (int)Math.Ceiling(number);
                case RoundType.RoundToEven:
                    return (int)Math.Round(number, MidpointRounding.ToEven);
                case RoundType.RoundAwayFromZero:
                    return (int)Math.Round(number, MidpointRounding.AwayFromZero);
                default:
                    throw new ArgumentOutOfRangeException(nameof(round), round, null);
            }
#endif
        }

        /// <summary>
        /// Performs an inverse linear interpolation without clamping the resulting value between the input-arguments
        /// (like "UnityEngine.Mathf.InverseLerp()" does).
        /// </summary>
        /// <param name="a">Lower limit of the interpolation range.</param>
        /// <param name="b">Upper limit of the interpolation range.</param>
        /// <param name="value">The interpolation-value</param>
        /// <returns>Result of an inverse lerp (not clamped).</returns>
        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            return (value - a) / (b - a);
        }

        /// <summary>
        /// Defines different types of round-operations.
        /// </summary>
        public enum RoundType
        {
            None, Floor, Ceil, RoundToEven, RoundAwayFromZero
        }
    }
}

