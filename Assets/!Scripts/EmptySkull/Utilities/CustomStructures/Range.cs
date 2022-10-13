using System;
using UnityEngine;

namespace EmptySkull.Utilities
{
    /// <summary>
    /// Holds two floats and ensures, that the lower range-end 'From' has never a higher
    /// value than the higher range-end 'To'.
    /// </summary>
    [Serializable]
    public struct Range
    {
        [SerializeField, HideInInspector] private float _fromValue;
        /// <summary>
        /// The lower end of the range.
        /// </summary>
        public float From
        {
            get => _fromValue;
            set
            {
                if(value > To)
                    throw new ArgumentException("You are not allowed to set the from-value larger then the to-value in a range!");
                _fromValue = value;
            }
        }

        [SerializeField, HideInInspector] private float _toValue;
        /// <summary>
        /// The higher end of the range.
        /// </summary>
        public float To
        {
            get => _toValue;
            set
            {
                if(value < From)
                    throw new ArgumentException("You are not allowed to set the to-value smaller then the from-value in a range!");
                _toValue = value;
            }
        }

        /// <summary>
        /// The total range (To minus From)
        /// </summary>
        public float Size => To - From;

        /// <summary>
        /// Holds two floats and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// </summary>
        /// <param name="from">The lower end of the range.</param>
        /// <param name="to">The higher end of the range.</param>
        public Range(float from, float to)
        {
            if(from > to)
                throw new ArgumentException("You are not allowed to create a range with larger from-value than to-value!");

            _toValue = to;
            _fromValue = from;
        }

        /// <inheritdoc />
        /// <summary>
        /// Holds two floats and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// </summary>
        /// <param name="to"></param>
        public Range(float to) : this(0, to) { }

        /// <inheritdoc />
        /// <summary>
        /// Holds two floats and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// </summary>
        /// <param name="r">The range to copy From and To from.</param>
        public Range(Range r) : this(r.From, r.To) { }

        /// <summary>
        /// Holds two floats and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// </summary>
        /// <param name="v">The vector to copy From and To from. X cannot be larger then Y. Use 'switchOnOvershoot' if this is intended.</param>
        /// <param name="switchOnOvershoot">When the Vectors X-Value is larger then its Y-value, the values get switched if this is set to true.</param>
        public Range(Vector2 v, bool switchOnOvershoot = false)
        {
            bool overshoot = v.x > v.y;
            if(overshoot && !switchOnOvershoot)
                throw new ArgumentException("You are not allowed to create a range with larger from-value than to-value!");

            _fromValue = !overshoot ? v.x : v.y;
            _toValue = !overshoot ? v.y : v.x;
        }


        /// <summary>
        /// Used to safely set the 'From'-value of the range. When invalid, the value is
        /// clamped. Use 'correctByGrowth' to instead set also the 'To'-value if necessary.
        /// </summary>
        /// <param name="value">The new value for 'From'.</param>
        /// <param name="correctByGrowth">When true, the 'To'-value is also set to the assigned
        /// value, when the value is larger then the current 'To'-value.</param>
        public void SetFrom(float value, bool correctByGrowth = false)
        {
            if (value > To)
            {
                if (correctByGrowth)
                    To = value;
                else
                {
                    From = To;
                    return;
                }
            }
            From = value;
        }

        /// <summary>
        /// Used to safely set the 'To'-value of the range. When invalid, the value is
        /// clamped. Use 'correctByDecrease' to instead set also the 'From'-value if necessary.
        /// </summary>
        /// <param name="value">The new value for 'To'.</param>
        /// <param name="correctByDecrease">When true, the 'From'-value is also set to the assigned
        /// value, when the value is smaller then the current 'From'-value.</param>
        public void SetTo(float value, bool correctByDecrease = false)
        {
            if (value < From)
            {
                if (correctByDecrease)
                    From = value;
                else
                {
                    To = From;
                    return;
                }
            }
            To = value;
        }


        /// <summary>
        /// Shifts the whole range by adding a value two both 'From' and 'To' simultaneously.
        /// </summary>
        /// <param name="shiftAmount">The value to shift by. Can be negative to shift in minus-direction.</param>
        public void Shift(float shiftAmount)
        {
            _fromValue += shiftAmount;
            _toValue += shiftAmount;
        }

        /// <summary>
        /// Scales the whole range relative to a given origin (that can be outside the range itself).
        /// </summary>
        /// <param name="scaleFactor">The factor to scale by.</param>
        /// <param name="origin">The origin of the scaling.</param>
        /// <param name="switchOnOvershoot">Whenever shrinking (negative scaling) intended by a 
        /// factor of less than -1, set this to true to switch the 'From' and 'To'-value so that
        /// their rules of relation are not violated.</param>
        public void Scale(float scaleFactor, float origin, bool switchOnOvershoot = false)
        {
            float newFrom = Mathf.LerpUnclamped(From, origin, scaleFactor);
            float newTo = Mathf.LerpUnclamped(To, origin, scaleFactor);

            if (scaleFactor < -1)
            {
                if (!switchOnOvershoot)
                    throw new ArgumentException(
                        "Result of range-operation leaves from-value higher as to-value. This is not allowed. " +
                        "Consider setting 'switchOnOvershoot' to true if this operation is intended.");

                _fromValue = newTo;
                _toValue = newFrom;
                return;
            }

            _fromValue = newFrom;
            _toValue = newTo;
        }

        /// <summary>
        /// Scales the whole range relative to a given origin, that itself is assigned as a relative
        /// value between 'From' and 'To' linearly interpolated (that can be outside the range itself).
        /// </summary>
        /// <param name="scaleFactor">The factor to scale by.</param>
        /// <param name="originRelative">The relative origin of the scaling.</param>
        /// <param name="switchOnOvershoot">Whenever shrinking (negative scaling) intended by a 
        /// factor of less than -1, set this to true to switch the 'From' and 'To'-value so that
        /// their rules of relation are not violated.</param>
        public void ScaleRelative(float scaleFactor, float originRelative, bool switchOnOvershoot = false)
        {
            float origin = Mathf.LerpUnclamped(From, To, originRelative);
            Scale(scaleFactor, origin, switchOnOvershoot);
        }


        /// <summary>
        /// Casts the Range to an IntRange.
        /// </summary>
        /// <param name="round">The method to round by.</param>
        /// <returns>The converted IntRange.</returns>
        public IntRange ToIntRange(MathHelper.RoundType round = MathHelper.RoundType.None)
        {
            return new IntRange(MathHelper.RoundByRoundType(From, round), MathHelper.RoundByRoundType(To, round));
        }


        /// <summary>
        /// Range from 0 to 1.
        /// </summary>
        public static Range Range01 => new Range(1);

        /// <summary>
        /// Range from 0 to 100.
        /// </summary>
        public static Range Percentage => new Range(100);

        /// <summary>
        /// Range from 0 to 360 as a full circle angle.
        /// </summary>
        public static Range CircleDegree => new Range(360);
    }
}
