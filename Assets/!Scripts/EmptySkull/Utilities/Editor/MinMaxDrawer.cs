using System.Globalization;
using EmptySkull.Utilities.Attributes;
using UnityEngine;
using UnityEditor;

namespace EmptySkull.Utilities
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer, ISerializationCallbackReceiver
    {
        private const float ValueRectWidth = 35;
        private const float MinMaxRectWidthPerDigit = 7;

        private bool _loggedWarning;

        private GUIStyle _endRangeStyleValue;
        private GUIStyle EndRangeStyle
        {
            get
            {
                if (_endRangeStyleValue != null)
                    return _endRangeStyleValue;

                _endRangeStyleValue = EditorStyles.miniBoldLabel;
                _endRangeStyleValue.alignment = TextAnchor.MiddleCenter;

                return _endRangeStyleValue;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MinMaxAttribute minMax = attribute as MinMaxAttribute;

            if (TryGetMinValue(property, out float minValue) && TryGetMaxValue(property, out float maxValue))
            {
                if (minMax.ShowSize)
                    position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

                float minLimit = minMax.MinLimit;
                float maxLimit = minMax.MaxLimit;

                float minEndRectWidth = minMax.ShowLimits
                    ? 3 + MinMaxRectWidthPerDigit * minMax.MinLimit.ToString(CultureInfo.InvariantCulture).Length
                    : 3;
                float maxEndRectWidth = minMax.ShowLimits
                    ? 3 + MinMaxRectWidthPerDigit * minMax.MaxLimit.ToString(CultureInfo.InvariantCulture).Length
                    : 3;

                Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                Rect minValueRect = new Rect(position.x + labelRect.width, position.y, minMax.HideValues ? 0 : ValueRectWidth, position.height);
                Rect minEndRect = new Rect(position.x + labelRect.width + minValueRect.width, position.y,
                    minEndRectWidth, position.height);

                Rect sliderRect = new Rect(position.x + labelRect.width + minValueRect.width + minEndRect.width, position.y, 
                    position.width - labelRect.width - 2 * minValueRect.width - minEndRectWidth - maxEndRectWidth, position.height);

                Rect maxEndRect = new Rect(position.x + labelRect.width + minValueRect.width + minEndRect.width + sliderRect.width, position.y,
                    maxEndRectWidth, position.height);
                Rect maxValueRect = new Rect(position.x + labelRect.width + minValueRect.width + minEndRect.width + sliderRect.width + maxEndRect.width, position.y, minMax.HideValues ? 0 : ValueRectWidth, position.height);
                

                EditorGUI.LabelField(labelRect, label);

                if (!minMax.HideValues)
                {
                    minValue = property.type != "IntRange" 
                        ? EditorGUI.DelayedFloatField(minValueRect, GUIContent.none, minValue) 
                        : EditorGUI.DelayedIntField(minValueRect, GUIContent.none, (int)minValue);

                    minValue = Mathf.Clamp(minValue, minMax.MinLimit, minMax.MaxLimit);
                    if (minValue > maxValue)
                        maxValue = minValue;

                    maxValue = property.type != "IntRange"
                        ? EditorGUI.DelayedFloatField(maxValueRect, GUIContent.none, maxValue)
                        : EditorGUI.DelayedIntField(maxValueRect, GUIContent.none, (int)maxValue);

                    maxValue = Mathf.Clamp(maxValue, minMax.MinLimit, minMax.MaxLimit);
                    if (maxValue < minValue)
                        minValue = maxValue;
                }

                if (minMax.ShowLimits)
                {
                    EditorGUI.LabelField(minEndRect, $"{minMax.MinLimit}", EndRangeStyle);
                    EditorGUI.LabelField(maxEndRect, $"{minMax.MaxLimit}", EndRangeStyle);
                }
                else
                {
                    EditorGUI.LabelField(minEndRect, "");
                    EditorGUI.LabelField(maxEndRect, "");
                }

                EditorGUI.MinMaxSlider(sliderRect, GUIContent.none, ref minValue, ref maxValue, minLimit, maxLimit);
                SetMinMaxValues(property, minValue, maxValue);

                if (minMax.ShowSize)
                {
                    position.y += EditorGUIUtility.singleLineHeight;
                    Rect drawRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);

                    GUI.enabled = false;
                    float defaultLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 90;
                    EditorGUI.FloatField(drawRect, "Selected Size", maxValue - minValue);
                    EditorGUIUtility.labelWidth = defaultLabelWidth;
                    GUI.enabled = true;
                }
            }
            else
            {
                if(!_loggedWarning)
                {
                    Debug.LogWarning($"The MinMaxAttribute cannot be assigned to a field of type '{property.type}'! It is only assignable to the following types: " +
                                 $"{string.Join(", ", "Vector2", "Range", "IntRange", "Vector2Int")}");
                    _loggedWarning = true;
                }
                
                EditorGUI.PropertyField(position, property, label, true);
            }
        }


        private static bool TryGetMinValue(SerializedProperty property, out float value)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                value = property.vector2Value.x;
                return true;
            }

            if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                value = property.vector2IntValue.x;
                return true;
            }

            if (property.type == "Range")
            {
                value = property.FindPropertyRelative("_fromValue").floatValue;
                return true;
            }

            if (property.type == "IntRange")
            {
                value = property.FindPropertyRelative("_fromValue").intValue;
                return true;
            }

            value = default(float);
            return false;
        }

        private static bool TryGetMaxValue(SerializedProperty property, out float value)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                value = property.vector2Value.y;
                return true;
            }

            if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                value = property.vector2IntValue.y;
                return true;
            }

            if (property.type == "Range")
            {
                value = property.FindPropertyRelative("_toValue").floatValue;
                return true;
            }

            if (property.type == "IntRange")
            {
                value = property.FindPropertyRelative("_toValue").intValue;
                return true;
            }

            value = default(float);
            return false;
        }

        private static void SetMinMaxValues(SerializedProperty property, float minValue, float maxValue)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                property.vector2Value = new Vector2(minValue, maxValue);
            }

            if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                property.vector2IntValue = new Vector2Int((int) minValue, (int) maxValue);
            }

            if (property.type == "Range")
            {
                property.FindPropertyRelative("_fromValue").floatValue = minValue;
                property.FindPropertyRelative("_toValue").floatValue = maxValue;
            }

            if (property.type == "IntRange")
            {
                property.FindPropertyRelative("_fromValue").intValue = (int) minValue;
                property.FindPropertyRelative("_toValue").intValue = (int) maxValue;
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            MinMaxAttribute minMax = attribute as MinMaxAttribute;
            return EditorGUIUtility.singleLineHeight * (minMax.ShowSize ? 2 : 1);
        }


        public void OnBeforeSerialize()
        {
            _loggedWarning = false;
        }

        public void OnAfterDeserialize() { }
    }
}