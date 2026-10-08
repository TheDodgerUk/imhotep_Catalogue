using FilePathHelper;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorTools.CatalogueViewer
{
    public static class TestSceneManager
    {
        private const string SceneName = "PrefabTestScene";
        private const string SpawnPoint = "SpawnPoint";

        public static void ViewPrefabInScene(GameObject prefab)
        {
            EditPrefabInScene(prefab);

            EditorApplication.isPlaying = true;
        }

        public static GameObject EditPrefabInScene(GameObject prefab)
        {
            if (LoadTestScene() == false)
            {
                return null;
            }

            var spawnPoint = FindSpawnPoint();
            ClearSpawnPoint(spawnPoint);

            var clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            clone.transform.SetParent(spawnPoint.transform, false);
            clone.hideFlags = HideFlags.DontSave;

            var sceneView = Utils.Editor.GetSceneView();
            sceneView.Focus();
            sceneView.LookAt(spawnPoint.position, sceneView.rotation, 2f);
            EditorHelper.SelectAndPingObject(clone);
            return clone;
        }

        private static bool LoadTestScene()
        {
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; ++i)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == SceneName)
                {
                    SceneManager.SetActiveScene(scene);
                    CatalogueWindow.ShowCatalogueEditor().ReLoadCatalogueEntryTab();
                    return true;
                }
            }

            var unityFiles = Directory.GetFiles(Application.dataPath, $"{SceneName}.unity", SearchOption.AllDirectories);
            if (unityFiles.Length == 1)
            {
                var scenePath = FilePath.FromAbsolute(unityFiles[0]);
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
                {
                    EditorSceneManager.OpenScene(scenePath.Relative, OpenSceneMode.Single);
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName));
                    CatalogueWindow.ShowCatalogueEditor().ReLoadCatalogueEntryTab();
                    return true;
                }
            }
            else
            {
                EditorUtility.DisplayDialog(SceneName, $"Could not find scene \"{SceneName}\" in project", "Ok");
            }

            return false;
        }

        private static Transform FindSpawnPoint()
        {
            var rootObjects = SceneManager.GetActiveScene().GetRootGameObjects().ToList();
            return rootObjects.Find(go => go.name == SpawnPoint).transform;
        }

        private static void ClearSpawnPoint(Transform spawnPoint)
        {
            var children = spawnPoint.GetDirectChildren();
            for (int i = 0; i < children.Count; ++i)
            {
                children[i].gameObject.DestroyObjectImmediate();
            }
        }
    }
}
