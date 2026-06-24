#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public sealed class ShowIfDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
        {
            return 0f;
        }

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    private bool ShouldShow(SerializedProperty property)
    {
        ShowIfAttribute attr = (ShowIfAttribute)attribute;

        SerializedProperty conditionProp = FindPropertyIncludingBacking(property, attr.condition);

        if (conditionProp == null)
        {
            return true;
        }

        return GetBool(conditionProp);
    }

    private static SerializedProperty FindPropertyIncludingBacking(SerializedProperty property, string name)
    {
        SerializedObject obj = property.serializedObject;

        // 1. direct lookup
        SerializedProperty direct = obj.FindProperty(name);
        if (direct != null)
        {
            return direct;
        }

        // 2. backing field lookup (auto property support)
        string backingName = $"<{name}>k__BackingField";
        SerializedProperty backing = obj.FindProperty(backingName);

        if (backing != null)
        {
            return backing;
        }

        // 3. fallback: scan fields via reflection (rare cases, nested types)
        return FindViaReflection(obj.targetObject, name);
    }

    private static SerializedProperty FindViaReflection(Object target, string name)
    {
        if (target == null)
        {
            return null;
        }

        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo field = target.GetType().GetField(name, flags);

        if (field == null)
        {
            return null;
        }

        return null; // Unity cannot convert FieldInfo → SerializedProperty reliably here
    }

    private static bool GetBool(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return prop.boolValue;

            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue != null;

            case SerializedPropertyType.Integer:
                return prop.intValue != 0;

            case SerializedPropertyType.Float:
                return prop.floatValue != 0f;

            case SerializedPropertyType.Enum:
                return prop.enumValueIndex != 0;

            default:
                return true;
        }
    }
}
#endif