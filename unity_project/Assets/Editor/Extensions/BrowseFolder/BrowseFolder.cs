using FilePathHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    [Serializable]
    public class BrowseFolder
    {
        public bool DriveAvailable { get; private set; }

        private readonly string m_DefaultFolderName;
        private readonly List<string> m_SubFolders;
        private readonly string m_FolderType;
        private readonly string m_Extension;
        private readonly string m_Key;

        private string m_Path;

        public BrowseFolder(string folderType, string key, string defaultPath) : this(folderType, key, defaultPath, null) { }
        public BrowseFolder(string folderType, string key, string defaultPath, string[] subFolders) : this(folderType, key, defaultPath, null, string.Empty, subFolders) { }
        public BrowseFolder(string folderType, string key, string defaultPath, string extension, string folderName, string[] subFolders)
        {
            m_Path = GetSavedPath(key, defaultPath);
            m_DefaultFolderName = folderName;
            m_SubFolders = subFolders.ToList();
            m_FolderType = folderType;
            m_Extension = extension;
            m_Key = key;

            CheckDriveExists();
        }

        public void UpdateSavedPath() => PlayerPrefs.SetString(m_Key, m_Path);

        public void CheckDriveExists() => DriveAvailable = Utils.IO.DriveExists(m_Path);
        public bool FoldersExist()
        {
            return GetAllPaths().TrueForAll(p => Utils.IO.FolderExists(p.Absolute));
        }

        public List<FilePath> GetAllPaths()
        {
            var path = GetCurrentPath();
            if (m_SubFolders == null)
            {
                return path.CreateList();
            }

            return m_SubFolders.Extract(path.Append);
        }

        public FilePath GetCurrentPath()
        {
            var fullPath = FilePath.FromAbsolute(m_Path);
            if (string.IsNullOrEmpty(m_Extension) == false)
            {
                fullPath = fullPath.Append(m_Extension);
            }
            return fullPath;
        }

        public void DrawFolderPath()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                string path = EditorGUILayout.TextField($"{m_FolderType} Folder", m_Path);
                if (string.IsNullOrEmpty(m_Extension) == false)
                {
                    EditorGUILayout.LabelField("/" + m_Extension);
                }
                path = BtnBrowse(m_FolderType, path, m_DefaultFolderName);

                if (m_Path != path)
                {
                    m_Path = path;
                    UpdateSavedPath();
                }
            }
        }

        public delegate bool ButtonHandler(string buttonText);
        public delegate void ControlsHandler(ButtonHandler button);
        public delegate void PerPathControlsHandler(FilePath path);

        public void DrawControls(bool isEnabled, PerPathControlsHandler perPathControls, ControlsHandler additionalControls)
        {
            DrawFolderPathLabels(perPathControls);
            DrawButtons(isEnabled, additionalControls);
        }

        private void DrawFolderPathLabels(PerPathControlsHandler perPathControls)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                string label = $"{m_FolderType} Folder";
                var width = GUILayout.Width(EditorGUIUtility.labelWidth);
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel, width);

                using (new EditorGUILayout.HorizontalScope())
                {
                    foreach (var path in GetAllPaths())
                    {
                        using (new EditorGUILayout.VerticalScope())
                        {
                            EditorGUILayout.LabelField(path.Absolute);
                            perPathControls?.Invoke(path);
                        }
                    }
                }
            }
        }

        private void DrawButtons(bool isEnabled, ControlsHandler additionalControls)
        {
            const float Width = 200;
            var btnWidth = GUILayout.Width(Width);

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GuiEnabledScope(isEnabled && FoldersExist()))
                {
                    if (GUILayout.Button("Show Folder", btnWidth) == true)
                    {
                        foreach (var path in GetAllPaths())
                        {
                            OnShowFolder(path);
                        }
                    }
                    if (GUILayout.Button("MaterialThumbnailGenerator") == true)
                    {
                        string SceneName = "MaterialThumbnailGenerator";
                        var unityFiles = Directory.GetFiles(Application.dataPath, $"{SceneName}.unity", SearchOption.AllDirectories);
                        if (unityFiles.Length == 1)
                        {
                            var scenePath = FilePath.FromAbsolute(unityFiles[0]);
                            if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
                            {
                                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath.Relative, UnityEditor.SceneManagement.OpenSceneMode.Single);
                                UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(SceneName));
                            }
                        }
                    }

                    if (GUILayout.Button("ThumbnailGenerator") == true)
                    {
                        string SceneName = "ThumbnailGenerator";
                        var unityFiles = Directory.GetFiles(Application.dataPath, $"{SceneName}.unity", SearchOption.AllDirectories);
                        if (unityFiles.Length == 1)
                        {
                            var scenePath = FilePath.FromAbsolute(unityFiles[0]);
                            if (UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
                            {
                                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath.Relative, UnityEditor.SceneManagement.OpenSceneMode.Single);
                                UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(SceneName));
                            }
                        }
                    }



                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    additionalControls?.Invoke(buttonText => GUILayout.Button(buttonText, btnWidth));
                }

            }
        }

        protected virtual void OnShowFolder(FilePath path)
        {
            Utils.IO.OpenFolder(path.Absolute);
        }

        protected virtual void OnCleanFolder(FilePath path)
        {
            Utils.IO.DestroyFolder(path.Absolute);
            FolderCleaner.CleanEmptyFoldersIn(path.Absolute, true);
        }

        private static string BtnBrowse(string type, string currentFolder, string defaultFolderName)
        {
            if (Utils.Gui.Btn("...", true, GUILayout.Width(24)) == true)
            {
                string parent = currentFolder;
                if (string.IsNullOrEmpty(defaultFolderName) == false)
                {
                    parent = FilePath.FromAbsolute(currentFolder).GetFolder().Absolute;
                }

                string title = $"Find your {type} folder";
                string result = EditorUtility.OpenFolderPanel(title, parent, defaultFolderName);
                if (string.IsNullOrEmpty(result) == false)
                {
                    EditorHelper.UnfocusControls();
                    return result;
                }
            }
            return currentFolder;
        }

        private static string GetSavedPath(string key, string defaultPath)
        {
            if (string.IsNullOrEmpty(key) == true)
            {
                return defaultPath;
            }
            return PlayerPrefs.GetString(key, defaultPath);
        }
    }
}