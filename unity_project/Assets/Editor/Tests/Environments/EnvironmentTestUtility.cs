using EditorTools;
using EditorTools.Environments;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class EnvironmentTests
    {
        public static List<TestCaseData> GlobalTestData => GetGlobalTestCaseData();
        public static List<TestCaseData> VariantTestData => GetVariantTestCaseData();
        public static List<TestCaseData> SceneTestData => GetSceneTestCaseData();

        public static bool IterateScenes(EnvironmentVariantData variant, Predicate<GameObject> test)
        {
            return variant.OriginalScenes.Exists(s =>
            {
                var scene = OpenScene(s);
                foreach (var root in scene.GetRootGameObjects())
                {
                    if (test(root) == true)
                    {
                        return true;
                    }
                }
                return false;
            });
        }

        public static void IterateSceneRoots(SceneAsset sceneAsset, Action<Scene, GameObject> test)
        {
            var scene = OpenScene(sceneAsset);
            foreach (var root in scene.GetRootGameObjects())
            {
                test(scene, root);
            }
        }

        private static Scene OpenScene(SceneAsset sceneAsset)
        {
            string path = sceneAsset.GetAssetPath().Relative;
            var scene = SceneManager.GetSceneByPath(path);
            if (scene.IsValid() == false)
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            }
            return scene;
        }

        public static void IterateSceneComponents<T>(SceneAsset sceneAsset, Action<Scene, GameObject, T> test) where T : Component
        {
            IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                foreach (var component in root.GetComponentsInChildren<T>())
                {
                    test(scene, root, component);
                }
            });
        }


        public static void IterateSceneCachedObjects(SceneAsset sceneAsset, Action<Scene, GameObject, Transform> test)
        {
            IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                foreach (Transform item in root.transform.GetDirectChildren())
                {
                    test(scene, root, item);
                }
            });
        }



        public static void IterateSceneMaterials(SceneAsset sceneAsset, Action<Scene, GameObject, Renderer, Material> test)
        {
            IterateSceneComponents<Renderer>(sceneAsset, (scene, root, renderer) =>
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    test(scene, root, renderer, mat);
                }
            });
        }

        public static void IterateSceneMeshes(SceneAsset sceneAsset, Action<Scene, GameObject, GameObject, Mesh> test)
        {
            IterateSceneComponents<MeshCollider>(sceneAsset, (scene, root, collider) => test(scene, root, collider.gameObject, collider.sharedMesh));
            IterateSceneComponents<MeshFilter>(sceneAsset, (scene, root, filter) => test(scene, root, filter.gameObject, filter.sharedMesh));
        }

        private static List<TestCaseData> GetSceneTestCaseData()
        {
            return GetTestCaseData(v =>
            {
                List<SceneAsset> combined = v.OriginalScenes;
                combined.AddRange(v.OptionalScenes);
                return combined.Extract(s =>
                {
                    string name = $"{GetTestCaseName(v)} {s.name}";
                    return new TestCaseData(v, s).SetName(name);
                });
            });
        }

        private static List<TestCaseData> GetVariantTestCaseData()
        {
            return GetTestCaseData(v =>
            {
                return new TestCaseData(v).SetName(GetTestCaseName(v));
            });
        }


        private static List<TestCaseData> GetGlobalTestCaseData()
        {
            List<TestCaseData> list = new List<TestCaseData>
            {
                new TestCaseData()
            };
            return list;
        }

        

        private static List<TestCaseData> GetTestCaseData(Func<EnvironmentVariantData, List<TestCaseData>> selector)
        {
            return LoadEnvironmentData().Extract(e => e.Variants).Flatten().Extract(selector).Flatten();
        }

        private static List<TestCaseData> GetTestCaseData(Func<EnvironmentVariantData, TestCaseData> selector)
        {
            return GetTestCaseData(v => selector(v).CreateList());
        }

        private static string GetTestCaseName(EnvironmentVariantData v) => $"{{m}} [{v.Environment.Name} - {v.Name}]";

        public static List<EnvironmentSceneData> LoadEnvironmentData()
        {
            var environmentManager = new EnvironmentDataManager();
            environmentManager.LoadFromFile();

            var environments = environmentManager.Environments;
            //foreach (var id in IgnoreEnvironments)
            //{
            //    environments.RemoveAll(e => e.Guid == id);
            //}

            //foreach (var environment in environments)
            //{
            //    foreach (var id in IgnoreVariants)
            //    {
            //        environment.Variants.RemoveAll(v => v.Guid == id);
            //    }
            //}

            foreach (var environment in environments)
            {
                environment.Variants.RemoveAll(v => v.IncludeInUnitTests == false);
            }

            foreach (var environment in environments)
            {
             //   environments.RemoveAll(e => e.Variants.Count == 0);
            }

            return environments;
        }

        private static readonly List<string> IgnoreEnvironments = new List<string>
        {
            "749aeaf7-bcab-4592-b06c-46b2a95e3769"
        };

        private static readonly List<string> IgnoreVariants = new List<string>
        {
            "a3c5f47d-ed8e-477d-88d2-54c6cd142763",
            "3c1814ee-de5f-4912-8a8a-c7af267d9637",
            "3b09cde3-cad2-40b5-a3d3-b76d3b7054f9"
        };
    }
}
