using System;
using UnityEngine;

namespace EmptySkull.Utilities
{
    /// <summary>
    /// Holds two integer and ensures, that the lower range-end 'From' has never a higher
    /// value than the higher range-end 'To'.
    /// </summary>
    [Serializable]
    public struct IntRange //TODO Inherit from "Range" and force From and To to int?
    {
        [SerializeField, HideInInspector] private int _fromValue;
        /// <summary>
        /// The lower end of the range.
        /// </summary>
        public int From
        {
            get => _fromValue;
            set
            {
                if (value > To)
                    throw new ArgumentException("You are not allowed to set the from-value larger then the to-value in a range!");
                _fromValue = value;
            }
        }

        [SerializeField, HideInInspector] private int _toValue;
        /// <summary>
        /// The higher end of the range.
        /// </summary>
        public int To
        {
            get => _toValue;
            set
            {
                if (value < From)
                    throw new ArgumentException("You are not allowed to set the to-value smaller then the from-value in a range!");
                _toValue = value;
            }
        }

        /// <summary>
        /// The total range (To minus From)
        /// </summary>
        public int Size => To - From;

        /// <summary>
        /// Holds two integer and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// </summary>
        /// <param name="from">The lower end of the range.</param>
        /// <param name="to">The higher end of the range.</param>
        public IntRange(int from, int to)
        {
            if (from > to)
                throw new ArgumentException("You are not allowed to create a range with larger from-value than to-value!");

            _toValue = to;
            _fromValue = from;
        }

        /// <inheritdoc />
        /// <summary>
        /// Holds two integer and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// </summary>
        /// <param name="to"></param>
        public IntRange(int to) : this(0, to) { }

        /// <inheritdoc />
        /// <summary>
        /// Holds two integer and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// </summary>
        /// <param name="r">The IntRange to copy From and To from.</param>
        public IntRange(IntRange r) : this(r.From, r.To) { }

        /// <summary>
        /// Holds two integer and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// Note that, due to integer-calculation, precision-loss is possible. Make use of the 'round'
        /// parameter to minimize it if necessary.
        /// </summary>
        /// <param name="r">The range to copy From and To from.</param>
        /// <param name="round">The method to round by</param>
        public IntRange(Range r, MathHelper.RoundType round = MathHelper.RoundType.None)
        {
            _fromValue = MathHelper.RoundByRoundType(r.From, round);
            _toValue = MathHelper.RoundByRoundType(r.To, round);
        }

        /// <summary>
        /// Holds two floats and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// </summary>
        /// <param name="v">The vector to copy From and To from. X cannot be larger then Y. Use 'switchOnOvershoot' if this is intended.</param>
        /// <param name="switchOnOvershoot">When the Vectors X-Value is larger then its Y-value, the values get switched if this is set to true.</param>
        public IntRange(Vector2Int v, bool switchOnOvershoot = false)
        {
            bool overshoot = v.x > v.y;
            if (overshoot && !switchOnOvershoot)
                throw new ArgumentException("You are not allowed to create a range with larger from-value than to-value!");

            _fromValue = !overshoot ? v.x : v.y;
            _toValue = !overshoot ? v.y : v.x;
        }

        /// <summary>
        /// Holds two floats and ensures, that the lower range-end 'From' has never a higher
        /// value than the higher range-end 'To'.
        /// Note that, due to integer-calculation, precision-loss is possible. Make use of the 'round'
        /// parameter to minimize it if necessary.
        /// </summary>
        /// <param name="v">The vector to copy From and To from. X cannot be larger then Y. Use 'switchOnOvershoot' if this is intended.</param>
        /// <param name="switchOnOvershoot">When the Vectors X-Value is larger then its Y-value, the values get switched if this is set to true.</param>
        /// /// <param name="round">The method to round by</param>
        public IntRange(Vector2 v, bool switchOnOvershoot = false, MathHelper.RoundType round = MathHelper.RoundType.None)
        {
            int rX = MathHelper.RoundByRoundType(v.x, round);
            int rY = MathHelper.RoundByRoundType(v.y, round);

            bool overshoot = rX > rY;
            if (overshoot && !switchOnOvershoot)
                throw new ArgumentException("You are not allowed to create a range with larger from-value than to-value!");

            _fromValue = !overshoot ? rX : rY;
            _toValue = !overshoot ? rY : rX;
        }


        /// <summary>
        /// Used to safely set the 'From'-value of the range. When invalid, the value is
        /// clamped. Use 'correctByGrowth' to instead set also the 'To'-value if necessary.
        /// </summary>
        /// <param name="value">The new value for 'From'.</param>
        /// <param name="correctByGrowth">When true, the 'To'-value is also set to the assigned
        /// value, when the value is larger then the current 'To'-value.</param>
        public void SetFrom(int value, bool correctByGrowth = false)
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
        public void SetTo(int value, bool correctByDecrease = false)
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
        public void Shift(int shiftAmount)
        {
            _fromValue += shiftAmount;
            _toValue += shiftAmount;
        }

        /// <summary>
        /// Scales the whole range relative to a given origin (that can be outside the range itself).
        /// Note that, due to integer-calculation, precision-loss is possible. Make use of the 'round'
        /// parameter to minimize it if necessary.
        /// </summary>
        /// <param name="scaleFactor">The factor to scale by.</param>
        /// <param name="origin">The origin of the scaling.</param>
        /// <param name="switchOnOvershoot">Whenever shrinking (negative scaling) intended by a 
        /// factor of less than -1, set this to true to switch the 'From' and 'To'-value so that
        /// thair rules of relation are not violated.</param>
        /// <param name="round">The method to round by.</param>
        public void Scale(float scaleFactor, int origin, bool switchOnOvershoot = false, MathHelper.RoundType round = MathHelper.RoundType.None)
        {
            int newFrom = MathHelper.RoundByRoundType(Mathf.LerpUnclamped(From, origin, scaleFactor), round);
            int newTo = MathHelper.RoundByRoundType(Mathf.LerpUnclamped(To, origin, scaleFactor), round);

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
        /// Note that, due to integer-calculation, precision-loss is possible. Make use of the 'round'
        /// parameter to minimize it if necessary.
        /// </summary>
        /// <param name="scaleFactor">The factor to scale by.</param>
        /// <param name="originRelative">The relative origin of the scaling.</param>
        /// <param name="switchOnOvershoot">Whenever shrinking (negative scaling) intended by a 
        /// factor of less than -1, set this to true to switch the 'From' and 'To'-value so that
        /// thair rules of relation are not violated.</param>
        /// /// <param name="round">The method to round by (relative origin and scale-calculations).</param>
        public void ScaleRelative(float scaleFactor, float originRelative, bool switchOnOvershoot = false, MathHelper.RoundType round = MathHelper.RoundType.None)
        {
            int origin = MathHelper.RoundByRoundType(Mathf.LerpUnclamped(From, To, originRelative), round);
            Scale(scaleFactor, origin, switchOnOvershoot, round);
        }

        /// <summary>
        /// Range from 0 to 100.
        /// </summary>
        public static IntRange Percentage => new IntRange(100);

        /// <summary>
        /// Range from 0 to 360 as a full circle angle.
        /// </summary>
        public static IntRange CircleDegree => new IntRange(360);

        /// <summary>
        /// Range from 0 to 255 as used in RGB-colorcoding.
        /// </summary>
        public static IntRange RgbColor => new IntRange(255);


        public static implicit operator Range(IntRange intRange)
        {
            return new Range(intRange.From, intRange.To);
        }
    }
}

