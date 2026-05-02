#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InlineSOAttribute))]
public sealed class InlineSODrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded || property.objectReferenceValue == null)
        {
            return height;
        }

        SerializedObject so = new SerializedObject(property.objectReferenceValue);
        SerializedProperty iterator = so.GetIterator();

        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            if (iterator.name == "m_Script")
            {
                enterChildren = false;
                continue;
            }

            height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;

            enterChildren = false;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect lineRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );

        Rect foldoutRect = new Rect(
            lineRect.x,
            lineRect.y,
            14f,
            lineRect.height
        );

        Rect labelRect = new Rect(
            lineRect.x + 14f,
            lineRect.y,
            EditorGUIUtility.labelWidth - 14f,
            lineRect.height
        );

        Rect objectRect = new Rect(
            position.x + EditorGUIUtility.labelWidth,
            lineRect.y,
            position.width - EditorGUIUtility.labelWidth,
            lineRect.height
        );

        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none, true);
        EditorGUI.LabelField(labelRect, label);

        property.objectReferenceValue = EditorGUI.ObjectField(
            objectRect,
            property.objectReferenceValue,
            fieldInfo.FieldType,
            false
        );

        if (property.isExpanded && property.objectReferenceValue != null)
        {
            EditorGUI.indentLevel++;

            SerializedObject so = new SerializedObject(property.objectReferenceValue);
            SerializedProperty iterator = so.GetIterator();

            float y = lineRect.yMax + EditorGUIUtility.standardVerticalSpacing;

            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.name == "m_Script")
                {
                    enterChildren = false;
                    continue;
                }

                float propertyHeight = EditorGUI.GetPropertyHeight(iterator, true);

                Rect propertyRect = new Rect(
                    position.x,
                    y,
                    position.width,
                    propertyHeight
                );

                EditorGUI.PropertyField(propertyRect, iterator, true);

                y += propertyHeight + EditorGUIUtility.standardVerticalSpacing;

                enterChildren = false;
            }

            so.ApplyModifiedProperties();

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}
#endif