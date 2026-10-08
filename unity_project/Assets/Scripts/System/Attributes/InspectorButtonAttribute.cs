using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class InspectorButtonAttribute : PropertyAttribute
{
    public bool PlayModeOnly { get; private set; }

    public InspectorButtonAttribute() : this(false) { }
    public InspectorButtonAttribute(bool playModeOnly)
    {
        PlayModeOnly = playModeOnly;
    }
}
