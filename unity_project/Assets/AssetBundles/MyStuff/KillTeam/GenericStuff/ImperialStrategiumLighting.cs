
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
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

        public void GenerateLighting()
        {
            ClearLighting();

            float halfWidth = roomWidth * 0.5f;
            float halfLength = roomLength * 0.5f;

            // Table centre relative to this GameObject.
            Vector3 tablePosition = table != null
                ? transform.InverseTransformPoint(table.position)
                : Vector3.zero;

            // Main table spotlight.
            Vector3 mainPosition = tablePosition +
                Vector3.up * tableLightHeight;

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

            // Candle lights near the table.
            if (generateCandles)
            {
                Vector3[] offsets =
                {
                    new Vector3(-1.5f, 0.8f, -1f),
                    new Vector3( 1.5f, 0.8f, -1f),
                    new Vector3(-1.5f, 0.8f,  1f),
                    new Vector3( 1.5f, 0.8f,  1f)
                };

                for (int i = 0; i < offsets.Length; i++)
                {
                    Light candle = CreateLight(
                        "Candle_" + i,
                        LightType.Point,
                        tablePosition + offsets[i],
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
                        flicker.seed = i * 13.37f;
                    }
                }
            }

            Debug.Log(
                "Imperial Strategium Lighting generated: " +
                generatedLights.Count + " lights.",
                this
            );
        }

        public void ClearLighting()
        {
            Transform root = transform.Find(GeneratedRootName);

            if (root != null)
            {
                // Detach immediately so repeated generation
                // cannot find objects awaiting destruction.
                root.SetParent(null);

                if (Application.isPlaying)
                    Destroy(root.gameObject);
                else
                    DestroyImmediate(root.gameObject);
            }

            generatedLights.Clear();
        }

        public void SetAllLightsEnabled(bool enabled)
        {
            foreach (Light light in generatedLights)
            {
                if (light != null)
                    light.enabled = enabled;
            }
        }
    }

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

            if (GUILayout.Button("Generate / Rebuild Lighting",
                GUILayout.Height(35)))
            {
                lighting.GenerateLighting();
                EditorUtility.SetDirty(lighting);
            }

            if (GUILayout.Button("Clear All Lighting"))
            {
                lighting.ClearLighting();
                EditorUtility.SetDirty(lighting);
            }

            EditorGUILayout.Space(8);

            if (GUILayout.Button("Enable All Lights"))
                lighting.SetAllLightsEnabled(true);

            if (GUILayout.Button("Disable All Lights"))
                lighting.SetAllLightsEnabled(false);
        }
    }
#endif
}
