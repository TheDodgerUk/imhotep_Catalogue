using FilePathHelper;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Utils
{
    public static class Editor
    {
        public static SceneView GetSceneView()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                return SceneView.lastActiveSceneView;
            }
            if (SceneView.sceneViews.Count > 0)
            {
                return SceneView.sceneViews[0] as SceneView;
            }
            return EditorWindow.GetWindow<SceneView>();
        }

        public static List<FilePath> GetFiles(string folder, params string[] extensions)
        {
            if (IO.FolderExists(folder) == true)
            {
                var exts = extensions.ToList().Extract(e => $"*.{e}");
                var paths = exts.Extract(e => IO.GetFiles(folder, e)).Flatten();
                return paths.Extract(p => FilePath.FromAbsolute(p));
            }
            return new List<FilePath>();
        }

        public static void CopyFiles(string sourceFolder, string destinationFolder, bool cleanCopy, params string[] extensionsToCopy)
        {
            if (IO.FolderExists(sourceFolder) == true)
            {
                if (cleanCopy == true)
                {
                    IO.DestroyFolder(destinationFolder);
                }

                var exts = extensionsToCopy.ToList().Extract(e => $"*.{e}");
                var sourceFiles = exts.Extract(e => IO.GetFiles(sourceFolder, e)).Flatten();

                using (var progressBar = new ProgressBar("Copying Files", sourceFiles.Count))
                {
                    foreach (var sourceFile in sourceFiles)
                    {
                        if (progressBar.UpdateIncrement(sourceFile) == true)
                        {
                            break;
                        }

                        var sourcePath = FilePath.FromAbsolute(sourceFile);
                        var destinationPath = sourcePath.ChangeRoot(sourceFolder, destinationFolder);

                        IO.ForceFolder(destinationPath.GetFolder().Absolute);
                        string destination = destinationPath.Absolute.DesanitiseSlashes();
                        string source = sourcePath.Absolute.DesanitiseSlashes();
                        File.Copy(source, destination, true);
                    }
                }
            }
        }

        private static List<string> GetFiles(string folder, string searchPattern)
        {
            return Directory.GetFiles(folder, searchPattern, SearchOption.AllDirectories).ToList();
        }

        public static List<Scene> GetLoadedScenes()
        {
            int sceneCount = SceneManager.sceneCount;
            var scenes = new List<Scene>(sceneCount);
            for (int i = 0; i < sceneCount; ++i)
            {
                scenes.Add(SceneManager.GetSceneAt(i));
            }
            return scenes;
        }

        public static bool IsLayerVisible(int layer)
        {
            return BitMask.IsSet(Tools.visibleLayers, 1 << layer);
        }
        public static void ToggleVisibleLayer(int layer, bool state)
        {
            Tools.visibleLayers = BitMask.Toggle(Tools.visibleLayers, 1 << layer, state);
        }
        public static void ToggleVisibleLayer(int layer)
        {
            Tools.visibleLayers = BitMask.Toggle(Tools.visibleLayers, 1 << layer);
        }
    }
}
