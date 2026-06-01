using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PauseService))]
public class PauseServiceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "_excludedScenes");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Excluded Scenes", EditorStyles.boldLabel);

        var allScenes = typeof(SceneDatabase.Scenes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue())
            .ToArray();

        var prop = serializedObject.FindProperty("_excludedScenes");

        var selected = new HashSet<string>();
        for (int i = 0; i < prop.arraySize; i++)
            selected.Add(prop.GetArrayElementAtIndex(i).stringValue);

        foreach (var scene in allScenes)
        {
            bool newValue = EditorGUILayout.ToggleLeft(scene, selected.Contains(scene));
            if (newValue) selected.Add(scene);
            else selected.Remove(scene);
        }

        prop.ClearArray();
        int idx = 0;
        foreach (var scene in selected)
        {
            prop.InsertArrayElementAtIndex(idx);
            prop.GetArrayElementAtIndex(idx).stringValue = scene;
            idx++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
