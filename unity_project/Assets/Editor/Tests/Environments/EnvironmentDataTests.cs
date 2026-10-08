using EditorTools.Environments;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    [TestFixture(Author = "Matt Conroy", Category = "Data", Description = "Verify Environment data is present and loadable")]
    [Category("Environment")]
    public class EnvironmentData
    {
        private static List<TestCaseData> m_VariantTestCases = Utils.EnvironmentTests.VariantTestData;

        [OneTimeSetUp]
        public void SetUp()
        {
            AssetDatabase.Refresh();
            m_VariantTestCases = Utils.EnvironmentTests.VariantTestData;
        }

        //[Test, Order(0)]
        //public void HasTestCases()
        //{
        //    Assert.That(m_VariantTestCases, Is.Not.Empty);
        //}

        [Test, Order(0)]
        public void CanLoadEnvironments() => Assert.That(() => Utils.EnvironmentTests.LoadEnvironmentData(), Throws.Nothing);

        [Test, Order(1)]
        public void EnvironmentDataExists() => Assert.That(Utils.EnvironmentTests.LoadEnvironmentData(), Is.Not.Empty);

        [TestCaseSource(nameof(m_VariantTestCases))]
        public void OriginalScenesExist(EnvironmentVariantData variant)
        {
            Assert.That(variant.OriginalScenes, Does.Not.Contain(null), () =>
            {
                return $"{variant.Guid} has a null scene at index {variant.OriginalScenes.IndexOf(null)}";
            });
        }

    }
}
