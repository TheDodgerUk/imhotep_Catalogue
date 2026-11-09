
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace KillTeam
{
    public class ImperialStrategiumLighting : MonoBehaviour
    {
        // ==================================================
        // ROOM SETTINGS
        // ==================================================

        [Header("Room Dimensions")]
        public float roomWidth = 10;
        public float roomLength = 10f;
        public float ceilingHeight = 7f;

        [Header("Table")]
        public Transform table;
        public float tableLightHeight = 2.484f;

        [Header("Main Table Light")]
        public Color tableLightColor =
            new Color(1f, 0.82f, 0.58f);

        public float tableLightIntensity = 3f;
        public float tableLightRange = 10f;
        public float tableSpotAngle = 75f;

        // ==================================================
        // CEILING SETTINGS
        // ==================================================

        [Header("Cold Ceiling Lights")]
        public Color ceilingColor =
            new Color(0.25f, 0.4f, 0.8f);

        public float ceilingIntensity = 1.5f;
        public float ceilingRange = 10f;

        // ==================================================
        // AUTOMATIC PILLAR SEARCH
        // ==================================================

        [Header("Pillar Automatic Search")]

        [Tooltip(
            "Find all scene objects containing this text " +
            "anywhere in their name.")]
        public string pillarNameContains = "Pillar";

        [Tooltip(
            "Ignore child matches when a parent already " +
            "matches the pillar search.")]
        public bool ignoreNestedPillarMatches = true;

        [Tooltip(
            "Include inactive GameObjects when searching.")]
        public bool includeInactivePillars = true;

        [SerializeField]
        private List<Transform> foundPillars =
            new List<Transform>();

        public IReadOnlyList<Transform> FoundPillars
        {
            get { return foundPillars; }
        }

        [Header("Red Pillar Lights")]
        public Color pillarColor =
            new Color(0.8f, 0.05f, 0.02f);

        public float pillarIntensity = 2f;
        public float pillarRange = 5f;
        public float pillarSpotAngle = 55f;
        public float pillarLightHeight = 0.5f;
        public float pillarLightTargetHeight = 4f;

        // ==================================================
        // WALL SETTINGS
        // ==================================================

        [Header("Orange Wall Lights")]
        public Color wallColor =
            new Color(1f, 0.35f, 0.08f);

        public float wallIntensity = 1.5f;
        public float wallRange = 5f;

        // ==================================================
        // CANDLE SETTINGS
        // ==================================================

        [Header("Candle Lights")]
        public bool generateCandles = true;

        public Color candleColor =
            new Color(1f, 0.48f, 0.12f);

        public float candleIntensity = 0.5f;
        public float candleRange = 2.5f;

        public bool flickerCandles = true;
        public float flickerAmount = 0.15f;
        public float flickerSpeed = 5f;

        // ==================================================
        // SHADOW SETTINGS
        // ==================================================

        [Header("Shadows")]
        public bool enableShadows = true;
        public LightShadows shadowType =
            LightShadows.Soft;

        [Header("Baked Shadow Radius")]
        [Min(0f)]
        public float tableShadowRadius = 0.2f;

        [Min(0f)]
        public float ceilingShadowRadius = 0.3f;

        [Min(0f)]
        public float pillarShadowRadius = 0.1f;

        [Min(0f)]
        public float wallShadowRadius = 0.2f;

        [Min(0f)]
        public float candleShadowRadius = 0.05f;

        // ==================================================
        // LIGHT RANGE VISUALISATION
        // ==================================================

        [Header("Light Range Visualisation")]
        public bool showLightRanges = true;
        public bool showOnlyEnabledLights = true;

        [Header("Show Light Types")]
        public bool showTableLightRange = true;
        public bool showCeilingLightRanges = true;
        public bool showPillarLightRanges = true;
        public bool showWallLightRanges = true;
        public bool showCandleLightRanges = true;

        [Header("Gizmo Appearance")]
        [Range(0.1f, 1f)]
        public float gizmoOpacity = 0.7f;

        [Range(8, 64)]
        public int gizmoSegments = 24;

        // ==================================================
        // LIGHTMAP UV SETTINGS
        // ==================================================

        [Header("Lightmap UV Generation")]

        [Tooltip(
            "Skip model assets whose detected mesh " +
            "triangle count exceeds this value. " +
            "Set 0 to disable the limit.")]
        [Min(0)]
        public int maxUVTriangleCount = 30000;

        // ==================================================
        // GENERATED LIGHTS
        // ==================================================

        [Header("Generated Lights")]
        [SerializeField]
        private List<Light> generatedLights =
            new List<Light>();

        private const string GeneratedRootName =
            "GeneratedLights";

        private const string CandlePrefix =
            "VFX_Candle_Flame";

        // ==================================================
        // BAKE MODE
        // ==================================================

        [Header("Generated Light Mode")]

        [Tooltip(
            "When enabled, newly generated lights " +
            "are configured as Baked in the Editor.")]
        public bool generateAsBaked = true;

        // ==================================================
        // CANDLE FLICKER
        // ==================================================

        public class CandleFlicker : MonoBehaviour
        {
            public Light targetLight;

            public float baseIntensity;
            public float amount;
            public float speed;
            public float seed;

            [Tooltip(
                "Disable for baked candle lights.")]
            public bool allowFlicker = true;

            private void Update()
            {
                if (!allowFlicker)
                    return;

                if (targetLight == null ||
                    !targetLight.isActiveAndEnabled)
                {
                    return;
                }

                float noise = Mathf.PerlinNoise(
                    Time.time * speed,
                    seed
                );

                targetLight.intensity = Mathf.Max(
                    0f,
                    baseIntensity +
                    (noise * 2f - 1f) * amount
                );
            }
        }

        // ==================================================
        // GET GENERATED ROOT
        // ==================================================

        private Transform GetGeneratedRoot()
        {
            Transform root =
                transform.Find(GeneratedRootName);

            if (root != null)
                return root;

            GameObject obj =
                new GameObject(GeneratedRootName);

            obj.transform.SetParent(transform, false);

            return obj.transform;
        }

        // ==================================================
        // SEARCH ALL PILLARS
        // ==================================================

        public void FindAllPillars()
        {
            foundPillars.Clear();

            if (string.IsNullOrWhiteSpace(
                pillarNameContains))
            {
                Debug.LogWarning(
                    "Pillar search string is empty.",
                    this
                );

                return;
            }

            HashSet<Transform> matches =
                new HashSet<Transform>();

            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene =
                    SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                GameObject[] roots =
                    scene.GetRootGameObjects();

                foreach (GameObject root in roots)
                {
                    Transform[] allObjects =
                        root.GetComponentsInChildren<
                            Transform>(true);

                    foreach (Transform item in allObjects)
                    {
                        if (item == null)
                            continue;

                        if (!includeInactivePillars &&
                            !item.gameObject.activeInHierarchy)
                        {
                            continue;
                        }

                        // Never match the lighting manager
                        // or anything under GeneratedLights.
                        if (item == transform ||
                            item.IsChildOf(transform))
                        {
                            continue;
                        }

                        if (item.name.IndexOf(
                            pillarNameContains,
                            StringComparison.OrdinalIgnoreCase
                            ) < 0)
                        {
                            continue;
                        }

                        matches.Add(item);
                    }
                }
            }

            foreach (Transform item in matches)
            {
                if (ignoreNestedPillarMatches)
                {
                    Transform parent = item.parent;
                    bool parentAlreadyMatches = false;

                    while (parent != null)
                    {
                        if (matches.Contains(parent))
                        {
                            parentAlreadyMatches = true;
                            break;
                        }

                        parent = parent.parent;
                    }

                    if (parentAlreadyMatches)
                        continue;
                }

                foundPillars.Add(item);
            }

            foundPillars.Sort(
                (a, b) => string.Compare(
                    GetHierarchyPath(a),
                    GetHierarchyPath(b),
                    StringComparison.OrdinalIgnoreCase
                )
            );

            Debug.Log(
                "========== PILLAR SEARCH ==========\n" +
                "Search: " + pillarNameContains + "\n" +
                "Pillars Found: " +
                foundPillars.Count + "\n" +
                "===================================",
                this
            );

            foreach (Transform pillar in foundPillars)
            {
                Debug.Log(
                    "Found Pillar: " +
                    GetHierarchyPath(pillar) +
                    " | Position: " +
                    pillar.position,
                    pillar.gameObject
                );
            }
        }

        // ==================================================
        // GET FULL HIERARCHY PATH
        // ==================================================

        private string GetHierarchyPath(
            Transform item)
        {
            if (item == null)
                return "(null)";

            string path = item.name;

            Transform parent = item.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
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
            bool castShadows,
            float bakedShadowRadius)
        {
            GameObject obj =
                new GameObject(lightName);

            obj.transform.SetParent(
                GetGeneratedRoot(),
                false
            );
            obj.AddComponent<ShowObjectGizmos>();
            obj.transform.localPosition =
                localPosition;

            obj.transform.localRotation =
                localRotation;

            Light light =
                obj.AddComponent<Light>();

            light.type = type;
            light.color = color;
            light.intensity = intensity;
            light.range = range;

            light.shadows =
                castShadows && enableShadows
                    ? shadowType
                    : LightShadows.None;

            light.shadowNormalBias = Mathf.Max(0f, bakedShadowRadius);

#if UNITY_EDITOR

            light.lightmapBakeType =
                generateAsBaked
                    ? LightmapBakeType.Baked
                    : LightmapBakeType.Realtime;

#endif

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
            bool shadows,
            float bakedShadowRadius)
        {
            Vector3 direction =
                target - position;

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
                shadows,
                bakedShadowRadius
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
            bool shadows,
            float bakedShadowRadius)
        {
            return CreateLight(
                name,
                LightType.Point,
                position,
                Quaternion.identity,
                color,
                intensity,
                range,
                shadows,
                bakedShadowRadius
            );
        }

        // ==================================================
        // GENERATE PILLAR LIGHTS
        // ==================================================

        private void GeneratePillarLights()
        {
            FindAllPillars();

            for (int i = 0;
                 i < foundPillars.Count;
                 i++)
            {
                Transform pillar =
                    foundPillars[i];

                if (pillar == null)
                    continue;

                Vector3 pillarPosition =
                    pillar.position;

                Vector3 lightWorldPosition =
                    pillarPosition +
                    Vector3.up * pillarLightHeight;

                Vector3 targetWorldPosition =
                    pillarPosition +
                    Vector3.up * pillarLightTargetHeight;

                Vector3 localPosition =
                    transform.InverseTransformPoint(
                        lightWorldPosition
                    );

                Vector3 localTarget =
                    transform.InverseTransformPoint(
                        targetWorldPosition
                    );

                CreateSpot(
                    "Pillar_Red_" + i,
                    localPosition,
                    localTarget,
                    pillarColor,
                    pillarIntensity,
                    pillarRange,
                    pillarSpotAngle,
                    true,
                    pillarShadowRadius
                );
            }
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
                Scene scene =
                    SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root
                         in scene.GetRootGameObjects())
                {
                    Transform[] objects =
                        root.GetComponentsInChildren<
                            Transform>(true);

                    foreach (Transform item in objects)
                    {
                        if (item == null)
                            continue;

                        // Ignore generated lights.
                        if (item == transform ||
                            item.IsChildOf(transform))
                        {
                            continue;
                        }

                        if (!item.name.StartsWith(
                            CandlePrefix,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        Vector3 localPosition =
                            transform.InverseTransformPoint(
                                item.position
                            );

                        Light candle = CreatePoint(
                            "Candle_" + candleIndex,
                            localPosition,
                            candleColor,
                            candleIntensity,
                            candleRange,
                            false,
                            candleShadowRadius
                        );

                        if (flickerCandles)
                        {
                            CandleFlicker flicker =
                                candle.gameObject
                                    .AddComponent<CandleFlicker>();

                            flicker.targetLight = candle;
                            flicker.baseIntensity =
                                candleIntensity;

                            flicker.amount =
                                flickerAmount;

                            flicker.speed =
                                flickerSpeed;

                            flicker.seed =
                                candleIndex * 13.37f;

                            // Flicker only when generated
                            // as realtime lighting.
                            flicker.allowFlicker =
                                !generateAsBaked;
                        }

                        candleIndex++;
                    }
                }
            }

            Debug.Log(
                "Generated " + candleIndex +
                " candle lights.",
                this
            );
        }

        // ==================================================
        // GENERATE ALL LIGHTING
        // ==================================================

        public void GenerateLighting()
        {
            ClearLighting();

            float halfWidth = 4F;

            float halfLength = 4F;

            Vector3 tablePosition =
                table != null
                    ? transform.InverseTransformPoint(
                        table.position)
                    : Vector3.zero;

            // Main table spotlight.
            CreateSpot(
                "Main_Table_Spotlight",
                new Vector3(tablePosition.x, tableLightHeight, tablePosition.z),
                tablePosition,
                tableLightColor,
                tableLightIntensity,
                tableLightRange,
                tableSpotAngle,
                true,
                tableShadowRadius
            );

            // Cold ceiling spotlights.
            for (int i = 0; i < 2; i++)
            {
                float z = i == 0
                    ? -roomLength * 0.25f
                    : roomLength * 0.25f;

                Vector3 position =
                    new Vector3(
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
                    true,
                    ceilingShadowRadius
                );
            }

            // Find and generate pillar lights.
            GeneratePillarLights();

            GenerateWallLights(halfWidth, ceilingHeight);

            // Candle lights.
            GenerateCandleLights();

#if UNITY_EDITOR

            if (!Application.isPlaying &&
                gameObject.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(
                    gameObject.scene
                );
            }

#endif

            Debug.Log(
                "Lighting generated.\n" +
                "Pillars found: " +
                foundPillars.Count + "\n" +
                "Total lights: " +
                generatedLights.Count,
                this
            );
        }

        private void GenerateWallLights(float halfWidth, float ceilingHeight)
        {
            var localCeilingHeight = ceilingHeight /2;

            // Orange wall lights.
            for (int i = 0; i < 3; i++)
            {
                float z = Mathf.Lerp(
                    -halfWidth + 2f,
                    localCeilingHeight,
                    i / 2f
                );

                CreatePoint(
                    "Wall_Left_" + i,
                    new Vector3(
                        -halfWidth,
                        localCeilingHeight,
                        z
                    ),
                    wallColor,
                    wallIntensity,
                    wallRange,
                    false,
                    wallShadowRadius
                );

                CreatePoint(
                    "Wall_Right_" + i,
                    new Vector3(
                        halfWidth,
                        localCeilingHeight,
                        z
                    ),
                    wallColor,
                    wallIntensity,
                    wallRange,
                    false,
                    wallShadowRadius
                );

                CreatePoint(
                    "Wall_Front_" + i,
                    new Vector3(
                        z,
                        localCeilingHeight,
                        halfWidth
                    ),
                    wallColor,
                    wallIntensity,
                    wallRange,
                    false,
                    wallShadowRadius
                );

                CreatePoint(
                    "Wall_Back_" + i,
                    new Vector3(
                        z,
                       localCeilingHeight,
                       -halfWidth
                    ),
                    wallColor,
                    wallIntensity,
                    wallRange,
                    false,
                    wallShadowRadius
                );
            }
        }

        // ==================================================
        // CLEAR LIGHTING
        // ==================================================

        public void ClearLighting()
        {
            Transform root =
                transform.Find(GeneratedRootName);

            if (root != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(root.gameObject);
                }
                else
                {
                    DestroyImmediate(root.gameObject);
                }
            }

            generatedLights.Clear();
        }

        // ==================================================
        // ENABLE / DISABLE LIGHTS
        // ==================================================

        public void SetAllLightsEnabled(
            bool enabled)
        {
            Transform root =
                transform.Find(GeneratedRootName);

            if (root == null)
                return;

            Light[] lights =
                root.GetComponentsInChildren<
                    Light>(true);

            foreach (Light light in lights)
            {
                if (light != null)
                    light.enabled = enabled;
            }
        }

#if UNITY_EDITOR

        // ==================================================
        // AUTOMATIC LIGHT RANGE GIZMOS
        // ==================================================

        private void OnDrawGizmos()
        {
            if (!showLightRanges)
                return;

            Transform root =
                transform.Find(GeneratedRootName);

            if (root == null)
                return;

            Light[] lights =
                root.GetComponentsInChildren<
                    Light>(true);

            foreach (Light light in lights)
            {
                if (light == null)
                    continue;

                if (showOnlyEnabledLights &&
                    !light.isActiveAndEnabled)
                {
                    continue;
                }

                if (!ShouldDrawLightRange(light))
                    continue;

                Color color = light.color;
                color.a = gizmoOpacity;

                Gizmos.color = color;

                if (light.type == LightType.Point)
                {
                    Gizmos.DrawWireSphere(
                        light.transform.position,
                        light.range
                    );
                }
                else if (light.type == LightType.Spot)
                {
                    DrawSpotLightRange(light);
                }
            }
        }

        // ==================================================
        // FILTER GIZMOS BY LIGHT TYPE
        // ==================================================

        private bool ShouldDrawLightRange(
            Light light)
        {
            string name =
                light.gameObject.name;

            if (name.StartsWith("Main_Table_"))
                return showTableLightRange;

            if (name.StartsWith("Ceiling_"))
                return showCeilingLightRanges;

            if (name.StartsWith("Pillar_"))
                return showPillarLightRanges;

            if (name.StartsWith("Wall_"))
                return showWallLightRanges;

            if (name.StartsWith("Candle_"))
                return showCandleLightRanges;

            return true;
        }

        // ==================================================
        // DRAW SPOTLIGHT CONE
        // ==================================================

        private void DrawSpotLightRange(
            Light light)
        {
            Transform t = light.transform;

            Vector3 position = t.position;
            Vector3 forward = t.forward;
            Vector3 right = t.right;
            Vector3 up = t.up;

            float range =
                Mathf.Max(0f, light.range);

            float radius =
                Mathf.Tan(
                    light.spotAngle *
                    0.5f *
                    Mathf.Deg2Rad
                ) * range;

            Vector3 endCentre =
                position + forward * range;

            Gizmos.DrawLine(
                position,
                endCentre
            );

            int segments =
                Mathf.Max(8, gizmoSegments);

            Vector3 previous =
                endCentre + right * radius;

            for (int i = 1;
                 i <= segments;
                 i++)
            {
                float angle =
                    i * Mathf.PI * 2f /
                    segments;

                Vector3 next =
                    endCentre +
                    right * Mathf.Cos(angle) * radius +
                    up * Mathf.Sin(angle) * radius;

                Gizmos.DrawLine(
                    previous,
                    next
                );

                Gizmos.DrawLine(
                    position,
                    next
                );

                previous = next;
            }

            Gizmos.DrawWireSphere(
                position,
                0.1f
            );
        }

        // ==================================================
        // GET GENERATED LIGHTS
        // ==================================================

        private Light[] GetGeneratedLights()
        {
            Transform root =
                transform.Find(GeneratedRootName);

            if (root == null)
                return new Light[0];

            return root.GetComponentsInChildren<
                Light>(true);
        }

        // ==================================================
        // SET ALL GENERATED LIGHT MODES
        // ==================================================

        public void SetAllLightsBakeType(
            LightmapBakeType bakeType)
        {
            Light[] lights =
                GetGeneratedLights();

            generateAsBaked =
                bakeType == LightmapBakeType.Baked;

            foreach (Light light in lights)
            {
                if (light == null)
                    continue;

                Undo.RecordObject(
                    light,
                    "Change Light Bake Type"
                );

                light.lightmapBakeType =
                    bakeType;

                CandleFlicker flicker =
                    light.GetComponent<CandleFlicker>();

                if (flicker != null)
                {
                    Undo.RecordObject(
                        flicker,
                        "Change Candle Flicker Mode"
                    );

                    flicker.allowFlicker =
                        bakeType != LightmapBakeType.Baked;

                    EditorUtility.SetDirty(flicker);
                }

                EditorUtility.SetDirty(light);
            }

            EditorUtility.SetDirty(this);

            if (gameObject.scene.IsValid())
            {
                EditorSceneManager.MarkSceneDirty(
                    gameObject.scene
                );
            }

            Debug.Log(
                "Set " + lights.Length +
                " generated lights to " +
                bakeType,
                this
            );
        }

        // ==================================================
        // SET ALL SCENE LIGHTS SOFT SHADOWS
        // ==================================================

        public void SetAllLightsSoftShadows()
        {
            int count = 0;

            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene =
                    SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root
                         in scene.GetRootGameObjects())
                {
                    Light[] lights =
                        root.GetComponentsInChildren<
                            Light>(true);

                    foreach (Light light in lights)
                    {
                        Undo.RecordObject(
                            light,
                            "Set Soft Shadows"
                        );

                        light.shadows =
                            LightShadows.Soft;

                        EditorUtility.SetDirty(light);
                        count++;
                    }
                }

                EditorSceneManager.MarkSceneDirty(
                    scene
                );
            }

            Debug.Log(
                "Updated " + count +
                " lights to Soft Shadows.",
                this
            );
        }

        // ==================================================
        // GENERATE LIGHTMAP UVs
        // ==================================================

        public void GenerateLightmapUVsAll()
        {
            Dictionary<string, int> models =
                new Dictionary<string, int>();

            int updated = 0;
            int alreadyEnabled = 0;
            int skipped = 0;
            int failed = 0;
            bool cancelled = false;

            // Collect mesh assets before reimporting.
            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene =
                    SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root
                         in scene.GetRootGameObjects())
                {
                    MeshFilter[] filters =
                        root.GetComponentsInChildren<
                            MeshFilter>(true);

                    foreach (MeshFilter filter in filters)
                    {
                        AddModelMesh(
                            filter.sharedMesh,
                            models
                        );
                    }

                    SkinnedMeshRenderer[] skinned =
                        root.GetComponentsInChildren<
                            SkinnedMeshRenderer>(true);

                    foreach (SkinnedMeshRenderer renderer
                             in skinned)
                    {
                        AddModelMesh(
                            renderer.sharedMesh,
                            models
                        );
                    }
                }
            }

            List<string> paths =
                new List<string>(models.Keys);

            try
            {
                for (int i = 0;
                     i < paths.Count;
                     i++)
                {
                    string path = paths[i];

                    if (EditorUtility
                        .DisplayCancelableProgressBar(
                            "Generating Lightmap UVs",
                            (i + 1) + "/" +
                            paths.Count + "\n" +
                            path,
                            (float)i /
                            Mathf.Max(1, paths.Count)))
                    {
                        cancelled = true;
                        break;
                    }

                    int triangles =
                        models[path];

                    if (maxUVTriangleCount > 0 &&
                        triangles > maxUVTriangleCount)
                    {
                        skipped++;

                        Debug.LogWarning(
                            "Skipped high-poly model (" +
                            triangles +
                            " triangles): " +
                            path,
                            this
                        );

                        continue;
                    }

                    ModelImporter importer =
                        AssetImporter.GetAtPath(path)
                            as ModelImporter;

                    if (importer == null)
                    {
                        skipped++;
                        continue;
                    }

                    if (importer.generateSecondaryUV)
                    {
                        alreadyEnabled++;
                        continue;
                    }

                    try
                    {
                        importer.generateSecondaryUV =
                            true;

                        importer.SaveAndReimport();

                        updated++;
                    }
                    catch (Exception exception)
                    {
                        failed++;

                        Debug.LogError(
                            "UV generation failed: " +
                            path + "\n" +
                            exception,
                            this
                        );
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            EditorUtility.DisplayDialog(
                "Lightmap UV Generation Finished",
                "Updated: " + updated + "\n" +
                "Already Enabled: " +
                alreadyEnabled + "\n" +
                "Skipped: " + skipped + "\n" +
                "Failed: " + failed + "\n" +
                "Cancelled: " + cancelled,
                "OK"
            );
        }

        // ==================================================
        // COLLECT MODEL TRIANGLE COUNTS
        // ==================================================

        private void AddModelMesh(
            Mesh mesh,
            Dictionary<string, int> models)
        {
            if (mesh == null)
                return;

            string path =
                AssetDatabase.GetAssetPath(mesh);

            if (string.IsNullOrEmpty(path))
                return;

            int triangles = 0;

            for (int i = 0;
                 i < mesh.subMeshCount;
                 i++)
            {
                if (mesh.GetTopology(i) !=
                    MeshTopology.Triangles)
                {
                    continue;
                }

                triangles +=
                    (int)(mesh.GetIndexCount(i) / 3);
            }

            if (models.ContainsKey(path))
            {
                models[path] =
                    Mathf.Max(
                        models[path],
                        triangles
                    );
            }
            else
            {
                models.Add(path, triangles);
            }
        }

        // ==================================================
        // CONTRIBUTE GI CONTROLS
        // ==================================================

        public void MarkAllStatic()
        {
            SetAllObjectsContributeGI(true);
        }

        public void MarkAllNonStatic()
        {
            SetAllObjectsContributeGI(false);
        }

        private void SetAllObjectsContributeGI(
            bool contributeGI)
        {
            int count = 0;

            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene =
                    SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                foreach (GameObject root
                         in scene.GetRootGameObjects())
                {
                    MeshRenderer[] renderers =
                        root.GetComponentsInChildren<
                            MeshRenderer>(true);

                    foreach (MeshRenderer renderer
                             in renderers)
                    {
                        GameObject obj =
                            renderer.gameObject;

                        Undo.RecordObject(
                            obj,
                            "Change Contribute GI"
                        );

                        StaticEditorFlags flags =
                            GameObjectUtility
                                .GetStaticEditorFlags(obj);

                        if (contributeGI)
                        {
                            flags |=
                                StaticEditorFlags.ContributeGI;
                        }
                        else
                        {
                            flags &=
                                ~StaticEditorFlags.ContributeGI;
                        }

                        GameObjectUtility
                            .SetStaticEditorFlags(
                                obj,
                                flags
                            );

                        EditorUtility.SetDirty(obj);
                        count++;
                    }
                }

                EditorSceneManager.MarkSceneDirty(
                    scene
                );
            }

            Debug.Log(
                "Updated Contribute GI on " +
                count + " MeshRenderers.",
                this
            );
        }

        // ==================================================
        // BAKE QUALITY PRESETS
        // ==================================================

        public enum BakeQuality
        {
            UltraFast,
            Fast,
            Good
        }

        public void SetBakeQuality(
            BakeQuality quality)
        {
            if (Application.isPlaying ||
                Lightmapping.isRunning)
            {
                return;
            }

            LightingSettings settings =
                Lightmapping.lightingSettings;

            if (settings == null)
            {
                EditorUtility.DisplayDialog(
                    "Lighting Settings Missing",
                    "Assign a Lighting Settings Asset " +
                    "in Window > Rendering > Lighting.",
                    "OK"
                );

                return;
            }

            Undo.RecordObject(
                settings,
                "Change Bake Quality"
            );

            settings.bakedGI = true;

            settings.lightmapper =
                LightingSettings.Lightmapper.ProgressiveGPU;

            switch (quality)
            {
                case BakeQuality.UltraFast:

                    settings.lightmapResolution = 2f;
                    settings.lightmapMaxSize = 512;
                    settings.lightmapPadding = 2;

                    settings.directSampleCount = 8;
                    settings.indirectSampleCount = 16;
                    settings.environmentSampleCount = 16;

                    settings.maxBounces = 1;
                    settings.ao = false;

                    break;

                case BakeQuality.Fast:

                    settings.lightmapResolution = 10f;
                    settings.lightmapMaxSize = 1024;
                    settings.lightmapPadding = 2;

                    settings.directSampleCount = 32;
                    settings.indirectSampleCount = 64;
                    settings.environmentSampleCount = 64;

                    settings.maxBounces = 2;
                    settings.ao = false;

                    break;

                case BakeQuality.Good:

                    settings.lightmapResolution = 30f;
                    settings.lightmapMaxSize = 2048;
                    settings.lightmapPadding = 3;

                    settings.directSampleCount = 128;
                    settings.indirectSampleCount = 256;
                    settings.environmentSampleCount = 256;

                    settings.maxBounces = 3;
                    settings.ao = true;

                    break;
            }

            // This property belongs to LightmapSettings,
            // not LightingSettings.
            LightmapSettings.lightmapsMode =
                LightmapsMode.NonDirectional;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();

            Debug.Log(
                "Applied bake quality: " +
                quality,
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
                "Resolution: " +
                settings.lightmapResolution + "\n" +
                "Direct Samples: " +
                settings.directSampleCount + "\n" +
                "Indirect Samples: " +
                settings.indirectSampleCount + "\n" +
                "Environment Samples: " +
                settings.environmentSampleCount + "\n" +
                "Bake Running: " +
                Lightmapping.isRunning + "\n" +
                "Lightmaps: " +
                LightmapSettings.lightmaps.Length + "\n" +
                "======================================",
                this
            );

            Selection.activeObject = settings;
            EditorGUIUtility.PingObject(settings);
        }


#if UNITY_EDITOR
        // ==================================================
        // Save Scene
        // ==================================================
        public void SaveScene()
        {
            if (Application.isPlaying)
                return;

            UnityEngine.SceneManagement.Scene scene =
                gameObject.scene;

            if (!scene.IsValid() || !scene.isLoaded)
                return;

            if (string.IsNullOrEmpty(scene.path))
            {
                EditorSceneManager.SaveScene(
                    scene,
                    "Assets/" + scene.name + ".unity"
                );
            }
            else
            {
                EditorSceneManager.SaveScene(scene);
            }

            Debug.Log("Scene saved: " + scene.path);
        }

#endif

        // ==================================================
        // SORT ONLY THIS OBJECT'S CHILDREN A-Z
        // ==================================================

        [Header("Hierarchy Sorting")]
        public bool sortNestedChildren = true;

        public void SortAllScenes()
        {
            // Retain the existing method name for compatibility,
            // but only sort descendants of this lighting manager.
            if (Application.isPlaying)
                return;

            Undo.SetCurrentGroupName("Sort Lighting Children A-Z");
            int undoGroup = Undo.GetCurrentGroup();

            int count = SortChildrenAlphabetically(transform);

            Undo.CollapseUndoOperations(undoGroup);

            if (gameObject.scene.IsValid() && gameObject.scene.isLoaded)
            {
                EditorSceneManager.MarkSceneDirty(gameObject.scene);

                if (!string.IsNullOrEmpty(gameObject.scene.path))
                    EditorSceneManager.SaveScene(gameObject.scene);
                else
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            Debug.Log("Sorted " + count + " child objects A-Z under " + gameObject.name, this);
        }

        private int SortChildrenAlphabetically(Transform parent)
        {
            List<Transform> children = new List<Transform>();

            foreach (Transform child in parent)
                children.Add(child);

            // Stable alphabetical ordering for identical names.
            children.Sort((a, b) =>
            {
                int result = StringComparer.OrdinalIgnoreCase.Compare(a.name, b.name);
                return result != 0 ? result : a.GetSiblingIndex().CompareTo(b.GetSiblingIndex());
            });

            if (children.Count > 1)
            {
                // Record the hierarchy change for Undo.
                Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Sort Children A-Z");

                for (int i = 0; i < children.Count; i++)
                    children[i].SetSiblingIndex(i);
            }

            int count = children.Count;

            if (sortNestedChildren)
            {
                foreach (Transform child in children)
                    count += SortChildrenAlphabetically(child);
            }

            return count;
        }

        // ==================================================
        // BAKE LIGHTING
        // ==================================================

        public void BakeLighting()
        {
            if (Application.isPlaying ||
                Lightmapping.isRunning)
            {
                return;
            }

            if (Lightmapping.lightingSettings == null)
            {
                EditorUtility.DisplayDialog(
                    "Lighting Settings Missing",
                    "Assign a Lighting Settings Asset " +
                    "before baking.",
                    "OK"
                );

                return;
            }

            if (!EditorSceneManager
                .SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            Lightmapping.bakeCompleted -=
                OnBakeCompleted;

            Lightmapping.bakeCompleted +=
                OnBakeCompleted;

            bool started =
                Lightmapping.BakeAsync();

            if (!started)
            {
                Lightmapping.bakeCompleted -=
                    OnBakeCompleted;

                Debug.LogWarning(
                    "Lighting bake could not start.",
                    this
                );

                return;
            }

            Debug.Log(
                "Lighting bake started.",
                this
            );
        }

        // ==================================================
        // BAKE COMPLETION POPUP
        // ==================================================

        private void OnBakeCompleted()
        {
            Lightmapping.bakeCompleted -=
                OnBakeCompleted;

            Debug.Log(
                "Imperial Strategium bake completed.",
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

            EditorGUILayout.LabelField("Hierarchy Sorting", EditorStyles.boldLabel);
            if (GUILayout.Button("SORT CHILDREN A-Z", GUILayout.Height(40)))
            {
                lighting.SortAllScenes();
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // PILLAR SEARCH
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Automatic Pillar Detection",
                EditorStyles.boldLabel
            );

            if (GUILayout.Button(
                "FIND ALL PILLARS",
                GUILayout.Height(40)))
            {
                lighting.FindAllPillars();

                EditorUtility.SetDirty(lighting);
                SceneView.RepaintAll();
            }

            EditorGUILayout.HelpBox(
                "Searches every GameObject in all loaded " +
                "scenes. Matches the search text anywhere " +
                "in the name, ignoring case. " +
                "Generated lighting objects are excluded.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // LIGHT GENERATION
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Lighting Generation",
                EditorStyles.boldLabel
            );

            if (GUILayout.Button(
                "Generate / Rebuild Lighting",
                GUILayout.Height(40)))
            {
                lighting.GenerateLighting();

                EditorUtility.SetDirty(lighting);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button(
                "Clear All Lighting",
                GUILayout.Height(30)))
            {
                lighting.ClearLighting();

                EditorUtility.SetDirty(lighting);
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // LIGHT MODES
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Generated Light Modes",
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

            EditorGUI.BeginDisabledGroup(
                Application.isPlaying
            );

            if (GUILayout.Button(
                "Generate Lightmap UVs ALL",
                GUILayout.Height(35)))
            {
                lighting.GenerateLightmapUVsAll();
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.HelpBox(
                "High-poly models above the triangle " +
                "limit are skipped. Reimporting a model " +
                "can still take time and cannot be " +
                "interrupted once Unity starts its import.",
                MessageType.Warning
            );

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // CONTRIBUTE GI
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Contribute GI Controls",
                EditorStyles.boldLabel
            );

            EditorGUI.BeginDisabledGroup(
                Application.isPlaying
            );

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
                "These buttons change Contribute GI on " +
                "every MeshRenderer in loaded scenes. " +
                "Do not mark moving miniatures, VR hands " +
                "or other dynamic objects for baked GI.",
                MessageType.Warning
            );

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // BAKE PRESETS
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Lighting Quality Presets",
                EditorStyles.boldLabel
            );

            EditorGUI.BeginDisabledGroup(
                Application.isPlaying ||
                Lightmapping.isRunning
            );

            if (GUILayout.Button(
                "ULTRA FAST BAKE",
                GUILayout.Height(40)))
            {
                lighting.SetBakeQuality(
                    ImperialStrategiumLighting
                        .BakeQuality.UltraFast
                );
            }

            if (GUILayout.Button(
                "FAST BAKE",
                GUILayout.Height(35)))
            {
                lighting.SetBakeQuality(
                    ImperialStrategiumLighting
                        .BakeQuality.Fast
                );
            }

            if (GUILayout.Button(
                "GOOD QUALITY BAKE",
                GUILayout.Height(35)))
            {
                lighting.SetBakeQuality(
                    ImperialStrategiumLighting
                        .BakeQuality.Good
                );
            }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.HelpBox(
                "ULTRA FAST: 2 texels/unit\n" +
                "FAST: 10 texels/unit\n" +
                "GOOD: 30 texels/unit\n\n" +
                "Choose a preset, then bake.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // BAKE LIGHTING
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Lightmap Baking",
                EditorStyles.boldLabel
            );

            EditorGUI.BeginDisabledGroup(
                Application.isPlaying ||
                Lightmapping.isRunning
            );

            if (GUILayout.Button(
                "BAKE LIGHTING NOW",
                GUILayout.Height(45)))
            {
                lighting.BakeLighting();
            }

            EditorGUI.EndDisabledGroup();

            if (Lightmapping.isRunning)
            {
                EditorGUILayout.HelpBox(
                    "Lighting bake is running.",
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

            if (GUILayout.Button(
                "Enable All Lights"))
            {
                lighting.SetAllLightsEnabled(true);
            }

            if (GUILayout.Button(
                "Disable All Lights"))
            {
                lighting.SetAllLightsEnabled(false);
            }

            EditorGUILayout.Space(10);

            // ----------------------------------------------
            // LIGHT RANGE VISUALISATION
            // ----------------------------------------------

            EditorGUILayout.LabelField(
                "Light Range Visualisation",
                EditorStyles.boldLabel
            );

            if (GUILayout.Button(
                "Show All Light Ranges",
                GUILayout.Height(30)))
            {
                Undo.RecordObject(
                    lighting,
                    "Show Light Ranges"
                );

                lighting.showLightRanges = true;
                lighting.showTableLightRange = true;
                lighting.showCeilingLightRanges = true;
                lighting.showPillarLightRanges = true;
                lighting.showWallLightRanges = true;
                lighting.showCandleLightRanges = true;

                EditorUtility.SetDirty(lighting);
                SceneView.RepaintAll();
            }

            if (GUILayout.Button(
                "Hide All Light Ranges",
                GUILayout.Height(30)))
            {
                Undo.RecordObject(
                    lighting,
                    "Hide Light Ranges"
                );

                lighting.showLightRanges = false;

                EditorUtility.SetDirty(lighting);
                SceneView.RepaintAll();
            }
        }
    }

#endif
}
