using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class InspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MonoBehaviour mono = (MonoBehaviour)target;
        MethodInfo[] methods = mono.GetType().GetMethods(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (MethodInfo method in methods)
        {
            InspectorButtonAttribute button =
                method.GetCustomAttribute<InspectorButtonAttribute>();

            if (button == null) continue;
            if (method.GetParameters().Length != 0) continue;

            string label = string.IsNullOrEmpty(button.Label)
                ? ObjectNames.NicifyVariableName(method.Name)
                : button.Label;

            if (GUILayout.Button(label))
            {
                method.Invoke(mono, null);
            }
        }
    }
}