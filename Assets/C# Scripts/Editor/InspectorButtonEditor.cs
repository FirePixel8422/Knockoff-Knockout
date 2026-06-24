using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class InspectorButtonDrawer
{
    private class MethodCacheEntry
    {
        public MethodInfo method;
        public ParameterInfo[] parameters;
        public object[] args;
        public bool expanded;
    }

    private static readonly Dictionary<(object, MethodInfo), MethodCacheEntry> cache = new();

    public static void Draw(object obj)
    {
        if (obj == null)
        {
            return;
        }

        MethodInfo[] methods = obj.GetType().GetMethods(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (MethodInfo method in methods)
        {
            InspectorButtonAttribute button =
                method.GetCustomAttribute<InspectorButtonAttribute>();

            if (button == null)
            {
                continue;
            }

            bool allowed = button.AllowUsageOutsidePlayMode || EditorApplication.isPlaying;

            var key = (obj, method);

            if (!cache.TryGetValue(key, out MethodCacheEntry entry))
            {
                ParameterInfo[] parameters = method.GetParameters();

                entry = new MethodCacheEntry
                {
                    method = method,
                    parameters = parameters,
                    args = new object[parameters.Length],
                    expanded = false
                };

                cache[key] = entry;
            }

            bool hasParams = entry.parameters.Length > 0;
            if (hasParams)
            {
                EditorGUILayout.BeginVertical("box");
            }

            bool prevGUI = GUI.enabled;
            GUI.enabled = allowed;

            string label = string.IsNullOrEmpty(button.Label)
                ? ObjectNames.NicifyVariableName(method.Name)
                : button.Label;
            float height = 22;

            if (GUILayout.Button(label, GUILayout.Height(height)))
            {
                RecordUndo(obj, label);

                entry.method.Invoke(obj, entry.args.Length == 0 ? null : entry.args);

                EditorUtility.SetDirty(obj as UnityEngine.Object);
            }

            GUI.enabled = prevGUI;

            if (entry.parameters.Length > 0)
            {
                EditorGUI.indentLevel++;

                entry.expanded = EditorGUILayout.Foldout(
                    entry.expanded,
                    "Parameters",
                    true
                );

                if (entry.expanded)
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < entry.parameters.Length; i++)
                    {
                        EditorGUI.BeginChangeCheck();

                        entry.args[i] = DrawParameter(entry.parameters[i], entry.args[i]);

                        if (EditorGUI.EndChangeCheck())
                        {
                            RecordUndo(obj, "Modify Parameters");
                            EditorUtility.SetDirty(obj as UnityEngine.Object);
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

            if (hasParams)
            {
                EditorGUILayout.EndVertical();
            }
        }
    }
    private static object DrawParameter(ParameterInfo param, object current)
    {
        Type type = param.ParameterType;

        if (type == typeof(int))
            return EditorGUILayout.IntField(param.Name, current != null ? (int)current : 0);

        if (type == typeof(float))
            return EditorGUILayout.FloatField(param.Name, current != null ? (float)current : 0f);

        if (type == typeof(string))
            return EditorGUILayout.TextField(param.Name, current as string ?? "");

        if (type == typeof(bool))
            return EditorGUILayout.Toggle(param.Name, current != null && (bool)current);

        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            return EditorGUILayout.ObjectField(param.Name, current as UnityEngine.Object, type, true);

        if (type.IsEnum)
        {
            Enum e = current as Enum ?? (Enum)Activator.CreateInstance(type);

            if (Attribute.IsDefined(type, typeof(FlagsAttribute)))
                return EditorGUILayout.EnumFlagsField(param.Name, e);

            return EditorGUILayout.EnumPopup(param.Name, e);
        }

        EditorGUILayout.LabelField($"{param.Name} (unsupported: {type.Name})");
        return current;
    }
    private static void RecordUndo(object obj, string label)
    {
        UnityEngine.Object uObj = obj as UnityEngine.Object;

        if (uObj == null)
        {
            return;
        }

        Undo.RecordObject(uObj, label);
    }
}