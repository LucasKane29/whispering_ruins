using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CrystalGlow))]
public class CrystalGlowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        CrystalGlow glow = (CrystalGlow)target;

        if (GUILayout.Button("Test Glow"))
            glow.Activate();
    }
}
