using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class SceneConstantAttribute : PropertyAttribute
{
    public Type SourceType;
    public SceneConstantAttribute(Type sourceType)
    {
        SourceType = sourceType;
    }
}
