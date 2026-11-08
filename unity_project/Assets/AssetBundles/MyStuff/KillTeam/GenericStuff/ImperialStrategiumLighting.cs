
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace KillTeam
{
    public class ImperialStrategiumLighting : MonoBehaviour
    {
        [Header("Room Dimensions")]
        public float roomWidth = 12f;
        public float roomLength = 16f;
        public float ceilingHeight = 6f;

        [Header("Table")]
        public Transform table;
        public float tableLightHeight = 4.5f;

        [Header("Main Table Light")]
        public Color tableLightColor = new Color(1f, 0.82f, 0.58f);
        public float tableLightIntensity = 3f;
        public float tableLightRange = 10f;
        public float tableSpotAngle = 75f;

        [Header("Cold Ceiling Lights")]
        public Color ceilingColor = new Color(0.25f, 0.4f, 0.8f);
        public float ceilingIntensity = 1.5f;
        public float ceilingRange = 10f;

        [Header("Red Pillar Lights")]

        [Header("Pillar Models")]
        [Tooltip("Drag pillar GameObjects from the Hierarchy here.")]
        public List<Transform> pillars = new List<Transform>();

        [Header("Pillar Light Settings")]
        public float pillarLightHeight = 0.5f;
        public float pillarLightTargetHeight = 4f;

        public Color pillarColor = new Color(0.8f, 0.05f, 0.02f);
        public float pillarIntensity = 2f;
        public float pillarRange = 5f;

        [Header("Orange Wall Lights")]
        public Color wallColor = new Color(1f, 0.35f, 0.08f);
        public float wallIntensity = 1.5f;
        public float wallRange = 5f;

        [Header("Candle Lights")]
        public bool generateCandles = true;
        public Color candleColor = new Color(1f, 0.48f, 0.12f);
        public float candleIntensity = 0.5f;
        public float candleRange = 2.5f;
        public bool flickerCandles = true;
        public float flickerAmount = 0.15f;
        public float flickerSpeed = 5f;

        [Header("Shadows")]
        public bool enableShadows = true;
        public LightShadows shadowType = LightShadows.Soft;

        [Header("Lightmap UV Generation")]
        [Min(0)]
        public int maxUVTriangleCount = 100000;

        [Header("Generated Lights")]
        [SerializeField]
        private List<Light> generatedLights = new List<Light>();

        private const string GeneratedRootName = "GeneratedLights";
        private const string CandlePrefix = "VFX_Candle_Flame";

        // ==================================================
        // CANDLE FLICKER
        // ==================================================

        private class CandleFlicker : MonoBehaviour
        {
            public Light targetLight;
            public float baseIntensity;
            public float amount;
            public float speed;
            public float seed;

            private void Update()
            {
                if (targetLight == null)
                    return;

                if (targetLight == null || !targetLight.enabled)
                    return;

                float noise = Mathf.PerlinNoise(
                    Time.time * speed,
                    seed
                );

                targetLight.intensity = Mathf.Max(
                    0f,
                    baseIntensity + (noise * 2f - 1f) * amount
                );
            }
        }

        // ==================================================
        // GENERATED ROOT
        // ==================================================

        private Transform GetGeneratedRoot()
        {
            Transform root = transform.Find(GeneratedRootName);

            if (root != null)
                return root;

            GameObject obj = new GameObject(GeneratedRootName);

            obj.transform.SetParent(transform, false);

            return obj.transform;
        }

        // ==================================================
        // CREATE LIGHT
        // ==================================================

        private Light CreateLight(
            string lightName,
            LightType type,
            Vector3 localPosition,
            Quaternion localRotation,
            Color color,
            float intensity,
            float range,
            bool castShadows)
        {
            GameObject obj = new GameObject(lightName);

            obj.transform.SetParent(GetGeneratedRoot(), false);
            obj.transform.localPosition = localPosition;
            obj.transform.localRotation = localRotation;

            Light light = obj.AddComponent<Light>();

            light.type = type;
            light.color = color;
            light.intensity = intensity;
            light.range = range;

            light.shadows = castShadows && enableShadows
                ? shadowType
                : LightShadows.None;

            generatedLights.Add(light);

            return light;
        }

        // ==================================================
        // CREATE SPOTLIGHT
        // ==================================================

        private Light CreateSpot(
            string name,
            Vector3 position,
            Vector3 target,
            Color color,
            float intensity,
            float range,
            float angle,
            bool shadows)
        {
            Vector3 direction = target - position;

            Quaternion rotation =
                direction.sqrMagnitude > 0.0001f
                    ? Quaternion.LookRotation(direction)
                    : Quaternion.identity;

            Light light = CreateLight(
                name,
                LightType.Spot,
                position,
                rotation,
                color,
                intensity,
                range,
                shadows
            );

            light.spotAngle = angle;

            return light;
        }

        // ==================================================
        // CREATE POINT LIGHT
        // ==================================================

        private Light CreatePoint(
            string name,
            Vector3 position,
            Color color,
            float intensity,
            float range,
            bool shadows)
        {
            return CreateLight(
                name,
                LightType.Point,
                position,
                Quaternion.identity,
                color,
                intensity,
                range,
                shadows
            );
        }

        // ==================================================
        // GENERATE CANDLE LIGHTS
        // ==================================================

        private void GenerateCandleLights()
        {
            if (!generateCandles)
                return;

            int candleIndex = 0;

            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    Transform[] transforms =
                        root.GetComponentsInChildren<Transform>(true);

                    foreach (Transform candleTransform in transforms)
                    {
                        if (!candleTransform.name.StartsWith(
                            CandlePrefix,
                            StringComparison.Ordinal))
                        {
                            continue;
                        }

                        Vector3 localPosition =
                            transform.InverseTransformPoint(
                                candleTransform.position
                            );

                        Light candle = CreatePoint(
                            "Candle_" + candleIndex,
                            localPosition,
                            candleColor,
                            candleIntensity,
                            candleRange,
                            false
                        );

                        if (flickerCandles)
                        {
                            CandleFlicker flicker =
                                candle.gameObject.AddComponent<CandleFlicker>();

                            flicker.targetLight = candle;
                            flicker.baseIntensity = candleIntensity;
                            flicker.amount = flickerAmount;
                            flicker.speed = flickerSpeed;
                            flicker.seed = candleIndex * 13.37f;
                        }

                        candleIndex++;
                    }
                }
            }

            Debug.Log(
                "Generated " + candleIndex + " candle lights.",
                this
            );
        }

        // ==================================================
        // GENERATE ALL LIGHTING
        // ==================================================

        public void GenerateLighting()
        {
            ClearLighting();

            float halfWidth = roomWidth * 0.5f;
            float halfLength = roomLength * 0.5f;

            Vector3 tablePosition =
                table != null
                    ? transform.InverseTransformPoint(table.position)
                    : Vector3.zero;

            Vector3 mainPosition =
                tablePosition + Vector3.up * tableLightHeight;

            // Main table spotlight.
            CreateSpot(
                "Main_Table_Spotlight",
                mainPosition,
                tablePosition,
                tableLightColor,
                tableLightIntensity,
                tableLightRange,
                tableSpotAngle,
                true
            );

            // Cold overhead lights.
            for (int i = 0; i < 2; i++)
            {
                float z = i == 0
                    ? -roomLength * 0.25f
                    : roomLength * 0.25f;

                Vector3 position = new Vector3(
                    0f,
                    ceilingHeight - 0.5f,
                    z
                );

                CreateSpot(
                    "Ceiling_Blue_" + i,
                    position,
                    new Vector3(0f, 0f, z),
                    ceilingColor,
                    ceilingIntensity,
                    ceilingRange,
                    90f,
                    false
                );
            }

            // Generate red lights for assigned pillar models.
            for (int i = 0; i < pillars.Count; i++)
            {
                Transform pillar = pillars[i];

                if (pillar == null)
                    continue;

                // Convert the pillar's world position into
                // the lighting manager's local space.
                Vector3 localPosition =
                    transform.InverseTransformPoint(pillar.position);

                Vector3 lightPosition =
                    localPosition + Vector3.up * pillarLightHeight;

                Vector3 targetPosition =
                    localPosition + Vector3.up * pillarLightTargetHeight;

                CreateSpot(
                    "Pillar_Red_" + i,
                    lightPosition,
                    targetPosition,
                    pillarColor,
                    pillarIntensity,
                    pillarRange,
                    55f,
                    false
                );
            }

            GenerateCandleLights();

            Debug.Log(
                "Generated " + generatedLights.Count + " lights.",
                this
            );
        }

        // ==================================================
        // CLEAR LIGHTING
        // ==================================================

        public void ClearLighting()
        {
            Transform root = transform.Find(GeneratedRootName);

            if (root != null)
            {
                root.SetParent(null);

                if (Application.isPlaying)
                    Destroy(root.gameObject);
                else
                    DestroyImmediate(root.gameObject);
            }

            generatedLights.Clear();
        }

        // ==================================================
        // ENABLE / DISABLE LIGHTS
        // ==================================================

        public void SetAllLightsEnabled(bool enabled)
        {
            Transform root = transform.Find(GeneratedRootName);

            if (root == null)
                return;

            Light[] lights =
                root.GetComponentsInChildren<Light>(true);

            foreach (Light light in lights)
            {
                if (light != null)
                    light.enabled = enabled;
            }
        }

#if UNITY_EDITOR

        // ==================================================
        // GET GENERATED LIGHTS
        // ==================================================

        private Light[] GetGeneratedLights()
        {
            Transform root = transform.Find(GeneratedRootName);

            if (root == null)
                return new Light[0];

            return root.GetComponentsInChildren<Light>(true);
        }

        // ==================================================
        // SET LIGHTS TO REALTIME OR BAKED
        // ==================================================

        public void SetAllLightsBakeType(LightmapBakeType bakeType)
        {
            Light[] lights = GetGeneratedLights();

            foreach (Light light in lights)
            {
                if (light == null)
                    continue;

                Undo.RecordObject(light, "Change Light Bake Type");

                light.lightmapBakeType = bakeType;

                EditorUtility.SetDirty(light);
            }

            if (gameObject.scene.IsValid())
                EditorSceneManager.MarkSceneDirty(gameObject.scene);

            Debug.Log(
                "Set " + lights.Length +
                " generated lights to " + bakeType,
                this
            );
        }

        // ==================================================
        // SET ALL SCENE LIGHTS TO SOFT SHADOWS
        // ==================================================

        public void SetAllLightsSoftShadows()
        {
            int count = 0;

            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    Light[] lights =
                        root.GetComponentsInChildren<Light>(true);

                    foreach (Light light in lights)
                    {
                        Undo.RecordObject(
                            light,
                            "Set Soft Shadows"
                        );

                        light.shadows = LightShadows.Soft;

                        EditorUtility.SetDirty(light);
                        count++;
                    }
                }

                EditorSceneManager.MarkSceneDirty(scene);
            }

            Debug.Log(
                "Set " + count + " lights to Soft Shadows.",
                this
            );
        }

        // ==================================================
        // GENERATE LIGHTMAP UVs
        // ==================================================

        public void GenerateLightmapUVsAll()
        {
            Dictionary<string, int> modelTriangles =
                new Dictionary<string, int>();

            int updatedCount = 0;
            int alreadyEnabled = 0;
            int skippedCount = 0;
            int failedCount = 0;

            // Collect model assets before reimporting anything.
            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    MeshFilter[] filters =
                        root.GetComponentsInChildren<MeshFilter>(true);

                    foreach (MeshFilter filter in filters)
                    {
                        AddModelMesh(
                            filter.sharedMesh,
                            modelTriangles
                        );
                    }

                    SkinnedMeshRenderer[] skinned =
                        root.GetComponentsInChildren<SkinnedMeshRenderer>(true);

                    foreach (SkinnedMeshRenderer renderer in skinned)
                    {
                        AddModelMesh(
                            renderer.sharedMesh,
                            modelTriangles
                        );
                    }
                }
            }

            List<string> paths =
                new List<string>(modelTriangles.Keys);

            try
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    string path = paths[i];

                    // Cancellation is checked between models.
                    bool cancel =
                        EditorUtility.DisplayCancelableProgressBar(
                            "Generating Lightmap UVs",
                            (i + 1) + " / " + paths.Count +
                            "\n" + path,
                            paths.Count == 0
                                ? 1f
                                : (float)i / paths.Count
                        );

                    if (cancel)
                    {
                        Debug.LogWarning(
                            "Lightmap UV generation cancelled."
                        );

                        break;
                    }

                    int triangles = modelTriangles[path];

                    if (maxUVTriangleCount > 0 &&
                        triangles > maxUVTriangleCount)
                    {
                        skippedCount++;

                        Debug.LogWarning(
                            "Skipped high-poly model (" +
                            triangles + " triangles): " + path,
                            this
                        );

                        continue;
                    }

                    ModelImporter importer =
                        AssetImporter.GetAtPath(path) as ModelImporter;

                    if (importer == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    if (importer.generateSecondaryUV)
                    {
                        alreadyEnabled++;
                        continue;
                    }

                    try
                    {
                        importer.generateSecondaryUV = true;
                        importer.SaveAndReimport();

                        updatedCount++;

                        Debug.Log(
                            "Generated Lightmap UVs: " + path,
                            this
                        );
                    }
                    catch (Exception exception)
                    {
                        failedCount++;

                        Debug.LogError(
                            "Failed to generate Lightmap UVs: " +
                            path + "\n" + exception,
                            this
                        );
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            Debug.Log(
                "========== LIGHTMAP UV RESULTS ==========\n" +
                "Models Found: " + paths.Count + "\n" +
                "Updated: " + updatedCount + "\n" +
                "Already Enabled: " + alreadyEnabled + "\n" +
                "Skipped: " + skippedCount + "\n" +
                "Failed: " + failedCount + "\n" +
                "========================================",
                this
            );

            EditorUtility.DisplayDialog(
                "Lightmap UV Generation Finished",
                "Models Updated: " + updatedCount + "\n" +
                "Already Enabled: " + alreadyEnabled + "\n" +
                "Skipped: " + skippedCount + "\n" +
                "Failed: " + failedCount,
                "OK"
            );
        }

        private void AddModelMesh(
            Mesh mesh,
            Dictionary<string, int> modelTriangles)
        {
            if (mesh == null)
                return;

            string path = AssetDatabase.GetAssetPath(mesh);

            if (string.IsNullOrEmpty(path))
                return;

            int triangleCount = 0;

            // Index counts are available without reading the
            // mesh vertex buffer.
            for (int subMesh = 0;
                 subMesh < mesh.subMeshCount;
                 subMesh++)
            {
                if (mesh.GetTopology(subMesh) == MeshTopology.Triangles)
                {
                    triangleCount +=
                        (int)(mesh.GetIndexCount(subMesh) / 3);
                }
            }

            if (modelTriangles.ContainsKey(path))
            {
                // Use the largest mesh encountered for this asset.
                modelTriangles[path] = Mathf.Max(
                    modelTriangles[path],
                    triangleCount
                );
            }
            else
            {
                modelTriangles.Add(path, triangleCount);
            }
        }

        // ==================================================
        // CONTRIBUTE GI CONTROLS
        // ==================================================

        public void MarkAllStatic()
        {
            SetAllObjectsStatic(true);
        }

        public void MarkAllNonStatic()
        {
            SetAllObjectsStatic(false);
        }

        private void SetAllObjectsStatic(bool makeStatic)
        {
            if (Application.isPlaying)
                return;

            int count = 0;

            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    Renderer[] renderers =
                        root.GetComponentsInChildren<Renderer>(true);

                    foreach (Renderer renderer in renderers)
                    {
                        if (!(renderer is MeshRenderer))
                            continue;

                        GameObject obj = renderer.gameObject;

                        Undo.RecordObject(
                            obj,
                            "Change Contribute GI"
                        );

                        StaticEditorFlags flags =
                            GameObjectUtility.GetStaticEditorFlags(obj);

                        if (makeStatic)
                            flags |= StaticEditorFlags.ContributeGI;
                        else
                            flags &= ~StaticEditorFlags.ContributeGI;

                        GameObjectUtility.SetStaticEditorFlags(
                            obj,
                            flags
                        );

                        EditorUtility.SetDirty(obj);
                        count++;
                    }
                }

                EditorSceneManager.MarkSceneDirty(scene);
            }

            Debug.Log(
                "Updated Contribute GI on " +
                count + " objects.",
                this
            );
        }

        // ==================================================
        // FAST / GOOD QUALITY PRESETS
        // ==================================================

        public void SetBakeQuality(bool goodQuality)
        {
            if (Application.isPlaying || Lightmapping.isRunning)
                return;

            LightingSettings settings =
                Lightmapping.lightingSettings;

            if (settings == null)
            {
                EditorUtility.DisplayDialog(
                    "Lighting Settings Missing",
                    "Create and assign a Lighting Settings Asset " +
                    "in Window > Rendering > Lighting.",
                    "OK"
                );

                return;
            }

            Undo.RecordObject(
                settings,
                "Change Lighting Quality"
            );

            settings.bakedGI = true;

            settings.lightmapper =
                LightingSettings.Lightmapper.ProgressiveGPU;

            settings.lightmapResolution =
                goodQuality ? 30f : 10f;

            settings.lightmapMaxSize =
                goodQuality ? 2048 : 1024;

            settings.lightmapPadding =
                goodQuality ? 3 : 2;

            settings.directSampleCount =
                goodQuality ? 128 : 32;

            settings.indirectSampleCount =
                goodQuality ? 256 : 64;

            settings.environmentSampleCount =
                goodQuality ? 256 : 64;

            settings.maxBounces =
                goodQuality ? 3 : 2;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            Debug.Log(
                goodQuality
                    ? "GOOD QUALITY bake settings applied."
                    : "FAST bake settings applied.",
                this
            );
        }

        // ==================================================
        // CHECK LIGHTING SETTINGS
        // ==================================================

        public void DebugLightingSettings()
        {
            LightingSettings settings =
                Lightmapping.lightingSettings;

            if (settings == null)
            {
                Debug.LogWarning(
                    "No Lighting Settings Asset assigned.",
                    this
                );

                return;
            }

            Debug.Log(
                "========== LIGHTING SETTINGS ==========\n" +
                "Asset: " + settings.name + "\n" +
                "Path: " + AssetDatabase.GetAssetPath(settings) + "\n" +
                "Bake Running: " + Lightmapping.isRunning + "\n" +
                "Baked Lightmaps: " +
                LightmapSettings.lightmaps.Length + "\n" +
                "======================================",
                this
            );

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }

        // ==================================================
        // BAKE LIGHTING
        // ==================================================

        public void BakeLighting()
        {
            if (Application.isPlaying || Lightmapping.isRunning)
                return;

            if (Lightmapping.lightingSettings == null)
            {
                EditorUtility.DisplayDialog(
                    "Lighting Settings Missing",
                    "Assign a Lighting Settings Asset before baking.",
                    "OK"
                );

                return;
            }

            if (!EditorSceneManager
                .SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            // Prevent duplicate subscriptions.
            Lightmapping.bakeCompleted -= OnBakeCompleted;

            // Register before starting the bake.
            Lightmapping.bakeCompleted += OnBakeCompleted;

            bool started = Lightmapping.BakeAsync();

            if (!started)
            {
                Lightmapping.bakeCompleted -= OnBakeCompleted;

                Debug.LogWarning(
                    "Lighting bake could not be started.",
                    this
                );

                return;
            }

            Debug.Log("Lighting bake started.", this);
        }

        // ==================================================
        // BAKE COMPLETED POPUP
        // ==================================================

        private void OnBakeCompleted()
        {
            // Remove callback so it only runs once.
            Lightmapping.bakeCompleted -= OnBakeCompleted;

            Debug.Log(
                "Imperial Strategium lighting bake completed!",
                this
            );

            EditorUtility.DisplayDialog(
                "Lighting Bake Complete",
                "Your Imperial Strategium lightmaps " +
                "have finished baking!",
                "OK"
            );
        }

#endif
    }

#if UNITY_EDITOR

    // ======================================================
    // CUSTOM INSPECTOR - SAME FILE
    // ======================================================

    [CustomEditor(typeof(ImperialStrategiumLighting))]
    public class ImperialStrategiumLightingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ImperialStrategiumLighting lighting =
                (ImperialStrategiumLighting)target;

            EditorGUILayout.Space(12);

            // ----------------------------------------------
            // LIGHT GENERATION
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Lighting Generation",
                EditorStyles.boldLabel
            );

            if (GUILayout.Button(
                "Generate / Rebuild Lighting",
                GUILayout.Height(35)))
            {
                lighting.GenerateLighting();
                EditorUtility.SetDirty(lighting);
            }

            if (GUILayout.Button(
                "Clear All Lighting",
                GUILayout.Height(30)))
            {
                lighting.ClearLighting();
                EditorUtility.SetDirty(lighting);
            }

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // LIGHT MODES
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Light Modes",
                EditorStyles.boldLabel
            );

            if (GUILayout.Button(
                "Realtime Lights ALL",
                GUILayout.Height(35)))
            {
                lighting.SetAllLightsBakeType(
                    LightmapBakeType.Realtime
                );
            }

            if (GUILayout.Button(
                "Baking Lighting ALL",
                GUILayout.Height(35)))
            {
                lighting.SetAllLightsBakeType(
                    LightmapBakeType.Baked
                );
            }

            if (GUILayout.Button(
                "Set ALL Lights Soft Shadows",
                GUILayout.Height(35)))
            {
                lighting.SetAllLightsSoftShadows();
            }

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // LIGHTMAP UV TOOLS
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Lightmap UV Tools",
                EditorStyles.boldLabel
            );

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            if (GUILayout.Button(
                "Generate Lightmap UVs ALL",
                GUILayout.Height(35)))
            {
                lighting.GenerateLightmapUVsAll();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // STATIC OBJECT CONTROLS
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Static Object Controls",
                EditorStyles.boldLabel
            );

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            if (GUILayout.Button(
                "Mark All Static (Contribute GI)",
                GUILayout.Height(35)))
            {
                lighting.MarkAllStatic();
            }

            if (GUILayout.Button(
                "Mark All Non Static (Remove GI)",
                GUILayout.Height(35)))
            {
                lighting.MarkAllNonStatic();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.HelpBox(
                "These buttons change Contribute GI on every " +
                "MeshRenderer in all loaded scenes. " +
                "Do not use Mark All Static while movable " +
                "VR objects and miniatures are present.",
                MessageType.Warning
            );

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // LIGHTING QUALITY
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Lighting Quality",
                EditorStyles.boldLabel
            );

            EditorGUI.BeginDisabledGroup(
                Application.isPlaying || Lightmapping.isRunning
            );

            if (GUILayout.Button(
                "FAST BAKE",
                GUILayout.Height(35)))
            {
                lighting.SetBakeQuality(false);
            }

            if (GUILayout.Button(
                "GOOD QUALITY BAKE",
                GUILayout.Height(35)))
            {
                lighting.SetBakeQuality(true);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.HelpBox(
                "FAST: 10 texels/unit, 32 direct samples.\n" +
                "GOOD: 30 texels/unit, 128 direct samples.\n" +
                "Select a preset, then press BAKE LIGHTING NOW.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // LIGHTMAP BAKING
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Lightmap Baking",
                EditorStyles.boldLabel
            );

            EditorGUI.BeginDisabledGroup(
                Application.isPlaying || Lightmapping.isRunning
            );

            if (GUILayout.Button(
                "BAKE LIGHTING NOW",
                GUILayout.Height(40)))
            {
                lighting.BakeLighting();
            }

            EditorGUI.EndDisabledGroup();

            if (Lightmapping.isRunning)
            {
                EditorGUILayout.HelpBox(
                    "Lighting bake is currently running.",
                    MessageType.Info
                );
            }

            if (GUILayout.Button(
                "Check Lighting Settings Asset",
                GUILayout.Height(30)))
            {
                lighting.DebugLightingSettings();
            }

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // LIGHT VISIBILITY
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Light Visibility",
                EditorStyles.boldLabel
            );

            if (GUILayout.Button("Enable All Lights"))
            {
                lighting.SetAllLightsEnabled(true);
            }

            if (GUILayout.Button("Disable All Lights"))
            {
                lighting.SetAllLightsEnabled(false);
            }
        }
    }

#endif
}
