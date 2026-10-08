using System;
using System.Text;
using UnityEngine;

namespace Utils
{
    public static class Unity
    {
        public static T LoadResource<T>(params string[] fileparts) where T : UnityEngine.Object
        {
            return Resources.Load<T>(string.Concat(fileparts));
        }

        public static T GetPlayerPrefEnum<T>(string key, T defaultValue)
        {
            return (T)(object)PlayerPrefs.GetInt(key, (int)(object)defaultValue);
        }

        public static float GetRandom(float min, float max) => UnityEngine.Random.Range(min, max);
        public static int GetRandom(int min, int max) => UnityEngine.Random.Range(min, max);

        public static Color GetRandomColour()
        {
            return new Color(GetRandom(0f, 1f), GetRandom(0f, 1f), GetRandom(0f, 1f));
        }

        public static LineRenderer CreateLine(Transform parent, float width, int endCapQuality, Material material = null)
        {
            if (material == null)
            {
                material = new Material(Shader.Find("Particles/Additive"));
            }

            var line = parent.CreateChild<LineRenderer>();

            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.material = material;

            line.numCapVertices = endCapQuality;
            line.widthMultiplier = width;
            return line;
        }

        public static void CombineMesh(GameObject item)
        {
            var meshFilters = item.GetComponentsInChildren<MeshFilter>();
            var instances = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; ++i)
            {
                instances[i].mesh = meshFilters[i].sharedMesh;
                instances[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
            }

            var combined = new Mesh();
            combined.CombineMeshes(instances);

            item.GetComponent<MeshFilter>().mesh = combined;
        }

        public static void LoadTextureFromDisk(MonoBehaviour host, string url, Action<Texture> callback)
        {
            host.WaitForWeb(new WWW("file:///" + url), (www) =>
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    callback?.Invoke(www.textureNonReadable);
                }
                else
                {
                    Debug.LogError("Failed to load texture: " + www.error);
                    callback?.Invoke(null);
                }
            });
        }

        public static Texture2D Blur(Texture2D image, int blurSize)
        {
            var blurred = new Texture2D(image.width, image.height);

            for (int xx = 0; xx < image.width; xx++)
            {
                for (int yy = 0; yy < image.height; yy++)
                {
                    float avgR = 0f;
                    float avgG = 0f;
                    float avgB = 0f;
                    float avgA = 0f;
                    int blurPixelCount = 0;

                    //Get average pixel colors
                    for (int x = xx; (x < xx + blurSize && x < image.width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.height); y++)
                        {
                            Color pixel = image.GetPixel(x, y);
                            avgR += pixel.r;
                            avgG += pixel.g;
                            avgB += pixel.b;
                            avgA += pixel.a;

                            blurPixelCount++;
                        }
                    }

                    //Get average
                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;
                    avgA = avgA / blurPixelCount;

                    for (int x = xx; (x < xx + blurSize && x < image.width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.height); y++)
                        {
                            blurred.SetPixel(x, y, new Color(avgR, avgG, avgB, avgA));
                        }
                    }
                }
            }

            blurred.Apply();
            return blurred;
        }

        const float MaxBits = 2048f;
        const float EncodeBit = 1f / MaxBits;
        static readonly Vector2 EncodeMul = new Vector2(1f, MaxBits);
        static readonly Vector2 DecodeDot = new Vector2(1f, EncodeBit);

        // Encoding/decoding [0..1] floats into 8 bit/channel RG. Note that 1.0 will not be encoded properly.
        public static Vector2 EncodeFloatRG(float f)
        {
            var enc = EncodeMul * f;
            enc = enc.Frac();
            enc.x -= enc.y * EncodeBit;
            return enc;
        }
        public static float DecodeFloatRG(Vector2 enc)
        {
            return Vector2.Dot(enc, DecodeDot);
        }
        public static float DecodeFloatRG(float x, float y)
        {
            return DecodeFloatRG(new Vector2(x, y));
        }

        public static float GetMax(float[][] array, bool pad = false)
        {
            float max = float.MinValue;
            for (int i = 0; i < array.Length; ++i)
            {
                float m = GetMax(array[i], false);
                max = max.Max(m);
            }
            return PadMax(max, pad);
        }

        public static float GetMax(float[] array, bool pad = false)
        {
            float max = float.MinValue;
            for (int i = 0; i < array.Length; ++i)
            {
                max = max.Max(array[i]);
            }
            return PadMax(max, pad);
        }

        private static float PadMax(float max, bool pad)
        {
            const float Epsilon = (float)0.001;
            return pad ? (max + Epsilon) : max;
        }

        public static T Create<T>() where T : Component => new GameObject(typeof(T).Name).AddComponent<T>();

        public static GameObject Clone(string resourcePath, Transform parent = null, bool maintainLocalPosition = false)
        {
            var prefab = Resources.Load<GameObject>(resourcePath);
            if (prefab != null)
            {
                return Clone(prefab, parent, maintainLocalPosition);
            }
            Debug.LogWarning($"Prefab was null, tried to load from \"{resourcePath}\"\n");
            return null;
        }

        public static T Clone<T>(string resourcePath, Transform parent = null, bool maintainLocalPosition = false) where T : Component
        {
            return Clone(resourcePath, parent, maintainLocalPosition)?.GetComponent<T>();
        }

        public static GameObject Clone(GameObject original, Transform parent = null, bool maintainLocalPosition = false)
        {
            var clone = UnityEngine.Object.Instantiate(original, parent, true);
            if (maintainLocalPosition == true)
            {
                clone.transform.localPosition = original.transform.localPosition;
                clone.transform.localRotation = original.transform.localRotation;
            }
            clone.name = original.name;
            return clone;
        }

        public static T Clone<T>(GameObject original, Transform parent = null, bool maintainLocalPosition = false) where T : Component
        {
            return Clone(original, parent, maintainLocalPosition)?.GetComponent<T>();
        }

        public static void CloneOntoParent(GameObject original, Transform parent, bool maintainLocalPosition = false)
        {
            if (maintainLocalPosition == true)
            {
                parent.localPosition = original.transform.localPosition;
                parent.localRotation = original.transform.localRotation;
            }

            foreach (Transform child in original.transform)
            {
                Clone(child.gameObject, parent, maintainLocalPosition);
            }
        }

        public static string GetPathRelativeTo(Transform transform, Transform relative)
        {
            if (transform == relative)
            {
                return transform.name;
            }
            if (transform.IsChildOf(relative) == true)
            {
                return GetScenePathRelativeTo(transform, relative);
            }
            return transform.name;
        }

        public static string GetScenePath(Transform transform)
        {
            return GetScenePathRelativeTo(transform, null);
        }

        private static string GetScenePathRelativeTo(Transform transform, Transform relative)
        {
            var sb = new StringBuilder();
            if (transform != null)
            {
                GetParentPath(transform, sb, relative);
                sb.Append(transform.name);
            }
            return sb.ToString();
        }

        private static void GetParentPath(Transform transform, StringBuilder sb, Transform relative)
        {
            if (transform.parent != relative)
            {
                GetParentPath(transform.parent, sb, relative);
                sb.Append(transform.parent.name);
                sb.Append("/");
            }
        }

        public static void ShutdownApplication()
        {
#if UNITY_EDITOR
            if (Application.isEditor == true)
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else
#endif
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
    }
}
