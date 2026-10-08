using EditorTools.CatalogueViewer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public static class CatalogueTestUtility
    {
        private static Dictionary<string, GameObject[]> m_VariantPrefabs;
        //private static Dictionary<string, GameObject[]> m_PartsPrefabs;
        private static Dictionary<string, GameObject[]> m_MainPrefabs;

        public static void SetupPrefabCache()
        {
            m_VariantPrefabs = new Dictionary<string, GameObject[]>();
            //m_PartsPrefabs = new Dictionary<string, GameObject[]>();
            m_MainPrefabs = new Dictionary<string, GameObject[]>();
        }

        public static void TearDownPrefabCache()
        {
            m_VariantPrefabs = null;
            //m_PartsPrefabs = null;
            m_MainPrefabs = null;
        }

        public static void IteratePrefabs(CatalogueEntry entry, Action<GameObject> test)
        {
            var renderMetaData = entry.GetMetaData<RenderMetaData>();
            if (renderMetaData != null)
            {
                InvokeTest(test, renderMetaData.Main, entry.Guid, 0, 1, m_MainPrefabs);

                int variantCount = renderMetaData.Variants.Count;
                for (int i = 0; i < variantCount; ++i)
                {
                    var item = renderMetaData.Variants[i];
                    InvokeTest(test, item, entry.Guid, i, variantCount, m_VariantPrefabs);
                }

                int partsCount = renderMetaData.Parts.Count;
                for (int i = 0; i < partsCount; ++i)
                {
                    var item = renderMetaData.Parts[i];
                    // InvokeTest(test, item, entry.Guid, i, partsCount, m_PartsPrefabs);
                }
            }
        }

        private static void InvokeTest(Action<GameObject> test, RenderItemData renderItemData, string key, int i, int count, Dictionary<string, GameObject[]> cache)
        {
            if (cache.ContainsKey(key) == false)
            {
                cache[key] = new GameObject[count];
            }
            if (cache[key][i] == null)
            {
                cache[key][i] = PrefabManager.LoadPrefab(renderItemData);
            }
            test(cache[key][i]);
        }

        public static void IterateColliders(CatalogueEntry entry, Action<GameObject, Collider> test) => IterateComponents(entry, test);

        public static void IterateMeshes(CatalogueEntry entry, Action<GameObject, GameObject, Mesh> test)
        {
            IterateComponents<MeshCollider>(entry, (prefab, collider) => test(prefab, collider.gameObject, collider.sharedMesh));
            IterateComponents<MeshFilter>(entry, (prefab, filter) => test(prefab, filter.gameObject, filter.sharedMesh));
        }

        public static void IterateMaterials(CatalogueEntry entry, Action<GameObject, Renderer, Material> test)
        {
            IterateComponents<Renderer>(entry, (prefab, renderer) =>
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    test(prefab, renderer, mat);
                }
            });
        }

        public static void IterateTextures(CatalogueEntry entry, Action<GameObject, Renderer, Material, Texture> test)
        {
            IterateMaterials(entry, (prefab, renderer, material) =>
            {
                if (material != null)
                {
                    var shader = material.shader;
                    int count = ShaderUtil.GetPropertyCount(shader);
                    for (int i = 0; i < count; ++i)
                    {
                        var type = ShaderUtil.GetPropertyType(shader, i);
                        if (type == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            string name = ShaderUtil.GetPropertyName(shader, i);
                            var texture = material.GetTexture(name);
                            if (texture != null)
                            {
                                test(prefab, renderer, material, texture);
                            }
                        }
                    }
                }
            });
        }

        private static void IterateComponents<TComponent>(CatalogueEntry entry, Action<GameObject, TComponent> test)
        {
            IteratePrefabs(entry, prefab =>
            {
                if (prefab != null)
                {
                    var components = prefab.GetComponentsInChildren<TComponent>().ToList();
                    foreach (var component in components)
                    {
                        test(prefab, component);
                    }
                }
            });
        }

        public static List<TestCaseData> GenerateTestCases(List<CatalogueEntry> entries)
        {
            return entries.Extract(e =>
            {
                var t = new TestCaseData(e);
                t.SetName($"{{m}} [{e.Guid}] - {e.FullName}");
                return t;
            });
        }

        public static List<CatalogueEntry> ReloadEntries()
        {
            return Catalogue.Instance.Entries;
        }
    }
}
