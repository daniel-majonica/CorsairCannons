using System;
using UnityEngine;

namespace EmptySkull.Utilities
{
    /// <summary>
    /// Used to create types for Unity, that are nullable (by an internal bool-flag).
    /// </summary>
    /// <typeparam name="T">The value-type.</typeparam>
    public class BaseNullable<T>
    {
        /// <summary>
        /// Bool-flag to force the 'Value' to return null when set to true.
        /// </summary>
        public bool SetToNull;

        /// <summary>
        /// The current internal value.
        /// </summary>
        [SerializeField] private T _value;

        /// <summary>
        /// Will be true when either the bool-flag is set or the value is currently null.
        /// </summary>
        public bool IsNull => SetToNull || CheckValueForNull(_value);

        /// <summary>
        /// The current value. Use 'IsNull' to check if access is possible.
        /// Will cause a NullReferenceException, when accessed while the bool-flag is set.
        /// </summary>
        public T Value
        {
            get
            {
                if (IsNull)
                    throw new NullReferenceException("Nullable type was null. Cannot get value.");
                return _value;
            }
            set
            {
                SetToNull = CheckValueForNull(value);
                _value = value;
            }
        }

        /// <summary>
        /// Default-constructor will initialize with a set bool flag (therefore will be considered null on access).
        /// </summary>
        public BaseNullable()
        {
            SetToNull = true;
        }

        /// <summary>
        /// Constructor with a starting-value will not have a set bool-flag.
        /// </summary>
        /// <param name="startValue">The starting value.</param>
        public BaseNullable(T startValue)
        {
            SetToNull = startValue == null;
            _value = startValue;
        }


        /// <summary>
        /// Will reset the internal value to its default state and set the Null-Flag.
        /// </summary>
        public void ResetToNull()
        {
            _value = default;
            if (!IsNull)
                SetToNull = true;
        }

        /// <summary>
        /// Used to safely access the internal value.
        /// </summary>
        /// <param name="value">The current internal value.</param>
        /// <returns>Will only be true, when the value is not considered being null.</returns>
        public bool TryGetValue(out T value)
        {
            value = _value;
            return !IsNull;
        }

        /// <summary>
        /// Used to determine if a specific value should be considered null.
        /// Used internally to allow "GameObjects" to correctly be 'null'.
        /// </summary>
        /// <param name="value">The value to check for.</param>
        /// <returns>True, when the value should be considered null.</returns>
        protected virtual bool CheckValueForNull(T value) => value == null;
    }
}