using FilePathHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.AI;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EditorTools.Environments.EnvironmentVariantDataView;

namespace EditorTools.Environments
{
    public class EnvironmentSceneManager
    {
        public static SceneAsset LoadScene(string path) => AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
        public static SceneAsset FindScene(string guid) => LoadScene(AssetDatabase.GUIDToAssetPath(guid));

        public static bool SaveOpenScenes()
        {
            EditorSceneManager.MarkAllScenesDirty();
            return EditorSceneManager.SaveOpenScenes();
        }

        public static List<SceneAsset> LoadScenes(EnvironmentData data)
        {
            var rootPath = EnvironmentSceneGenerator.GeneratedSceneRoot;
            return data.Chunks.Extract(c => LoadScene(rootPath.Append($"{c}.unity").Relative));
        }

        public static List<Scene> OpenScenes(List<SceneAsset> sceneAssets)
        {
            if (sceneAssets.Contains(null) == true)
            {
                return null;
            }
            var scenes = GetOpenScenes(sceneAssets);
            if (scenes != null)
            {
                return scenes;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
            {
                using (var progress = new ProgressBar("Loading scenes...", sceneAssets.Count))
                {
                    scenes = new List<Scene>();
                    var mode = OpenSceneMode.Single;
                    foreach (var asset in sceneAssets)
                    {
                        progress.UpdateIncrement(asset.name);
                        string path = asset.GetAssetPath().Relative;
                        scenes.Add(EditorSceneManager.OpenScene(path, mode));
                        mode = OpenSceneMode.Additive;
                    }
                }
            }
            return scenes;
        }

        private static List<Scene> GetOpenScenes(List<SceneAsset> sceneAssets)
        {
            if (SceneManager.sceneCount != sceneAssets.Count)
            {
                return null;
            }

            var scenes = new List<Scene>();
            for (int i = 0; i < sceneAssets.Count; ++i)
            {
                var scene = SceneManager.GetSceneByName(sceneAssets[i].name);
                if (scene.IsValid() == false)
                {
                    return null;
                }
                scenes.Add(scene);
            }
            return scenes;
        }

        public static bool IsBaking() => (Lightmapping.isRunning || NavMeshBuilder.isRunning);


        public static void StaticCheck(List<SceneAsset> sceneAssets)
        {
            bool batch = UnityEditorInternal.InternalEditorUtility.inBatchMode;

            GameObject[] items = GameObject.FindObjectsOfType<GameObject>(); //returns Object[]
            foreach (var item in items)
            {
                if(item.name.StartsWith("Static", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    item.SetStaticRecursively();
                }
            }
            SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        public static void BakeLighting(List<SceneAsset> sceneAssets)
        {
            bool batch = UnityEditorInternal.InternalEditorUtility.inBatchMode;

            // open relevant scenes
            var loadedScenes = OpenScenes(sceneAssets);

            // force disable navmesh renderers

            SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // bake lighting
            if (batch == true)
            {
                Lightmapping.Bake();
            }
            else
            {
                Lightmapping.BakeAsync();
                EditorCoroutineUtility.StartCoroutineOwnerless(BakeCompleted());
            }

            SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static IEnumerator BakeCompleted()
        {
            yield return null;
            while (true == Lightmapping.isRunning)
            {
                yield return null;
            }
            yield return new WaitForSeconds(2);

            SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Bake Lighting", "Bake Lighting has been completed", "Ok");
        }


        public static void BakeLightingV(EnvironmentVariantData variant, List<SceneAsset> sceneAssets)
        {
            List<int> distict = new List<int>();
            foreach(var item in variant.m_BakeQuality)
            {
                distict.Add(item.m_Index);
            }
            var distinctIndexs = distict.Distinct().ToList();
            bool batch = UnityEditorInternal.InternalEditorUtility.inBatchMode;

            EditorCoroutineUtility.StartCoroutineOwnerless(BakeAll(sceneAssets, distinctIndexs, variant.m_BakeQuality));
        }


        private static IEnumerator BakeAll(List<SceneAsset> sceneAssets, List<int> distinctIndexs, List<EnvironmentData.BakeQuality> bakeQuality)
        {
            yield return null;

            foreach (var item in distinctIndexs)
            {
                Debug.Log($"distinctIndexs: {item}");
            }

            yield return null;
            yield return null;
            yield return null;

            foreach (var item in distinctIndexs)
            {
                List<SceneAsset> groupedScenes = new List<SceneAsset>();

                var lastSetting = bakeQuality.FirstOrDefault(e => e.m_Index == item);
                SceneAsset lightSettingsScene = sceneAssets.FindLast(e => e.name.CaseInsensitiveContains("LightBake-" + lastSetting.m_QualityEnum.ToString()));
                if (lightSettingsScene != null)
                {
                    groupedScenes.Add(lightSettingsScene);
                }
                groupedScenes.Add(sceneAssets[0]);    // generated lights                


                for (int i = 0; i < bakeQuality.Count; i++)
                {
                    if(bakeQuality[i].m_IsBaked == true && bakeQuality[i].m_Index == item)
                    {
                        groupedScenes.Add(sceneAssets[i]);
                    }
                }

                foreach (var itemInGroup in groupedScenes)
                {
                    Debug.Log($"index: {item},  includes: {itemInGroup.name}");
                }

                var loadedScenes = OpenScenes(groupedScenes);

                //Set the settings scene as the active one to apply settings to all others
                SceneManager.SetActiveScene(loadedScenes[0]);

                Lightmapping.BakeAsync();

                EditorCoroutineUtility.StartCoroutineOwnerless(BakeCompleted());
                Debug.Log($"Completed index: {item}");
            }

            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var Variant = environmentData.GetCurrentVariant();

            string outputString = $"{ EnvironmentSceneGenerator.GeneratedSceneRoot.Absolute }/{ Variant.Guid}";
            Debug.Log(outputString);
            EditorUtility.DisplayDialog("Bake Lighting", $"Bake Lighting has been completed \n\n{outputString}", "Ok");
        }

        private static IEnumerator BakeCompleted(Action callback)
        {
            yield return null;
            while (true == Lightmapping.isRunning)
            {
                yield return null;
            }
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var Variant = environmentData.GetCurrentVariant();
            EditorUtility.DisplayDialog("Bake Lighting", $"Bake Lighting has been completed \n\n{EnvironmentSceneGenerator.GeneratedSceneRoot.Absolute}/{Variant.Guid}", "Ok");
            callback?.Invoke();
        }

        public static void BakeNavMesh(List<SceneAsset> sceneAssets)
        {
            // open relevant scenes
            var loadedScenes = OpenScenes(sceneAssets);

            // force enable navmesh renderers

            // build navmesh
            NavMeshBuilder.BuildNavMesh();

            // disable navmesh renderers

            // save scenes
            SaveOpenScenes();
            List<GameObject> gameObjectList = GetEnvironmentGameObjects(loadedScenes);

            var flag = StaticEditorFlags.NavigationStatic;
            gameObjectList.RemoveAll(g => !g.gameObject.HasStaticFlag(flag));
            Selection.objects = gameObjectList.ToArray();
            if (gameObjectList.Count == 0 )
            {
                EditorUtility.DisplayDialog("Navmesh generation failed.",
                    "One of the following is causing this issue: \n - There are no Collider components found within generated scenes \n - Navigation -> Navigation static is not enabled for your floor \n - The floor object " +
                    " is not on layer: Environment \n - The floor is not set to Static: Navigation Mesh \n \n You not be able to teleport.", "Ok");
            }

        }

        public static void BakeOcclusion(List<SceneAsset> sceneAssets)
        {
            // open relevant scenes
            var loadedScenes = OpenScenes(sceneAssets);

            // force disable navmesh renderers
            foreach (var item in loadedScenes)
            {
                EditorSceneManager.SetActiveScene(item);
            }
            bool batch = UnityEditorInternal.InternalEditorUtility.inBatchMode;

            foreach (var item in loadedScenes)
            {
                EditorSceneManager.SetActiveScene(item);
                StaticOcclusionCulling.smallestOccluder = 1f;
                StaticOcclusionCulling.Compute();
                // save scenes
                SaveOpenScenes();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorApplication.Beep();
            EditorUtility.DisplayDialog("BakeOcclusion", $"BakeOcclusion Completed\n smallestOccluder :{StaticOcclusionCulling.smallestOccluder}\n smallestHole: {StaticOcclusionCulling.smallestHole}", "Ok");
        }


        public static List<GameObject> GetEnvironmentGameObjects(List<Scene> scenes)
        {
            // get renderers in scenes
            var roots = scenes.Extract(s => s.GetRootGameObjects()).Flatten();
            var environmentColliders = roots.Extract(go => go.GetComponentsInChildren<Collider>(true)).Flatten();

            var flag = StaticEditorFlags.NavigationStatic;
            environmentColliders.RemoveAll(r => !r.IsEnvironment() && !r.gameObject.HasStaticFlag(flag));

            // convert to list GameObjects
            List<GameObject> gameObjectList = new List<GameObject>();
            foreach (var item in environmentColliders)
            {
                gameObjectList.Add(item.gameObject);
            }
            return gameObjectList;
        }

        public static void ToggleNavigationRenderers(List<Renderer> renderers, bool state)
        {
            foreach (var renderer in renderers)
            {
                Undo.RecordObject(renderer.gameObject, "Toggle NavMesh Renderers");
                Undo.RecordObject(renderer, "Toggle NavMesh Renderers");
                renderer.SetActive(state);
                renderer.enabled = state;
            }
        }

        public static void CleanUnusedEnvironments(List<EnvironmentSceneData> environments)
        {
            if (false == Utils.IO.FolderExists(EnvironmentSceneGenerator.GeneratedSceneRoot.Absolute))
            {
                //Dont have a generated scene folder... cannot get all environments
                return;
            }

            var generatedScenes = EnvironmentSceneGenerator.GetAllGeneratedScenes();
            using (var progress = new ProgressBar("Checking scene files", generatedScenes.Count))
            {
                foreach (string scenePath in generatedScenes)
                {
                    var filePath = FilePath.FromAbsolute(scenePath);
                    progress.UpdateIncrement($"Checking {filePath.FileName}");

                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(filePath.Relative);
                    if (environments.Exists(e => e.Variants.Exists(v => v.GeneratedScenes.Contains(sceneAsset))) == false)
                    {
                        progress.UpdateInfo($"Deleting file {filePath.FileName}");
                        AssetDatabase.MoveAssetToTrash(filePath.Relative);

                        var folder = filePath.GetFolder();
                        var folderFiles = Utils.IO.GetAllFiles(folder.Absolute, true);
                        folderFiles.RemoveAll(f => !f.EndsWith(".unity"));

                        if (folderFiles.Count == 0)
                        {
                            progress.UpdateInfo($"Deleting folder {folder.FileName}");
                            Utils.IO.DestroyFolder(folder.Absolute);
                        }
                    }
                }
            }

            FolderCleaner.CleanEmptyFoldersIn(EnvironmentSceneGenerator.GeneratedSceneRoot.Absolute);
        }
    }
}