
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KillTeamLightingSetup : EditorWindow
{
    private const string LightName = "KillTeam Directional Light";
    private const string VolumeName = "KillTeam Global Volume";

    private float lightIntensity = 1.3f;
    private float shadowStrength = 0.85f;
    private float bloomIntensity = 0.2f;
    private float contrast = 10f;

    [MenuItem("Tools/Kill Team/Lighting Setup")]
    public static void OpenWindow()
    {
        GetWindow<KillTeamLightingSetup>(
            "Kill Team Lighting");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(
            "Kill Team VR Lighting",
            EditorStyles.boldLabel);

        EditorGUILayout.Space(10);

        lightIntensity = EditorGUILayout.Slider(
            "Light Intensity", lightIntensity, 0f, 3f);

        shadowStrength = EditorGUILayout.Slider(
            "Shadow Strength", shadowStrength, 0f, 1f);

        bloomIntensity = EditorGUILayout.Slider(
            "Bloom Intensity", bloomIntensity, 0f, 1f);

        contrast = EditorGUILayout.Slider(
            "Contrast", contrast, -50f, 50f);

        EditorGUILayout.Space(15);

        if (GUILayout.Button(
            "Apply Kill Team Lighting",
            GUILayout.Height(40)))
        {
            ApplyLighting();
        }
    }

    private void ApplyLighting()
    {
        SetupDirectionalLight();
        SetupAmbientLighting();
        SetupGlobalVolume();
        SetupURPShadows();

        Debug.Log(
            "Kill Team lighting setup complete.");
    }

    private void SetupDirectionalLight()
    {
        GameObject lightObject =
            GameObject.Find(LightName);

        if (lightObject == null)
        {
            lightObject = new GameObject(LightName);
            Undo.RegisterCreatedObjectUndo(
                lightObject, "Create Kill Team Light");
        }

        Light light = lightObject.GetComponent<Light>();

        if (light == null)
            light = Undo.AddComponent<Light>(lightObject);

        Undo.RecordObject(light, "Configure Light");
        Undo.RecordObject(
            lightObject.transform, "Rotate Light");

        light.type = LightType.Directional;
        light.color = new Color(1f, 0.94f, 0.83f);
        light.intensity = lightIntensity;
        light.shadows = LightShadows.Soft;
        light.shadowStrength = shadowStrength;

        lightObject.transform.rotation =
            Quaternion.Euler(50f, 330f, 0f);

        // Prevent a second directional light from
        // being created when the scene already has one.
        RenderSettings.sun = light;
    }

    private void SetupAmbientLighting()
    {
        RenderSettings.ambientMode =
            AmbientMode.Flat;

        RenderSettings.ambientLight =
            new Color(0.32f, 0.36f, 0.44f);

        RenderSettings.reflectionIntensity = 1f;
    }

    private void SetupGlobalVolume()
    {
        GameObject volumeObject =
            GameObject.Find(VolumeName);

        if (volumeObject == null)
        {
            volumeObject = new GameObject(VolumeName);
            Undo.RegisterCreatedObjectUndo(
                volumeObject, "Create Kill Team Volume");
        }

        Volume volume =
            volumeObject.GetComponent<Volume>();

        if (volume == null)
            volume = Undo.AddComponent<Volume>(
                volumeObject);

        Undo.RecordObject(
            volume, "Configure Kill Team Volume");

        volume.isGlobal = true;
        volume.priority = 10f;
        volume.weight = 1f;

        string folder = "Assets/KillTeamLighting";

        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder(
                "Assets", "KillTeamLighting");
        }

        string profilePath =
            folder + "/KillTeamVolumeProfile.asset";

        VolumeProfile profile =
            AssetDatabase.LoadAssetAtPath<VolumeProfile>(
                profilePath);

        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<
                VolumeProfile>();

            AssetDatabase.CreateAsset(
                profile, profilePath);
        }

        volume.sharedProfile = profile;

        Tonemapping tonemapping;

        if (!profile.TryGet(out tonemapping))
            tonemapping = profile.Add<Tonemapping>(true);

        tonemapping.active = true;
        tonemapping.mode.Override(
            TonemappingMode.ACES);

        Bloom bloom;

        if (!profile.TryGet(out bloom))
            bloom = profile.Add<Bloom>(true);

        bloom.active = true;
        bloom.intensity.Override(bloomIntensity);
        bloom.threshold.Override(1.1f);
        bloom.scatter.Override(0.6f);

        ColorAdjustments adjustments;

        if (!profile.TryGet(out adjustments))
        {
            adjustments =
                profile.Add<ColorAdjustments>(true);
        }

        adjustments.active = true;
        adjustments.contrast.Override(contrast);
        adjustments.saturation.Override(5f);

        EditorUtility.SetDirty(profile);
        EditorUtility.SetDirty(volume);
        AssetDatabase.SaveAssets();
    }


    private void SetupURPShadows()
    {
        UniversalRenderPipelineAsset urp =
            GraphicsSettings.currentRenderPipeline
                as UniversalRenderPipelineAsset;

        if (urp == null)
        {
            Debug.LogWarning("URP asset not found.");
            return;
        }

        Undo.RecordObject(urp, "Configure URP Shadows");

        SerializedObject serializedURP =
            new SerializedObject(urp);

        SetBool(serializedURP, "m_MainLightShadowsSupported", true);
        SetBool(serializedURP, "m_SoftShadowsSupported", true);

        // URP stores shadow resolution as an integer.
        SetInt(serializedURP, "m_MainLightShadowmapResolution", 2048);

        serializedURP.ApplyModifiedProperties();

        // These properties have public setters.
        urp.shadowDistance = 20f;
        urp.shadowCascadeCount = 2;

        EditorUtility.SetDirty(urp);
        AssetDatabase.SaveAssets();

        Debug.Log("URP shadow settings updated.");
    }

    private static void SetBool(
        SerializedObject obj,
        string propertyName,
        bool value)
    {
        SerializedProperty property =
            obj.FindProperty(propertyName);

        if (property != null)
            property.boolValue = value;
        else
            Debug.LogWarning($"Property not found: {propertyName}");
    }

    private static void SetInt(
        SerializedObject obj,
        string propertyName,
        int value)
    {
        SerializedProperty property =
            obj.FindProperty(propertyName);

        if (property != null)
            property.intValue = value;
        else
            Debug.LogWarning($"Property not found: {propertyName}");
    }

}

#endif
