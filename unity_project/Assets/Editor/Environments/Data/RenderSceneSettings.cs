using UnityEngine;
using UnityEngine.Rendering;

public class RenderSceneSettings : EnvironmentSceneSettings
{
    public Color SubtractiveShadowColor { set { m_SubtractiveShadowColor.Value = value; } }
    public Material SkyboxMaterial { set { m_SkyboxMaterial.Value = value; } }
    public Light SunSource { set { m_SunSource.Value = value; } }

    public AmbientMode AmbientSource { set { m_AmbientSource.Value = value; } }
    public Color AmbientSkyColor { set { m_AmbientSkyColor.Value = value; } }
    public Color AmbientEquatorColor { set { m_AmbientEquatorColor.Value = value; } }
    public Color AmbientGroundColor { set { m_AmbientGroundColor.Value = value; } }
    public float AmbientIntensity { set { m_AmbientIntensity.Value = value; } }

    public DefaultReflectionMode ReflectionMode { set { m_DefaultReflectionMode.Value = value; } }
    public int ReflectionResolution { set { m_DefaultReflectionResolution.Value = value; } }
    public Cubemap CustomReflection { set { m_CustomReflection.Value = value; } }
    public float ReflectionIntensity { set { m_ReflectionIntensity.Value = value; } }
    public int ReflectionBounces { set { m_ReflectionBounces.Value = value; } }

    private SerializableProperty<Color> m_SubtractiveShadowColor;
    private SerializableProperty<Material> m_SkyboxMaterial;
    private SerializableProperty<Light> m_SunSource;

    private SerializableProperty<AmbientMode> m_AmbientSource;
    private SerializableProperty<Color> m_AmbientSkyColor;
    private SerializableProperty<Color> m_AmbientEquatorColor;
    private SerializableProperty<Color> m_AmbientGroundColor;
    private SerializableProperty<float> m_AmbientIntensity;

    private SerializableProperty<DefaultReflectionMode> m_DefaultReflectionMode;
    private SerializableProperty<int> m_DefaultReflectionResolution;
    private SerializableProperty<Cubemap> m_CustomReflection;
    private SerializableProperty<float> m_ReflectionIntensity;
    private SerializableProperty<int> m_ReflectionBounces;

    public RenderSceneSettings() : base(typeof(RenderSettings), "GetRenderSettings")
    {
        m_SubtractiveShadowColor = GetProperty<Color>("m_SubtractiveShadowColor");
        m_SkyboxMaterial = GetProperty<Material>("m_SkyboxMaterial");
        m_SunSource = GetProperty<Light>("m_Sun");

        m_AmbientSource = GetProperty<AmbientMode>("m_AmbientMode");
        m_AmbientSkyColor = GetProperty<Color>("m_AmbientSkyColor");
        m_AmbientEquatorColor = GetProperty<Color>("m_AmbientEquatorColor");
        m_AmbientGroundColor = GetProperty<Color>("m_AmbientGroundColor");
        m_AmbientIntensity = GetProperty<float>("m_AmbientIntensity");

        m_DefaultReflectionMode = GetProperty<DefaultReflectionMode>("m_DefaultReflectionMode");
        m_DefaultReflectionResolution = GetProperty<int>("m_DefaultReflectionResolution");
        m_CustomReflection = GetProperty<Cubemap>("m_CustomReflection");
        m_ReflectionIntensity = GetProperty<float>("m_ReflectionIntensity");
        m_ReflectionBounces = GetProperty<int>("m_ReflectionBounces");
    }

    public static bool Apply()
    {
        using (var settings = new RenderSceneSettings()
        {
            SubtractiveShadowColor = new Color(0.42f, 0.478f, 0.627f, 1f),
            SkyboxMaterial = null,
            SunSource = null,

            AmbientSource = AmbientMode.Flat,
            AmbientSkyColor = new Color(0.212f, 0.227f, 0.259f),
            AmbientEquatorColor = Color.black,
            AmbientGroundColor = Color.black,
            AmbientIntensity = 1,

            CustomReflection = null,
            ReflectionMode = DefaultReflectionMode.Custom,
            ReflectionResolution = 128,
            ReflectionBounces = 1,
            ReflectionIntensity = 1,
        })
        {
            return settings.ApplySerializedObjects();
        }
    }
}