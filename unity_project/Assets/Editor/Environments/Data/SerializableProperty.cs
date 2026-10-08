using UnityEditor;

public class SerializableProperty<T> : ISerializableProperty
{
    public string ValueString => Value?.ToString() ?? "(null)";
    public string Path => m_Property.propertyPath;
    public T Value { get; set; }

    private SerializedProperty m_Property;
    private T m_OriginalValue;

    public SerializableProperty(SerializedProperty property)
    {
        m_Property = property;
        Update();
    }

    public bool HasChanged()
    {
        if (m_OriginalValue == null && Value == null)
        {
            return false;
        }
        if (m_OriginalValue == null && Value != null)
        {
            return true;
        }
        if (m_OriginalValue != null && Value == null)
        {
            return true;
        }
        if (m_OriginalValue.Equals(Value) == false)
        {
            return true;
        }
        return false;
    }

    public void Update()
    {
        Value = (T)GetValue();
        m_OriginalValue = Value;
    }

    public void Apply()
    {
        if (HasChanged() == true)
        {
            SetValue(Value);
        }
    }

    private void SetValue(object value)
    {
        switch (m_Property.propertyType)
        {
            case SerializedPropertyType.Integer:
                m_Property.intValue = (int)value;
                break;
            case SerializedPropertyType.Boolean:
                m_Property.boolValue = (bool)value;
                break;
            case SerializedPropertyType.Float:
                m_Property.floatValue = (float)value;
                break;
            case SerializedPropertyType.String:
                m_Property.stringValue = (string)value;
                break;
            case SerializedPropertyType.Color:
                m_Property.colorValue = (UnityEngine.Color)value;
                break;
            case SerializedPropertyType.Enum:
                m_Property.enumValueIndex = (int)value;
                break;
            case SerializedPropertyType.ObjectReference:
                m_Property.objectReferenceValue = (UnityEngine.Object)value;
                break;
            case SerializedPropertyType.LayerMask:
                m_Property.intValue = (int)value;
                break;
            case SerializedPropertyType.Vector2:
                m_Property.vector2Value = (UnityEngine.Vector2)value;
                break;
            case SerializedPropertyType.Vector3:
                m_Property.vector3Value = (UnityEngine.Vector3)value;
                break;
            case SerializedPropertyType.Vector4:
                m_Property.vector4Value = (UnityEngine.Vector4)value;
                break;
            case SerializedPropertyType.Rect:
                m_Property.rectValue = (UnityEngine.Rect)value;
                break;
            case SerializedPropertyType.AnimationCurve:
                m_Property.animationCurveValue = (UnityEngine.AnimationCurve)value;
                break;
            case SerializedPropertyType.Bounds:
                m_Property.boundsValue = (UnityEngine.Bounds)value;
                break;
            case SerializedPropertyType.Quaternion:
                m_Property.quaternionValue = (UnityEngine.Quaternion)value;
                break;
            case SerializedPropertyType.Vector2Int:
                m_Property.vector2IntValue = (UnityEngine.Vector2Int)value;
                break;
            case SerializedPropertyType.Vector3Int:
                m_Property.vector3IntValue = (UnityEngine.Vector3Int)value;
                break;
            case SerializedPropertyType.RectInt:
                m_Property.rectIntValue = (UnityEngine.RectInt)value;
                break;
            case SerializedPropertyType.BoundsInt:
                m_Property.boundsIntValue = (UnityEngine.BoundsInt)value;
                break;

            case SerializedPropertyType.Generic:
            case SerializedPropertyType.ArraySize:
            case SerializedPropertyType.Character:
            case SerializedPropertyType.Gradient:
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.FixedBufferSize:
            default: break;
        }
    }

    private object GetValue()
    {
        switch (m_Property.propertyType)
        {
            case SerializedPropertyType.Integer: return m_Property.intValue;
            case SerializedPropertyType.Boolean: return m_Property.boolValue;
            case SerializedPropertyType.Float: return m_Property.floatValue;
            case SerializedPropertyType.String: return m_Property.stringValue;
            case SerializedPropertyType.Color: return m_Property.colorValue;
            case SerializedPropertyType.Enum: return m_Property.enumValueIndex;
            case SerializedPropertyType.ObjectReference: return m_Property.objectReferenceValue;
            case SerializedPropertyType.LayerMask: return m_Property.intValue;
            case SerializedPropertyType.Vector2: return m_Property.vector2Value;
            case SerializedPropertyType.Vector3: return m_Property.vector3Value;
            case SerializedPropertyType.Vector4: return m_Property.vector4Value;
            case SerializedPropertyType.Rect: return m_Property.rectValue;
            case SerializedPropertyType.AnimationCurve: return m_Property.animationCurveValue;
            case SerializedPropertyType.Bounds: return m_Property.boundsValue;
            case SerializedPropertyType.Quaternion: return m_Property.quaternionValue;
            case SerializedPropertyType.Vector2Int: return m_Property.vector2IntValue;
            case SerializedPropertyType.Vector3Int: return m_Property.vector3IntValue;
            case SerializedPropertyType.RectInt: return m_Property.rectIntValue;
            case SerializedPropertyType.BoundsInt: return m_Property.boundsIntValue;

            case SerializedPropertyType.Generic:
            case SerializedPropertyType.ArraySize:
            case SerializedPropertyType.Character:
            case SerializedPropertyType.Gradient:
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.FixedBufferSize:
            default: return null;
        }
    }
}

public interface ISerializableProperty
{
    string ValueString { get; }
    string Path { get; }

    bool HasChanged();

    void Apply();
}