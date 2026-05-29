using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneConstantAttribute))]
public class SceneConstantDrawer : PropertyDrawer
{
    private string[] _names;
    private string[] _values;

    private void EnsureInitialized()
    {
        if (_values != null) return;

        var attr = (SceneConstantAttribute)attribute;
        var fields = attr.SourceType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .ToArray();

        _names = fields.Select(f => f.Name).ToArray();
        _values = fields.Select(f => (string)f.GetRawConstantValue()).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnsureInitialized();

        int currentIndex = Array.IndexOf(_values, property.stringValue);
        if (currentIndex < 0) currentIndex = 0;

        EditorGUI.BeginProperty(position, label, property);
        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, _names);
        if (newIndex != currentIndex)
            property.stringValue = _values[newIndex];
        EditorGUI.EndProperty();
    }
}
