using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentSceneSettings : System.IDisposable
{
    protected List<ISerializableProperty> m_Properties;
    protected SerializedObject m_SerializedObject;

    public EnvironmentSceneSettings(System.Type settingsType, string methodName) : this(GetSettings(settingsType, methodName))
    {
    }

    public EnvironmentSceneSettings(Object obj)
    {
        m_SerializedObject = new SerializedObject(obj);
        m_Properties = new List<ISerializableProperty>();
    }

    public void Dispose()
    {
        m_SerializedObject.Dispose();
        m_SerializedObject = null;
    }

    public void UpdateSerializedObjects() => m_SerializedObject.UpdateIfRequiredOrScript();

    public bool ApplySerializedObjects()
    {
        var changed = m_Properties.FindAll(p => p.HasChanged());
        if (changed.Count > 0 || m_Properties.Count == 0)
        {
            m_Properties.ForEach(p => p.Apply());
            m_SerializedObject.ApplyModifiedProperties();
            Debug.Log($"Modified [{m_SerializedObject.targetObject.name}] on \"{SceneManager.GetActiveScene().name}\".\n");
            return true;
        }
        else
        {
            return false;
        }
    }

    protected SerializableProperty<T> GetProperty<T>(SerializedProperty property, string name)
    {
        return AddProperty<T>(property.FindPropertyRelative(name));
    }

    protected SerializableProperty<T> GetProperty<T>(string name)
    {
        return AddProperty<T>(m_SerializedObject.FindProperty(name));
    }

    private SerializableProperty<T> AddProperty<T>(SerializedProperty property)
    {
        var serialized = new SerializableProperty<T>(property);
        m_Properties.Add(serialized);
        return serialized;
    }

    private static Object GetSettings(System.Type settingsType, string methodName)
    {
        var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
        return settingsType.GetMethod(methodName, flags).Invoke(null, null) as Object;
    }
}
