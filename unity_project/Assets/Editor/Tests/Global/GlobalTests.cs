using EditorTools.Environments;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Tests
{
    [TestFixture(Author = "Matt Conroy", Category = "Data", Description = "Verify Environment data is present and loadable")]
    [Category("Global")]
    public class GlobalTests
    {
        private static List<TestCaseData> m_GlobalTestCases = Utils.EnvironmentTests.GlobalTestData;

        [OneTimeSetUp]
        public void SetUp()
        {
            AssetDatabase.Refresh();
            m_GlobalTestCases = Utils.EnvironmentTests.GlobalTestData;
        }

        [TestCaseSource(nameof(m_GlobalTestCases))]
        public void UniqueSceneNames()
        {
            string[] guidArray = AssetDatabase.FindAssets($"t:scene", null);
            Dictionary<string, string> names = new Dictionary<string, string>();
            foreach (string guid in guidArray)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string[] splitNames = path.Split('/');
                string endPart = splitNames[splitNames.Length - 1];   
                names.Add(path, endPart);
            }

            var duplicateValues = names.GroupBy(x => x.Value).Where(x => x.Count() > 1);
            if(duplicateValues.Count() > 0)
            {
                string error = "";
                foreach (var item in duplicateValues)
                {
                    error += $"{item.Key}";
                    Assert.That(error.Length == 0, Is.False, () =>
                    {
                        return error;
                    });
                }
            }


        }
    }
}
