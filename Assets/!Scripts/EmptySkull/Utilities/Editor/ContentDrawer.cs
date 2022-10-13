/*
Inspired by the work of "TheVastBernie".
Source: https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393/
*/

using EmptySkull.Utilities.Attributes;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EmptySkull.Utilities
{
    [CustomPropertyDrawer(typeof(DrawContentAttribute))]
    public class ContentDrawer : PropertyDrawer, ISerializationCallbackReceiver
    {
        private const string ScriptTypeString = "PPtr<MonoScript>";

        private GUIStyle _missingContentStyleValue;

        private GUIStyle MissingContentStyle
        {
            get
            {
                if (_missingContentStyleValue != null)
                    return _missingContentStyleValue;

                _missingContentStyleValue = EditorStyles.boldLabel;
                _missingContentStyleValue.alignment = TextAnchor.MiddleCenter;

                return _missingContentStyleValue;
            }
        }

        private Editor _editor;
        private bool _loggedWarning;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!(attribute is DrawContentAttribute drawContent))
                throw new UnityException(
                    $"Could not find any attribute of type {nameof(DrawContentAttribute)} to use inside the inspector!");

            EditorGUI.PropertyField(position, property, label, true);
            if (TryGetContentValue(property, out Object content))
            {
                bool noFoldout = drawContent.NoFoldout;
                if (!noFoldout)
                    property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);

                if (property.isExpanded || noFoldout)
                {
                    int indentLevelBefore = EditorGUI.indentLevel;
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.BeginVertical("Box");
                        {
                            if (content == null)
                            {
                                EditorGUILayout.LabelField("Assign an object to show the content.",
                                    MissingContentStyle);
                            }
                            else
                            {
                                if (_editor == null)
                                    Editor.CreateCachedEditor(content, null, ref _editor);

                                if (drawContent.AllowCustomInspector)
                                    _editor.OnInspectorGUI();
                                else
                                {
                                    SerializedObject sObj = _editor.serializedObject;
                                    sObj.Update();

                                    SerializedProperty sProp = sObj.GetIterator();
                                    sProp.NextVisible(true);
                                    do
                                    {
                                        if (sProp.type == ScriptTypeString)
                                            if (drawContent.ShowScriptField)
                                            {
                                                GUI.enabled = false;
                                                EditorGUILayout.PropertyField(sProp, true);
                                                GUI.enabled = true;

                                                continue;
                                            }
                                            else
                                            {
                                                continue;
                                            }


                                        EditorGUILayout.PropertyField(sProp, true);
                                    } while (sProp.NextVisible(false));

                                    sObj.ApplyModifiedProperties();
                                }
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUI.indentLevel = indentLevelBefore;
                }
            }
            else
            {
                if (_loggedWarning)
                    return;

                Debug.LogWarning
                (
                    content != null
                        ? $"The {nameof(DrawContentAttribute)} cannot be assigned to a field of type '{content.GetType()}'! It is only assignable to references of scriptable object assets!"
                        : $"The {nameof(DrawContentAttribute)} cannot be assigned to this field! It is only assignable to references of scriptable object assets!"
                );

                _loggedWarning = true;
            }
        }

        private static bool TryGetContentValue(SerializedProperty property, out Object value)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                value = property.objectReferenceValue;

                if (value is ScriptableObject)
                    return true;

                if (value == null)
                    return true;
            }

            value = default;
            return false;
        }

        public void OnBeforeSerialize() => _loggedWarning = false;

        public void OnAfterDeserialize()
        {
        }
    }
}