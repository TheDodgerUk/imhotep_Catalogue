using EditorTools;
using EditorTools.Environments;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tests
{
    [TestFixture(Author = "Matt Conroy", Category = "Scene", Description = "Verify Environment data is present and loadable")]
    [Category("Environment")]
    public class EnvironmentScene
    {
        private static List<TestCaseData> m_VariantTestCases = Utils.EnvironmentTests.VariantTestData;
        private static List<TestCaseData> m_SceneTestCases = Utils.EnvironmentTests.SceneTestData;

        [OneTimeSetUp]
        public void ReloadTestCases()
        {
            AssetDatabase.Refresh();

            m_VariantTestCases = Utils.EnvironmentTests.VariantTestData;
            m_SceneTestCases = Utils.EnvironmentTests.SceneTestData;
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void CollidersAreNotNegativeSize(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            Utils.EnvironmentTests.IterateSceneComponents<Collider>(sceneAsset, (scene, root, collider) =>
            {
                string path = collider.transform.GetPathRelativeTo(root.transform);
                Assert.That(Utils.Tests.ColliderIsNegative(collider), Is.False, () =>
                {
                    return $"{path} has negative size";
                });
            });
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void AnimatorsControllerAreNotPresent(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorString = "";
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                Animator[] animatorList = root.GetComponentsInChildren<Animator>(true);
                foreach (var item in animatorList)
                {
                    bool isThereAnError = (null == item.runtimeAnimatorController);
                    BuildErrorString(ref errorString, isThereAnError, "Missing runtimeAnimatorController on Animator", $"{item.gameObject.GetGameObjectPathWithScene()}");
                }
            });
            AssertErrorMessage(ref errorString);
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void ScriptsAreNotPresentOnRootGameObjects(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                string errorString = "";
                Assert.That(root.GetComponents<MonoBehaviour>(), Is.Empty, () =>
                {
                    foreach (var item in root.GetComponentsInChildren<MonoBehaviour>())
                    {
                        BuildErrorString(ref errorString, true, $"Found scripts under root GameObject {sceneAsset.name} : {root.name}    items list:", $"{sceneAsset.name} : {item.transform.gameObject.GetGameObjectPathWithScene()}   script name : {item.GetType()}");
                    }
                    return errorString;
                });
            });
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void GameObjectsInvalidLayerForRendering(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorString = "";
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                Renderer[] rendererRefs = root.GetComponentsInChildren<Renderer>(true);
                for (int i = 0; i < rendererRefs.Length; i++)
                {
                    Renderer rendererRef = rendererRefs[i];
                }

                AssertErrorMessage(ref errorString);
            });
        }


        [TestCaseSource(nameof(m_SceneTestCases))]
        public void CachedObjectsCollider(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                string errorString = "";
                if (root.name.Contains((CachedObjects.TAG)))
                {
                    Collider[] collidersRef = root.GetComponentsInChildren<Collider>(true);
                    for (int i = 0; i < collidersRef.Length; i++)
                    {
                        Collider colliderRef = collidersRef[i];
                    }

                    AssertErrorMessage(ref errorString);
                }
            });
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void TaggedRootItemsEnabled(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                if (true == IsNamedTagged(root))
                {
                    Assert.That(root.activeSelf, Is.True, () =>
                    {
                        return $"{root.transform} is disabled";
                    });
                }
            });
        }


        [TestCaseSource(nameof(m_SceneTestCases))]
        public void TaggedRootItemsEnvironmentEffect(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string[] testAgainst = new string[] { "[ScreenDummies]", "[LEDPanelDummies]", "[ProjectorCones]", "[FastTravelSpots]", "[InfoPoints]", "[PositionTrigger]", "[RaycastTrigger]" };// these needs _    ie => [ScreenDummies]_MainScreen

            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                string rootName = root.name;
                string taggedName = string.Empty;
                if (true == GetNamedTagged(root, ref taggedName))
                {
                    if ((true == Array.Exists(testAgainst, e => e == taggedName)) && (false == rootName.Contains("_")))
                    {
                        Assert.That(true, Is.False, () =>
                        {
                            return $"{rootName} : needs an underscore _ in the name, check  =>  public abstract class EnvironmentEffect : MonoBehaviour ";
                        });
                    }
                }
            });
        }



        [TestCaseSource(nameof(m_VariantTestCases))]
        public void EnvironmentContainsLightProbes(EnvironmentVariantData variant)
        {
            Assert.That(Utils.EnvironmentTests.IterateScenes(variant, root =>
            {
                return (root.GetComponentInChildren<LightProbeGroup>() != null);
            }), Is.True);
        }


        [TestCaseSource(nameof(m_VariantTestCases))]
        public void EnvironmentContainsReflectionProbe(EnvironmentVariantData variant)
        {
            Assert.That(Utils.EnvironmentTests.IterateScenes(variant, root =>
            {
                return (root.GetComponentInChildren<ReflectionProbe>() != null);
            }), Is.True);
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void ScenePathIsBelowMaxLength_Generated(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            const int MaxPathLength = 260;
            var path = EnvironmentSceneGenerator.GetScenePath(variant, sceneAsset.name);
            Assert.That(path.Absolute, Has.Length.LessThan(MaxPathLength), () =>
            {
                string p = path.Absolute;
                return $"Generated Scene Path length must be less than {MaxPathLength}. Path length is {p.Length}\n{p}";
            });
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void SceneHasNoMissingMaterials(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            Utils.EnvironmentTests.IterateSceneMaterials(sceneAsset, (scene, root, renderer, material) =>
            {
                int index = renderer.sharedMaterials.ToList().IndexOf(material);
                Assert.That(material, Is.Not.Null, () =>
                {
                    string path = renderer.transform.GetGameObjectPathWithScene();
                    return $"{path} has a null material in slot {index}";
                });
            });
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void SceneHasNoMissingMeshes(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            Utils.EnvironmentTests.IterateSceneMeshes(sceneAsset, (scene, root, obj, mesh) =>
            {
                Assert.That(mesh, Is.Not.Null, () =>
                {
                    return $"{obj.transform.GetPathRelativeTo(root.transform)} has a null mesh";
                });
            });
        }


        [TestCaseSource(nameof(m_SceneTestCases))]
        public void SceneObjectsAreNotNegativelyScaled(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorMessage = "";
            Utils.EnvironmentTests.IterateSceneComponents<Transform>(sceneAsset, (scene, root, transform) =>
            {
                if (transform.localScale.x < 0 || transform.localScale.y < 0 || transform.localScale.z < 0)
                {
                    BuildErrorString(ref errorMessage, true, "is negatively scaled", $"{transform.GetGameObjectPathWithScene()}, current LocalScale: {transform.localScale}");
                }
            });
            AssertErrorMessage(ref errorMessage);
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void SceneObjectsAreScaledToOne(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorMessage = "";
            Utils.EnvironmentTests.IterateSceneComponents<Transform>(sceneAsset, (scene, root, transform) =>
            {
                if (false == (transform is RectTransform)) // if its a RectTransform then the scale can be odd
                {
                    bool close = Mathf.Approximately(transform.localScale.sqrMagnitude, 3.0f);
                    BuildErrorString(ref errorMessage, (close == false), "Is not scaled to 1", $"Scene: {transform.GetGameObjectPathWithScene()}, current magnitude: {transform.localScale.magnitude}");
                }
            });
            AssertErrorMessage(ref errorMessage);
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void SceneObjectsStaticFlagsAreSet_Lightmap(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            Utils.EnvironmentTests.IterateSceneComponents<Transform>(sceneAsset, (scene, root, transform) =>
            {
                AssertStaticFlags(root, transform, StaticEditorFlags.ContributeGI, new[]
                {
                    StaticEditorFlags.OccluderStatic,
                    StaticEditorFlags.BatchingStatic,
                    StaticEditorFlags.OccludeeStatic,
                    StaticEditorFlags.ReflectionProbeStatic,
                });
            });
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void SceneObjectsStaticFlagsAreSet_Navigation(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            Utils.EnvironmentTests.IterateSceneComponents<Transform>(sceneAsset, (scene, root, transform) =>
            {
                AssertStaticFlags(root, transform, StaticEditorFlags.NavigationStatic, new[]
                {
                    StaticEditorFlags.NavigationStatic,
                    StaticEditorFlags.OffMeshLinkGeneration,
                });
            });
        }

        private static void AssertStaticFlags(GameObject root, Transform transform, StaticEditorFlags targetFlag, StaticEditorFlags[] matchingFlags)
        {
            var flags = GameObjectUtility.GetStaticEditorFlags(transform.gameObject);
            if (flags.HasFlag(targetFlag) == true)
            {
                foreach (var flag in matchingFlags)
                {
                    Assert.That(flags.HasFlag(flag), () =>
                    {
                        string path = transform.GetGameObjectPathWithScene();
                        return $"{path} is marked as {targetFlag}, but isn't {flag}";
                    });
                }
            }
        }



        [TestCaseSource(nameof(m_SceneTestCases))]
        public void WallsAreNonNavigationStatic(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorString = "";
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                Transform[] transformRefs = root.GetComponentsInChildren<Transform>(true);
                foreach (var transformRef in transformRefs)
                {
                    if (true == transformRef.name.CaseInsensitiveContains("Wall") && null != transformRef.GetComponent<Collider>())
                    {
                        BuildErrorString(ref errorString, transformRef.gameObject.HasStaticFlag(StaticEditorFlags.NavigationStatic), "The Following have StaticEditorFlags.NavigationStatic, with colliders, you would be able to teleport to these ", $"{transformRef.gameObject.GetGameObjectPathWithScene()}");
                    }
                }
            });
            AssertErrorMessage(ref errorString);
        }


        [TestCaseSource(nameof(m_SceneTestCases))]
        public void CeilingAreNonNavigationStatic(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorString = "";
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                Transform[] transformRefs = root.GetComponentsInChildren<Transform>(true);
                foreach (var transformRef in transformRefs)
                {
                    if (transformRef.name.CaseInsensitiveContains("Ceiling") && null != transformRef.GetComponent<Collider>())
                    {
                        BuildErrorString(ref errorString, transformRef.gameObject.HasStaticFlag(StaticEditorFlags.NavigationStatic), "The Following have StaticEditorFlags.NavigationStatic, with colliders, you would be able to teleport to these ", $"{transformRef.gameObject.GetGameObjectPathWithScene()}");
                    }
                }
            });
            AssertErrorMessage(ref errorString);
        }


        [TestCaseSource(nameof(m_SceneTestCases))]
        public void TriggersCollidersAreCorrectlySet(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorString = "";
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                if (true == root.name.CaseInsensitiveContains("[PositionTrigger]") || true == root.name.CaseInsensitiveContains("[RaycastTrigger]"))
                {
                    Collider[] transformArray = root.GetComponentsInChildren<Collider>(true);
                    foreach (var colliderRef in transformArray)
                    {

                    }
                }
            });
            AssertErrorMessage(ref errorString);
        }

        [TestCaseSource(nameof(m_SceneTestCases))]
        public void TriggersColliderDoNotHaveRendersAttached(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorString = "";
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                if (true == root.name.CaseInsensitiveContains("[PositionTrigger]") || true == root.name.CaseInsensitiveContains("[RaycastTrigger]"))
                {
                    Collider[] transformArray = root.GetComponentsInChildren<Collider>(true);
                    foreach (var colliderRef in transformArray)
                    {
                        Renderer rendererRef = colliderRef.GetComponent<Renderer>();
                        BuildErrorString(ref errorString, (rendererRef != null), $"renderer cannot be on same Gameobject as collider, to make collider work its on a layer the camera does not render", $"{colliderRef.gameObject.GetGameObjectPathWithScene()}");
                    }
                }
            });
            AssertErrorMessage(ref errorString);
        }


        [TestCaseSource(nameof(m_SceneTestCases))]
        public void TriggersBaseAreValid(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorString = "";
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                if (true == root.name.CaseInsensitiveContains("[PositionTrigger]") || true == root.name.CaseInsensitiveContains("[RaycastTrigger]"))
                {
                    List<Transform> transformList = root.transform.GetDirectChildren();
                    bool hasCollider = false;
                    foreach (var transformRef in transformList)
                    {
                        Collider colliderRef = transformRef.GetComponentInChildren<Collider>(true);
                        if (null != colliderRef)
                        {
                            hasCollider = true;
                        }
                    }
                    if (false == hasCollider)
                    {
                        BuildErrorString(ref errorString, true, $"Missing Collider on child", $"{root.gameObject.GetGameObjectPathWithScene()}");

                    }
                }
            });
            AssertErrorMessage(ref errorString);
        }


        [TestCaseSource(nameof(m_SceneTestCases))]
        public void CheckForValidationHeleprsOutcome(EnvironmentVariantData variant, SceneAsset sceneAsset)
        {
            string errorString = "";
            Utils.EnvironmentTests.IterateSceneRoots(sceneAsset, (scene, root) =>
            {
                BaseValidation[] receiverArray = root.gameObject.GetComponentsInChildren<BaseValidation>(true);
                foreach (var receiver in receiverArray)
                {
                    BuildErrorString(ref errorString, false == receiver.IsTheNameValid(), "Incorrect gameObjectName", $" {receiver.gameObject.GetGameObjectPathWithScene()}, the name was invalid, run the helper script on it");                   
                }
            });
            AssertErrorMessage(ref errorString);
        }


        private void ValidationHeleprs<T>(Transform trans, ref string errorMessage) where T : MonoBehaviour
        {
            if(null == trans.gameObject.GetComponent<T>())
            {
                BuildErrorString(ref errorMessage, true, "Missing Validations", $" {trans.gameObject.GetGameObjectPathWithScene()} was missing {nameof(T)}");
                trans.gameObject.ForceComponent<T>();
            }            
        }


        private void BuildErrorString(ref string errorString, bool addToError, string header, string error)
        {
            if (true == addToError)
            {
                if (true == string.IsNullOrEmpty(errorString))
                {
                    errorString = header + "\n";
                }
                errorString += "---------" + error + "\n";
            }
        }

        private void AssertErrorMessage(ref string errorString)
        {
            string localError = errorString;
            Assert.That(errorString.Length == 0, Is.True, () =>
            {
                return localError;
            });
        }


        public static bool IsNamedTagged(GameObject objectRef)
        {
            string namedTag = "";
            return GetNamedTagged(objectRef, ref namedTag);
        }

        public static bool GetNamedTagged(GameObject objectRef, ref string taggedName)
        {
            var match = System.Text.RegularExpressions.Regex.Match(objectRef.name, @"\[(\w+)\]");
            taggedName = match.Value;
            return match.Success;
        }

        public static string GetObjectName(GameObject gameObjectRef)
        {
            var split = gameObjectRef.name.Split('_');
            if (split.Length > 1)
            {
                return split[1];
            }
            return null;
        }
    }
}
