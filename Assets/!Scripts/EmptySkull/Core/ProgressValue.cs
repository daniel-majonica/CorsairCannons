using System.Text;
using UnityEngine;

namespace EmptySkull.Tools.Unity.Core
{
    /// <summary>
    /// A floating-point value always between 0 and 1, where 0 means no progress and 1 means progress is completed.
    /// </summary>
    public struct ProgressValue
    {
        public float Value { get; private set; }

        public bool IsCompleted => Value >= 1f;
        public bool IsStarted => Value >= 0f;
        public bool IsRunning => IsStarted && !IsCompleted;

        public ProgressValue(float v)
        {
            Value = Mathf.Clamp01(v);
        }

        public static implicit operator float(ProgressValue v) => v.Value;
        public static implicit operator ProgressValue(float f) => new ProgressValue(f);

        public string AsPercentLabel(bool spacing = true, int? decimalPlaces = null)
        {
            StringBuilder sBuilder = new StringBuilder();
            float percent = Value * 100;

            if (decimalPlaces.HasValue)
                sBuilder.Append(percent.ToString($"F{decimalPlaces.Value}"));
            else
                sBuilder.Append(percent);

            if (spacing)
                sBuilder.Append(" ");

            sBuilder.Append('%');

            return sBuilder.ToString();
        }
    }
}