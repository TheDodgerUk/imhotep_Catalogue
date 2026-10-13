using FilePathHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GlobalConsts;

namespace EditorTools.Environments
{
    public class EnvironmentSceneGenerator
    {
        public static List<VFXReplaceInScene> m_VFXReplaceInScene = new List<VFXReplaceInScene>();
        private const string BundleExtension = Bundles.AssetBundleBuilder.BundleExtension;
        public static FilePath GeneratedSceneRoot => FilePath.FromRelative("Assets/Artwork/Scenes/GeneratedScenes");

        private static readonly List<GeneratedSceneMap> ExtraScenes = new List<GeneratedSceneMap>
        {
            new GeneratedSceneMap("Lighting", new Dictionary<Type, Predicate<Component>>
            {
                { typeof(Light), ShouldIgnoreLight },
                { typeof(LightProbeGroup), ShouldIgnoreLight },
                { typeof(ReflectionProbe), ShouldIgnoreLight },
            }, typeof(Light), typeof(LightProbeGroup), typeof(ReflectionProbe)),

            new GeneratedSceneMap("Collision", new Dictionary<Type, Predicate<Component>>
            {
            }, typeof(Collider)),
        };

        public static List<string> GetAllGeneratedScenes() => Utils.IO.GetFiles(GeneratedSceneRoot.Absolute, "*.unity", true);

        public static List<string> GetGeneratedSceneNames(EnvironmentVariantData v)
        {

            var scenes = v.OriginalScenes.Extract(s => GetGeneratedSceneName(v, s));
            for (int i = 0; i < ExtraScenes.Count; ++i)
            {
                scenes.Insert(i, GetSceneName(v, i, ExtraScenes[i].Name));
            }
            return scenes;
        }

        public static List<string> GetOptionalGeneratedSceneNames(EnvironmentVariantData v)
        {
            var scenes = v.OptionalScenes.Extract(s => GetGeneratedSceneName(v, s));
            return scenes;
        }



        public static void GenerateScenes(EnvironmentSceneData environment, EnvironmentVariantData variant)
        {
            List<Scene> loadedScenes = EnvironmentSceneManager.OpenScenes(variant.OriginalScenes);
            // load up the scenes we are about to copy from
            if (loadedScenes == null)
            {
                Debug.LogError($"Failed to open original scenes\n");
                return;
            }

            // destroy existing generated scenes
            CleanTargetFolder(variant);

            // empty the generated scenes list
            variant.GeneratedScenes.Clear();


            GenerateDuplicateScenes(variant, variant.OriginalScenes, true);
           

            // load up the scenes we are about to copy from
            if (EnvironmentSceneManager.OpenScenes(variant.GeneratedScenes) == null)
            {
                Debug.LogError($"Failed to open generated scenes\n");
                return;
            }

            using (var progressBar = new ProgressBar("Generating Scenes", ExtraScenes))
            {
                //Populate lighting and collision scene if we are not combining into
                //single scene
                // populate extra scenes
                for (int i = 0; i < ExtraScenes.Count; ++i)
                {
                    progressBar.UpdateIncrement($"Generating {ExtraScenes[i].Name} Scene");
                    PopulateScene(variant, variant.GeneratedScenes[i], ExtraScenes[i]);
                }


                //// layer all colliders
                //ProcessComponentsInScene<Collider>(c =>
                //{
                //    c.gameObject.layer = Layers.EnvironmentLayer;
                //});

                // remove all scripts

                m_VFXReplaceInScene.Clear();
                RemoveMono();
                RemoveComponent();

                Scene FinalLightBakeSettings = SceneManager.GetSceneByName(variant.Guid+"_"+"_MT_LightBake-HighQuality");

                if (FinalLightBakeSettings.IsValid())
                { 
                    SceneManager.SetActiveScene(FinalLightBakeSettings);
                }


                // save scenes
                EnvironmentSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private static void RemoveMono()
        {
            ProcessComponentsInScene<MonoBehaviour>(mb =>
            {
                if (null != mb)
                {
                    if (null == mb.GetType())
                    {
                        Debug.Log($"Null script detected in scene on object {mb}");
                    }
                    var type = mb.GetType();;
                    if (mb.GetType().IsInUserAssembly() == true)
                    {
                        bool remove = true;

                        foreach (var item in GlobalConsts.DO_NOT_REMOVE)
                        {
                            if (item == type)
                            {
                                remove = false;
                            }
                        }
                        foreach (var item in GlobalConsts.REMOVE)
                        {
                            if (item == type)
                            {
                                Debug.LogError($"Remove {item}");
                                remove = true;
                            }
                        }
                        if (remove == true)
                        {
                            Undo.DestroyObjectImmediate(mb);
                        }
                    }
                    else
                    {
                        bool remove = false;
                        foreach (var item in GlobalConsts.REMOVE)
                        {
                            if (item == type)
                            {
                                Debug.LogError($"Remove {item}");
                                remove = true;
                            }
                        }
                        if (remove == true)
                        {
                            Undo.DestroyObjectImmediate(mb);
                        }

                    }
                }
                else
                {
                    Debug.LogError("Something went wrong doing again, doing again so you can track");
                    ProcessComponentsInScene<MonoBehaviour>(tryAgain =>
                    {
                        int ff = 0;
                    });
                }
            });
        }


        private static void RemoveComponent()
        {
            ProcessComponentsInScene<Component>(mb =>
            {
                if (null != mb)
                {
                    if (null == mb.GetType())
                    {
                        Debug.Log($"Null script detected in scene on object {mb}");
                    }
                    var type = mb.GetType(); ;
                    if (mb.GetType().IsInUserAssembly() == true)
                    {
                        bool remove = true;

                        foreach (var item in GlobalConsts.DO_NOT_REMOVE)
                        {
                            if (item == type)
                            {
                                remove = false;
                            }
                        }
                        foreach (var item in GlobalConsts.REMOVE)
                        {
                            if (item == type)
                            {
                                Debug.LogError($"Remove {item}");
                                remove = true;
                            }
                        }
                        if (remove == true)
                        {
                            Undo.DestroyObjectImmediate(mb);
                        }
                    }
                    else
                    {
                        bool remove = false;
                        foreach (var item in GlobalConsts.REMOVE)
                        {
                            if (item == type)
                            {
                                if(type == typeof(UnityEngine.VFX.VisualEffect))
                                {
                                    VFXReplaceInScene vFXReplaceInScene = new VFXReplaceInScene();
                                    vFXReplaceInScene.VfxName = ((UnityEngine.VFX.VisualEffect)mb).visualEffectAsset.name;
                                    vFXReplaceInScene.HierarchyPath = mb.gameObject.GetGameObjectPath();
                                    vFXReplaceInScene.HierarchyPathRoot = vFXReplaceInScene.HierarchyPath.Split('/')[0];
                                    vFXReplaceInScene.Scene = mb.gameObject.scene.name;
                                    m_VFXReplaceInScene.Add(vFXReplaceInScene);
                                }
          
                                Debug.LogError($"Remove {item}");
                                remove = true;
                            }
                        }
                        if (remove == true)
                        {
                            Undo.DestroyObjectImmediate(mb);
                        }

                    }
                }
                else
                {
                    Debug.LogError("Something went wrong doing again, doing again so you can track");
                    ProcessComponentsInScene<MonoBehaviour>(tryAgain =>
                    {
                        int ff = 0;
                    });
                }
            });
        }

        private static void GenerateCombinedScene(List<Scene> loadedScenes, EnvironmentVariantData variant)
        {
            int progressCount = loadedScenes.Count;
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

            using (var progressBar = new ProgressBar("Combining Scenes", progressCount))
            {
                foreach (var loaded in loadedScenes)
                {
                    progressBar.UpdateIncrement($"Combining {loaded.name}");

                    GameObject[] roots = loaded.GetRootGameObjects();

                    foreach (GameObject root in roots)
                    {
                        GameObject clone = Utils.Unity.Clone(root, null, true);
                        SceneManager.MoveGameObjectToScene(clone, scene);
                    }
                }

                // generate scene name + path
                var targetPath = GetScenePath(variant, "Combined");

                // ensure that the destination folder path exists
                Utils.IO.ForceFolder(targetPath.GetFolder().Absolute);

                // if the file already exists, destroy it
                if (Utils.IO.FileExists(targetPath.Absolute) == true)
                {
                    Utils.IO.DeleteFile(targetPath.Absolute);
                }

                //Save combined scene
                EditorSceneManager.SaveScene(scene, targetPath.Relative);

                // load the newly generated scene and set the correct asset bundle label
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(targetPath.Relative);
                sceneAsset.GetImporter().assetBundleName = GetBundleLabel(variant);
                variant.GeneratedScenes.Add(sceneAsset);
            }
        }

        public static void GenerateDuplicateScenes(EnvironmentVariantData variant, List<SceneAsset> sceneAssetList, bool includeExtraScenes)
        {
            int progressCount = sceneAssetList.Count + ExtraScenes.Count;
            using (var progressBar = new ProgressBar("Generating Scenes", progressCount))
            {
                if (true == includeExtraScenes)
                {
                    foreach (var extraScene in ExtraScenes)
                    {
                        progressBar.UpdateIncrement($"Generating {extraScene.Name}");

                        // generate scene name + path
                        var targetPath = GetScenePath(variant, extraScene.Name);

                        // ensure that the destination folder path exists
                        Utils.IO.ForceFolder(targetPath.GetFolder().Absolute);

                        // if the file already exists, destroy it
                        if (Utils.IO.FileExists(targetPath.Absolute) == true)
                        {
                            Utils.IO.DeleteFile(targetPath.Absolute);
                        }

                        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                        if (EditorSceneManager.SaveScene(scene, targetPath.Relative) == false)
                        {
                            Debug.LogError($"Failed to create extra scene \"{extraScene.Name}\"\n");
                            return;
                        }
 
                        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(targetPath.Relative);
                        sceneAsset.GetImporter().assetBundleName = GetBundleLabel(variant);
                        variant.GeneratedScenes.Add(sceneAsset);
                    }
                }
                // copy each original scene file to its new location to be the generated scene
                foreach (var sceneAssetRef in sceneAssetList)
                {
                    progressBar.UpdateIncrement($"Generating {sceneAssetRef.name}");

                    // generate scene name + path
                    var targetPath = GetScenePath(variant, sceneAssetRef.name);

                    // ensure that the destination folder path exists
                    Utils.IO.ForceFolder(targetPath.GetFolder().Absolute);

                    // if the file already exists, destroy it
                    if (Utils.IO.FileExists(targetPath.Absolute) == true)
                    {
                        Utils.IO.DeleteFile(targetPath.Absolute);
                    }


                    // read the original file into a string and write it back out to the new path
                    var content = Utils.IO.ReadBytesFromFile(sceneAssetRef.GetAssetPath().Absolute);
                    Utils.IO.WriteBytesToFile(targetPath.Absolute, content);
                    AssetDatabase.Refresh();

                    // load the newly generated scene and set the correct asset bundle label
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(targetPath.Relative);
                    sceneAsset.GetImporter().assetBundleName = GetBundleLabel(variant);
                    variant.GeneratedScenes.Add(sceneAsset);
                    DisableUnwantedCode(targetPath);
                }
            }
        }

        private static void DisableUnwantedCode(FilePath targetPath)
        {
            var scene = EditorSceneManager.OpenScene(targetPath.Relative);
            var items = scene.GetRootGameObjects();
            foreach (var item in items)
            {
                string taggedName = "";
                if(false == EnvironmentHelpers.Helpers.GetNamedTagged(item, ref taggedName))
                {
                    continue;
                }
                switch (taggedName)
                {
                    case "[CatalogeOnly]":
                        item.SetActive(false);
                        break;
                }
            }
        }

        private static void CleanTargetFolder(EnvironmentVariantData variant)
        {
            try
            {
                // destroy the existing scenes in the folder (top level files will be destroyed only)
                if (Utils.IO.FolderExists(GeneratedSceneRoot.Absolute) == true)
                {
                    var files = Directory.GetFiles(GeneratedSceneRoot.Absolute, $"{variant.Guid}*");
                    using (var progressBar = new ProgressBar("Cleaning Scenes", files.Length))
                    {
                        foreach (var file in files)
                        {
                            var path = FilePath.FromAbsolute(file);
                            progressBar.UpdateIncrement($"Removing {path.Relative}");

                            if (path.Extension != ".meta")
                            {
                                AssetDatabase.MoveAssetToTrash(path.Relative);
                            }
                        }
                    }
                }
                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static void PopulateScene(EnvironmentVariantData variant, SceneAsset sceneAsset, GeneratedSceneMap generatedSceneMap)
        {
            var scene = SceneManager.GetSceneByName(sceneAsset.name);
            foreach (var type in generatedSceneMap.Types)
            {
                // find target components in all scenes
                var components = new List<Component>();
                ProcessGameObjectsInScene(root =>
                {
                    components.AddRange(root.GetComponentsInChildren(type));
                });

                // filter components based on type
                var filter = generatedSceneMap.Filters.Get(type);
                if (filter != null)
                {
                    components.RemoveAll(filter);
                }

                // group components to gameobject and move to scene
                Debug.Log("this is were is splits up the colliders and lighting");
                ////////var go = GroupToGameObject(components, type);
                ////////SceneManager.MoveGameObjectToScene(go, scene);
            }

            // save scene to file
            EditorSceneManager.SaveScene(scene);
        }

        private static bool ShouldIgnoreLight(Component component)
        {
            if (component is Light)
            {
                var light = (Light)component;
                if(light.lightmapBakeType == LightmapBakeType.Realtime)
                {
                    return true;
                }
                else
                {
                    if(light.isActiveAndEnabled == false)
                    {
                        return true;
                    }
                }
            }
            if (component is ReflectionProbe)
            {
                var reflection = (ReflectionProbe)component;
                if(reflection.hdr == true)
                {
                    Debug.LogError($"Component {component.name}'s ReflectionProbe HDR is now forced off. If left on, the image comes across blown out.");
                    reflection.hdr = false;
                }
                
                if (reflection.mode == UnityEngine.Rendering.ReflectionProbeMode.Realtime)
                {
                    return true;
                }
                else
                {
                    if (reflection.isActiveAndEnabled == false)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private static void ProcessGameObjectsInScene(Action<GameObject> process)
        {
            for (int i = 0; i < SceneManager.loadedSceneCount; ++i)
            {
                foreach (var root in SceneManager.GetSceneAt(i).GetRootGameObjects())
                {
                    process(root);
                }
            }
        }

        private static void ProcessComponentsInScene<T>(Action<T> process)
        {
            ProcessGameObjectsInScene(root =>
            {
                var gameObject = root.gameObject.GetGameObjectPath();
                var components = root.GetComponentsInChildren<T>(true);
                foreach (var component in components)
                {
                    if(null == component)
                    {
                        Debug.Log($"Null Script on: {root.name}");
                    }
                    process(component);
                }
            });
        }

        private static GameObject GroupToGameObject(List<Component> components, Type type)
        {
            bool isColliderType = (type == typeof(Collider));
            var clonedObjects = new List<GameObject>();

            // create root object
            var go = new GameObject(type.Name) { isStatic = true };
            
            foreach (var component in components)
            {
                var gameObject = component.gameObject;

                if(true == isColliderType)
                {
                    if((null != gameObject.GetComponent<Renderer>()) && (null != gameObject.GetComponent<Collider>()))
                    {
                        Debug.LogError($"Collider has a Renderer on it: {gameObject.GetGameObjectPath()} ", gameObject);
                    }
                }
                // has this object already been cloned
                if (HasBeenCloned(clonedObjects, gameObject) == false)
                {
                    clonedObjects.Add(gameObject);

                    // clone gameobject attached to component
                    var clone = Utils.Unity.Clone(gameObject, go.transform);

                    // get all components under the clone that are not the correct type
                    var cloneComponents = clone.GetComponentsInChildren<Component>(true).ToList();
                    foreach (var typedComponent in clone.GetComponentsInChildren(type, true))
                    {
                        cloneComponents.Remove(typedComponent);
                    }
                    cloneComponents.RemoveAll(c => c is Transform);

                    // remove all components under clone that are not the correct type
                    foreach (var cloneComponent in cloneComponents)
                    {
                        cloneComponent.DestroyObjectImmediate();
                    }

                    // destroy children of clone that do not have any components
                    CleanEmptyGameObjects(clone, true);
                }
            }

            // destroy components from generated scenes
            foreach (var component in components)
            {
                if (component != null)
                {
                    var gameObject = component.gameObject;

                    // destroy original component
                    component.DestroyObjectImmediate();

                    // destroy original object if it is now empty
                    CleanEmptyGameObjects(gameObject, false);
                }
            }
            return go;
        }

        private static void CleanEmptyGameObjects(GameObject gameObject, bool cleanChildren)
        {
            var transforms = gameObject.GetComponentsInChildren<Transform>(true);
            var components = gameObject.GetComponentsInChildren<Component>(true);

            // if the only components in this parent tree are transforms
            if (transforms.Length == components.Length)
            {
                if (PrefabInstanceStatus.Connected == PrefabUtility.GetPrefabInstanceStatus(gameObject))
                {
                    var root = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
                    PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    // destroy along with children
                    gameObject.DestroyObjectImmediate();
                }
            }
            else if (cleanChildren == true)
            {
                // check each child recursively
                foreach (var child in gameObject.transform.GetDirectChildren())
                {
                    CleanEmptyGameObjects(child.gameObject, cleanChildren);
                }
            }
        }

        private static bool HasBeenCloned(List<GameObject> clonedObjects, GameObject gameObject)
        {
            if (clonedObjects.Contains(gameObject) == true)
            {
                return true;
            }
            else
            {
                var parent = gameObject.transform.parent;
                if (parent != null)
                {
                    return HasBeenCloned(clonedObjects, parent.gameObject);
                }
                else
                {
                    return false;
                }
            }
        }

        private static void AttemptToCleanGameObject(GameObject go, Action<GameObject> destroyHandler)
        {
            var parent = go.transform.parent;
            if (IsGameObjectEmpty(go) == true)
            {
                destroyHandler(go);
            }
            if (parent != null)
            {
                AttemptToCleanGameObject(parent.gameObject, destroyHandler);
            }
        }

        private static bool IsGameObjectEmpty(GameObject go)
        {
            if (go.transform.childCount == 0)
            {
                var c = go.GetComponents<Component>();
                if (c.Length == 1 && c[0] == go.transform)
                {
                    return true;
                }
            }
            return false;
        }

        private static string GetBundleLabel(EnvironmentVariantData variant) => $"environments/{variant.Guid}.{BundleExtension}";
        private static string GetGeneratedSceneName(EnvironmentVariantData variant, SceneAsset originalScene)
        {
            if (originalScene == null)
            {
                return string.Empty;
            }
            int index = variant.OriginalScenes.IndexOf(originalScene) + ExtraScenes.Count;
            return GetSceneName(variant, index, originalScene.name);
        }

        public static FilePath GetScenePath(EnvironmentVariantData variant, string sceneName) => GeneratedSceneRoot.Append(GetSceneName(variant, sceneName)).AppendExt(".unity");
        private static string GetSceneName(EnvironmentVariantData variant, string sceneName) => GetSceneName(variant, variant.GeneratedScenes.Count, sceneName);
        private static string GetSceneName(EnvironmentVariantData variant, int index, string sceneName) => $"{variant.Guid}_{sceneName}";

        public class GeneratedSceneMap
        {
            public readonly Dictionary<Type, Predicate<Component>> Filters = new Dictionary<Type, Predicate<Component>>();
            public readonly List<Type> Types;
            public readonly string Name;

            public GeneratedSceneMap(string name, Dictionary<Type, Predicate<Component>> filters, params Type[] types) : this(name, types)
            {
                Filters = filters;
            }

            public GeneratedSceneMap(string name, params Type[] types)
            {
                Name = name;
                Types = types.ToList();
            }
        }
    }
}
