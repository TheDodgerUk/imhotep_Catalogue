
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

        [Header("Performance")]
        public bool enableShadows = true;
        public LightShadows shadowType = LightShadows.Soft;

        [Header("Debug")]
        [SerializeField]
        private List<Light> generatedLights = new List<Light>();

        private const string GeneratedRootName = "GeneratedLights";
        private const string CandlePrefix = "VFX_Candle_Flame";

        // --------------------------------------------------
        // CANDLE FLICKER
        // --------------------------------------------------

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

                // Baked lights cannot animate their illumination.
                if (targetLight.lightmapBakeType == LightmapBakeType.Baked)
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

        // --------------------------------------------------
        // GENERATED LIGHT ROOT
        // --------------------------------------------------

        private Transform GetGeneratedRoot()
        {
            Transform root = transform.Find(GeneratedRootName);

            if (root == null)
            {
                GameObject obj = new GameObject(GeneratedRootName);

                root = obj.transform;
                root.SetParent(transform, false);
            }

            return root;
        }

        // --------------------------------------------------
        // CREATE LIGHT
        // --------------------------------------------------

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

        // --------------------------------------------------
        // CREATE SPOTLIGHT
        // --------------------------------------------------

        private void CreateSpot(
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

            Quaternion rotation = direction.sqrMagnitude > 0.0001f
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
        }

        // --------------------------------------------------
        // CREATE POINT LIGHT
        // --------------------------------------------------

        private void CreatePoint(
            string name,
            Vector3 position,
            Color color,
            float intensity,
            float range,
            bool shadows)
        {
            CreateLight(
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

        // --------------------------------------------------
        // FIND CANDLE FLAMES IN ALL LOADED SCENES
        // --------------------------------------------------

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

                GameObject[] rootObjects = scene.GetRootGameObjects();

                foreach (GameObject root in rootObjects)
                {
                    Transform[] allTransforms =
                        root.GetComponentsInChildren<Transform>(true);

                    foreach (Transform candleTransform in allTransforms)
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

                        Light candle = CreateLight(
                            "Candle_" + candleIndex,
                            LightType.Point,
                            localPosition,
                            Quaternion.identity,
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

        // --------------------------------------------------
        // GENERATE ALL LIGHTING
        // --------------------------------------------------

        public void GenerateLighting()
        {
            ClearLighting();

            float halfWidth = roomWidth * 0.5f;
            float halfLength = roomLength * 0.5f;

            Vector3 tablePosition = table != null
                ? transform.InverseTransformPoint(table.position)
                : Vector3.zero;

            // Main table spotlight.
            Vector3 mainPosition =
                tablePosition + Vector3.up * tableLightHeight;

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

            // Red pillar accent lights.
            Vector3[] pillarPositions =
            {
                new Vector3(-halfWidth + 1f, 0.5f, -halfLength + 2f),
                new Vector3( halfWidth - 1f, 0.5f, -halfLength + 2f),
                new Vector3(-halfWidth + 1f, 0.5f,  halfLength - 2f),
                new Vector3( halfWidth - 1f, 0.5f,  halfLength - 2f)
            };

            for (int i = 0; i < pillarPositions.Length; i++)
            {
                Vector3 position = pillarPositions[i];

                CreateSpot(
                    "Pillar_Red_" + i,
                    position,
                    position + Vector3.up * 4f,
                    pillarColor,
                    pillarIntensity,
                    pillarRange,
                    55f,
                    false
                );
            }

            // Orange wall lights.
            for (int i = 0; i < 3; i++)
            {
                float z = Mathf.Lerp(
                    -halfLength + 2f,
                    halfLength - 2f,
                    i / 2f
                );

                CreatePoint(
                    "Wall_Left_" + i,
                    new Vector3(-halfWidth + 0.5f, 3f, z),
                    wallColor,
                    wallIntensity,
                    wallRange,
                    false
                );

                CreatePoint(
                    "Wall_Right_" + i,
                    new Vector3(halfWidth - 0.5f, 3f, z),
                    wallColor,
                    wallIntensity,
                    wallRange,
                    false
                );
            }

            // Find VFX_Candle_Flame objects and place lights.
            GenerateCandleLights();

            Debug.Log(
                "Imperial Strategium Lighting generated: " +
                generatedLights.Count + " lights.",
                this
            );
        }

        // --------------------------------------------------
        // CLEAR GENERATED LIGHTING
        // --------------------------------------------------

        public void ClearLighting()
        {
            Transform root = transform.Find(GeneratedRootName);

            if (root != null)
            {
                // Detach before destroying.
                root.SetParent(null);

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

        // --------------------------------------------------
        // ENABLE / DISABLE GENERATED LIGHTS
        // --------------------------------------------------

        public void SetAllLightsEnabled(bool enabled)
        {
            foreach (Light light in generatedLights)
            {
                if (light != null)
                    light.enabled = enabled;
            }
        }

        // --------------------------------------------------
        // EDITOR-ONLY LIGHTING AND STATIC CONTROLS
        // --------------------------------------------------

#if UNITY_EDITOR

        // Find generated lights, even after an Editor reload.
        private Light[] GetGeneratedLights()
        {
            Transform root = transform.Find(GeneratedRootName);

            if (root == null)
                return new Light[0];

            return root.GetComponentsInChildren<Light>(true);
        }

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

            EditorSceneManager.MarkSceneDirty(gameObject.scene);

            Debug.Log(
                "Set " + lights.Length +
                " generated lights to " + bakeType,
                this
            );
        }

        // --------------------------------------------------
        // BAKE LIGHTING
        // --------------------------------------------------

        public void BakeLighting()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning(
                    "Cannot bake lighting while in Play Mode.",
                    this
                );

                return;
            }

            if (Lightmapping.isRunning)
            {
                Debug.LogWarning(
                    "Lighting bake is already running.",
                    this
                );

                return;
            }

            // Save current scenes without opening or closing scenes.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            bool started = Lightmapping.BakeAsync();

            if (started)
            {
                Debug.Log("Lighting Bake Started!", this);
            }
            else
            {
                Debug.LogWarning(
                    "Lighting bake could not be started.",
                    this
                );
            }
        }

        // --------------------------------------------------
        // MARK ALL OBJECTS STATIC / NON STATIC
        // --------------------------------------------------

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
            {
                Debug.LogWarning(
                    "Static flags cannot be edited in Play Mode.",
                    this
                );

                return;
            }

            int count = 0;

            // One Undo operation for all objects.
            Undo.IncrementCurrentGroup();

            int undoGroup = Undo.GetCurrentGroup();

            Undo.SetCurrentGroupName(
                makeStatic ? "Mark All Static" : "Mark All Non Static"
            );

            for (int sceneIndex = 0;
                 sceneIndex < SceneManager.sceneCount;
                 sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                    continue;

                GameObject[] rootObjects = scene.GetRootGameObjects();

                foreach (GameObject root in rootObjects)
                {
                    // Includes inactive children.
                    Transform[] allTransforms =
                        root.GetComponentsInChildren<Transform>(true);

                    foreach (Transform child in allTransforms)
                    {
                        GameObject obj = child.gameObject;

                        // Ignore objects that cannot be edited.
                        if ((obj.hideFlags & HideFlags.NotEditable) != 0)
                            continue;

                        Undo.RecordObject(
                            obj,
                            makeStatic
                                ? "Mark All Static"
                                : "Mark All Non Static"
                        );

                        // StaticEditorFlags.Everything enables all
                        // available static flags for this Unity version.
                        StaticEditorFlags flags = makeStatic
                            ? StaticEditorFlags.ContributeGI |
                              StaticEditorFlags.OccluderStatic |
                              StaticEditorFlags.OccludeeStatic |
                              StaticEditorFlags.BatchingStatic |
                              StaticEditorFlags.NavigationStatic |
                              StaticEditorFlags.OffMeshLinkGeneration
                            : (StaticEditorFlags)0;

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

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log(
                (makeStatic
                    ? "Marked Static: "
                    : "Marked Non Static: ") +
                count + " GameObjects.",
                this
            );
        }

#endif
    }

    // ======================================================
    // CUSTOM INSPECTOR - SAME FILE
    // ======================================================

#if UNITY_EDITOR

    [CustomEditor(typeof(ImperialStrategiumLighting))]
    public class ImperialStrategiumLightingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ImperialStrategiumLighting lighting =
                (ImperialStrategiumLighting)target;

            EditorGUILayout.Space(12);

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

            EditorGUILayout.Space(10);

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

            EditorGUILayout.Space(10);

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

            EditorGUILayout.Space(12);

            EditorGUILayout.LabelField(
                "Static Object Controls",
                EditorStyles.boldLabel
            );

            if (GUILayout.Button(
                "Mark All Static",
                GUILayout.Height(35)))
            {
                lighting.MarkAllStatic();
            }

            if (GUILayout.Button(
                "Mark All Non Static",
                GUILayout.Height(35)))
            {
                lighting.MarkAllNonStatic();
            }

            EditorGUILayout.HelpBox(
                "Static buttons affect ALL GameObjects in ALL " +
                "loaded scenes, including inactive objects. " +
                "Movable VR objects should not remain static.",
                MessageType.Warning
            );
        }
    }

#endif
}
