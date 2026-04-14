using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumStructArray<,>), true)]
public class EnumStructArrayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty values = property.FindPropertyRelative("values");

        Type enumType = fieldInfo.FieldType.GetGenericArguments()[0];
        string[] names = Enum.GetNames(enumType);

        if (values == null) return EditorGUIUtility.singleLineHeight;

        if (values.arraySize != names.Length)
            values.arraySize = names.Length;

        float height = EditorGUIUtility.singleLineHeight;

        if (property.isExpanded)
        {
            for (int i = 0; i < values.arraySize; i++)
            {
                SerializedProperty element = values.GetArrayElementAtIndex(i);
                height += EditorGUI.GetPropertyHeight(element, true);
            }
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty values = property.FindPropertyRelative("values");

        Type enumType = fieldInfo.FieldType.GetGenericArguments()[0];
        string[] names = Enum.GetNames(enumType);

        if (values == null) return;

        if (values.arraySize != names.Length)
            values.arraySize = names.Length;

        Rect rect = position;
        rect.height = EditorGUIUtility.singleLineHeight;

        // <-- CHANGE HERE: use Foldout instead of BeginFoldoutHeaderGroup
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
        rect.y += rect.height;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < values.arraySize; i++)
            {
                SerializedProperty element = values.GetArrayElementAtIndex(i);
                rect.height = EditorGUI.GetPropertyHeight(element, true);
                EditorGUI.PropertyField(rect, element, new GUIContent(names[i]), true);
                rect.y += rect.height;
            }
            EditorGUI.indentLevel--;
        }
    }
}