using EditorTools;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    [TestFixture(Author = "Matt Conroy", Category = "Data", Description = "Verify Catalogue Data is present and loadable")]
    [Category("Catalogue")]
    public class CatalogueData
    {
        private static List<CatalogueEntry> m_Entries = Utils.CatalogueTestUtility.ReloadEntries();
        private static List<TestCaseData> m_CachedTestCases = Utils.CatalogueTestUtility.GenerateTestCases(m_Entries);

        [OneTimeSetUp]
        public void SetUp()
        {
            AssetDatabase.Refresh();
            Assert.That(() =>
            {
                Utils.CatalogueTestUtility.SetupPrefabCache();
                m_Entries = Utils.CatalogueTestUtility.ReloadEntries();
                m_CachedTestCases = Utils.CatalogueTestUtility.GenerateTestCases(m_Entries);
            }, Throws.Nothing);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Utils.CatalogueTestUtility.TearDownPrefabCache();
        }

        [Test, Order(1)]
        public void HasTestCases() => Assert.That(m_CachedTestCases, Is.Not.Empty);

        [Test, Order(3)]
        public void CatalogueContainsNoDuplicateEntries()
        {
            var ids = m_Entries.Extract(e => e.Guid);
            var hash = new HashSet<string>();
            foreach (string id in ids)
            {
                Assert.That(hash.Add(id), Is.True, () =>
                {
                    return $"GUID: {id} exists more than once";
                });
            }
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void InternalMetaDataExists(CatalogueEntry entry)
        {
            foreach (var kvp in entry.MetaData)
            {
                Assert.That(kvp.Value, Is.Not.Null, () => $"Internal MetaData {kvp.Key} was null");
            }
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void PrefabExists(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IteratePrefabs(entry, prefab => Assert.That(prefab, Is.Not.Null));
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void PrefabNameIsUniqueInBundle(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IteratePrefabs(entry, prefab =>
            {
                Assert.That(prefab, Is.Not.Null, () => $"There is no prefab for this  {entry}");

                string bundleName = prefab.GetAssetBundleName();
                var assets = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, prefab.name);
                Assert.That(assets, Has.Length.EqualTo(1), () => $"Asset Bundle {bundleName} contains multiple assets that match the prefab name.\n" +
                assets.ToList().Stringify());
            });
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void PrefabHasCollider(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IteratePrefabs(entry, prefab =>
            {
                if (prefab != null)
                {
                    Assert.That(prefab.GetComponentsInChildren<Collider>(), Is.Not.Empty);
                }
            });
        }


        [TestCaseSource(nameof(m_CachedTestCases))]
        public void CollidersAreNotNegativeScaled(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IterateColliders(entry, (prefab, collider) =>
            {
                Utils.Tests.AssertVectorIsNotNegative(collider.transform.localScale, () =>
                {
                    return $"{collider.GetPathRelativeTo(prefab.transform)} is negatively scaled";
                });
            });
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void CollidersAreNotNegativeSize(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IterateColliders(entry, (prefab, collider) =>
            {
                Assert.That(Utils.Tests.ColliderIsNegative(collider), Is.False, () =>
                {
                    return $"{collider.GetPathRelativeTo(prefab.transform)} has negative size";
                });
            });
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void CollidersAreConvex(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IterateColliders(entry, (prefab, collider) =>
            {
                if (collider is MeshCollider)
                {
                    Assert.That(((MeshCollider)collider).convex, Is.True, () =>
                    {
                        return $"{collider.GetPathRelativeTo(prefab.transform)} is not convex";
                    });
                }
            });
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void CollidersAreTriggers(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IterateColliders(entry, (prefab, collider) =>
            {
                Assert.That(collider.isTrigger, Is.True, () =>
                {
                    return $"{collider.GetPathRelativeTo(prefab.transform)} is not a trigger";
                });
            });
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void RenderersHaveMaterial(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IterateMaterials(entry, (prefab, renderer, material) =>
            {
                Assert.That(material, Is.Not.Null, () =>
                {
                    int index = renderer.sharedMaterials.ToList().IndexOf(material);
                    string path = renderer.GetPathRelativeTo(prefab.transform);
                    return $"{path} has a null material in slot {index}";
                });
            });
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void RenderersHaveNonDefaultMaterial(CatalogueEntry entry)
        {
            var defaultMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
            Utils.CatalogueTestUtility.IterateMaterials(entry, (prefab, renderer, material) =>
            {
                Assert.That(material, Is.Not.EqualTo(defaultMaterial), () =>
                {
                    int index = renderer.sharedMaterials.ToList().IndexOf(material);
                    string path = renderer.GetPathRelativeTo(prefab.transform);
                    return $"{path} has a default material in slot {index}";
                });
            });
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void PrefabsHaveMeshes(CatalogueEntry entry)
        {
            Utils.CatalogueTestUtility.IterateMeshes(entry, (prefab, obj, mesh) =>
            {
                Assert.That(mesh, Is.Not.Null, () =>
                {
                    return $"{obj.transform.GetPathRelativeTo(prefab.transform)} has a null mesh";
                });
            });
        }
    }
}
