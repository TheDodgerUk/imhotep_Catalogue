using UnityEditor;

public class GlobalIlluminationSceneSettings : EnvironmentSceneSettings
{
    public enum AmbientLightingMode { Realtime, Baked };
    public enum GIWorkflowMode { AutoGenerate, Manual };

    public bool AutoGenerateLightmaps { set { m_GIWorkflowMode.Value = value ? GIWorkflowMode.AutoGenerate : GIWorkflowMode.Manual; } }

    public float BounceScale { set { m_BounceScale.Value = value; } }
    public float IndirectIntensity { set { m_IndirectOutputScale.Value = value; } }
    public float AlbedoBoost { set { m_AlbedoBoost.Value = value; } }

    public AmbientLightingMode AmbientMode { set { m_EnvironmentLightingMode.Value = value; } }
    public bool EnableBakedLightmaps { set { m_EnableBakedLightmaps.Value = value; } }
    public bool EnableRealtimeLightmaps { set { m_EnableRealtimeLightmaps.Value = value; } }

    private SerializableProperty<GIWorkflowMode> m_GIWorkflowMode;

    private SerializableProperty<float> m_BounceScale;
    private SerializableProperty<float> m_IndirectOutputScale;
    private SerializableProperty<float> m_AlbedoBoost;

    private SerializableProperty<AmbientLightingMode> m_EnvironmentLightingMode;
    private SerializableProperty<bool> m_EnableBakedLightmaps;
    private SerializableProperty<bool> m_EnableRealtimeLightmaps;

    public GlobalIlluminationSceneSettings() : base(typeof(LightmapEditorSettings), "GetLightmapSettings")
    {
        var giSettings = m_SerializedObject.FindProperty("m_GISettings");

        m_GIWorkflowMode = GetProperty<GIWorkflowMode>("m_GIWorkflowMode");

        m_BounceScale = GetProperty<float>(giSettings, "m_BounceScale");
        m_IndirectOutputScale = GetProperty<float>(giSettings, "m_IndirectOutputScale");
        m_AlbedoBoost = GetProperty<float>(giSettings, "m_AlbedoBoost");

        m_EnvironmentLightingMode = GetProperty<AmbientLightingMode>(giSettings, "m_EnvironmentLightingMode");
        m_EnableBakedLightmaps = GetProperty<bool>(giSettings, "m_EnableBakedLightmaps");
        m_EnableRealtimeLightmaps = GetProperty<bool>(giSettings, "m_EnableRealtimeLightmaps");
    }

    public static bool Apply()
    {
        using (var settings = new GlobalIlluminationSceneSettings()
        {
            AutoGenerateLightmaps = false,

            BounceScale = 1,
            IndirectIntensity = 1,
            AlbedoBoost = 1,

            AmbientMode = AmbientLightingMode.Baked,
            EnableBakedLightmaps = true,
            EnableRealtimeLightmaps = false,
        })
        {
            return settings.ApplySerializedObjects();
        }
    }
}
