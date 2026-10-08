using EditorTools;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    [TestFixture(Author = "Matt Conroy", Category = "Model", Description = "Verify Assets used for Catalogue Prefabs are valid & within restraints")]
    [Category("Catalogue")]
    public class CatalogueModel
    {
        private static List<TestCaseData> m_CachedTestCases = Utils.CatalogueTestUtility.GenerateTestCases(Utils.CatalogueTestUtility.ReloadEntries());

        private const int MaxPolyCount = 36000;
        private const int MaxTextureSize = 2048;

        [OneTimeSetUp]
        public void SetUp()
        {
            AssetDatabase.Refresh();
            Utils.CatalogueTestUtility.SetupPrefabCache();
            var entries = Utils.CatalogueTestUtility.ReloadEntries();
            m_CachedTestCases = Utils.CatalogueTestUtility.GenerateTestCases(entries);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Utils.CatalogueTestUtility.TearDownPrefabCache();
        }

        [Test, Order(0)]
        public void HasTestCases() => Assert.That(m_CachedTestCases, Is.Not.Empty);

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void ImportedTexturesAreNotAboveMaxSize(CatalogueEntry entry)
        {
            if (true == entry.IncludeInUnitTests)
            {
                Utils.CatalogueTestUtility.IterateTextures(entry, (prefab, renderer, material, texture) =>
                {
                    Assert.That(texture.width, Is.LessThanOrEqualTo(MaxTextureSize), $"{texture.name}'s imported width is over {MaxTextureSize}");
                    Assert.That(texture.height, Is.LessThanOrEqualTo(MaxTextureSize), $"{texture.name}'s imported height is over {MaxTextureSize}");
                });
            }
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void SourceTexturesAreNotAboveMaxSize(CatalogueEntry entry)
        {
            if (true == entry.IncludeInUnitTests)
            {
                Utils.CatalogueTestUtility.IterateTextures(entry, (prefab, renderer, material, texture) =>
                {
                    var actualSize = GetActualTextureSize(texture);
                    Assert.That(actualSize.x, Is.LessThanOrEqualTo(MaxTextureSize), $"{texture.name}'s actual width is over {MaxTextureSize}");
                    Assert.That(actualSize.y, Is.LessThanOrEqualTo(MaxTextureSize), $"{texture.name}'s actual height is over {MaxTextureSize}");
                });
            }
        }

        [TestCaseSource(nameof(m_CachedTestCases))]
        public void ImportedMeshesAreNotAboveMaxPolyCount(CatalogueEntry entry)
        {
            if (true == entry.IncludeInUnitTests)
            {
                Utils.CatalogueTestUtility.IterateMeshes(entry, (prefab, obj, mesh) =>
                {
                    for (int i = 0; i < mesh.subMeshCount; ++i)
                    {
                        int polyCount = mesh.GetTriangles(i).Length / 3;
                        Assert.That(polyCount, Is.LessThanOrEqualTo(MaxPolyCount), $"{mesh.name}'s poly count ({polyCount.ToString("N0")}) is over {MaxPolyCount.ToString("N0")}");
                    }
                });
            }
        }

        private Vector2 GetActualTextureSize(Texture texture)
        {
            const string MethodName = "GetWidthAndHeight";
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            var method = typeof(TextureImporter).GetMethod(MethodName, flags);

            var args = new object[2] { 0, 0 };
            method.Invoke(texture.GetImporter(), args);
            return new Vector2((int)args[0], (int)args[1]);
        }
    }
}
