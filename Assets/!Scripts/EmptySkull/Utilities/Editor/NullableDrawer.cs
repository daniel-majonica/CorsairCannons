using UnityEditor;
using UnityEngine;

namespace EmptySkull.Utilities
{
    public class NullableDrawer : PropertyDrawer
    {
        private const string SetToNullPropKey = "SetToNull";
        private const string ValuePropKey = "_value";

        private const int _nullToggleWidth = 20;
        private const int _spacing = 2;

        private SerializedProperty _setToNullProp;
        private SerializedProperty _valueProp;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (_setToNullProp == null)
                _setToNullProp = property.FindPropertyRelative(SetToNullPropKey);
            if (_valueProp == null)
                _valueProp = property.FindPropertyRelative(ValuePropKey);

            Rect setToNullRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, _nullToggleWidth,
                position.height);
            Rect valueRect = new Rect(position.x + EditorGUIUtility.labelWidth + _nullToggleWidth + _spacing,
                position.y, position.width - (EditorGUIUtility.labelWidth + _nullToggleWidth + _spacing),
                position.height);

            EditorGUI.LabelField(position, label);
            _setToNullProp.boolValue = !EditorGUI.Toggle(setToNullRect, !_setToNullProp.boolValue);

            bool enabledBefore = GUI.enabled;
            if (_setToNullProp.boolValue)
                GUI.enabled = false;

            EditorGUI.PropertyField(valueRect, _valueProp, GUIContent.none);

            GUI.enabled = enabledBefore;

            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(NullableByte))]
    public class NullableByteDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableSByte))]
    public class NullableSByteDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableShort))]
    public class NullableShortDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableUShort))]
    public class NullableUShortDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableInt))]
    public class NullableIntDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableUInt))]
    public class NullableUIntDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableLong))]
    public class NullableLongDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableULong))]
    public class NullableULongDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableFloat))]
    public class NullableFloatDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableDouble))]
    public class NullableDoubleDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableDecimal))]
    public class NullableDecimalDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableChar))]
    public class NullableCharDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableString))]
    public class NullableStringDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableBool))]
    public class NullableBoolDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableGameObject))]
    public class NullableGameObjectDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableVector2))]
    public class NullableVector2Drawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableVector2Int))]
    public class NullableVector2IntDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableVector3))]
    public class NullableVector3Drawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableVector3Int))]
    public class NullableVector3IntDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableVector4))]
    public class NullableVector4Drawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableColor))]
    public class NullableColorDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableGradient))]
    public class NullableGradientDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableBounds))]
    public class NullableBoundsDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableBoundsInt))]
    public class NullableBoundsIntDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableAnimationCurve))]
    public class NullableAnimationCurveDrawer : NullableDrawer
    {
    }

    [CustomPropertyDrawer(typeof(NullableRect))]
    public class NullableRectDrawer : NullableDrawer
    {
    }
}