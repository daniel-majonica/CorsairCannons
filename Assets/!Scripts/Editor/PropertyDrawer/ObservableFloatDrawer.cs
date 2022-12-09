using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ObservableFloat))]
public class ObservableFloatDrawer : PropertyDrawer
{
    private const string ValuePropertyKey = "_value";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty sProp = property.FindPropertyRelative(ValuePropertyKey);

        float valueBefore = sProp.floatValue;

        float valueAfter = EditorGUI.FloatField(position, label, sProp.floatValue);

        if (valueBefore == valueAfter)
            return;

        //Before...

        sProp.floatValue = valueAfter;
        
        //After...
    }

    //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    //{
    //    return EditorGUIUtility.singleLineHeight;
    //}
}
