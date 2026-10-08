using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class ProfileDataExplorerView : ExplorerView<string>
    {
        private readonly Dictionary<string, Type> m_PathsToType;
        private readonly HashSet<string> m_ParentEntries;
        private readonly HashSet<Type> m_FoldoutEntries;

        public ProfileDataExplorerView(Dictionary<string, Type> profilePaths) : base(null, profilePaths.Keys.ToList())
        {
            ExplorerViewSize.LoadStatics();
            m_PathsToType = profilePaths;
            DrawModifyControls = false;
            m_DrawDisplay = false;

            m_ParentEntries = new HashSet<string>();
            m_FoldoutEntries = new HashSet<Type>();
            var addedParents = new HashSet<Type>();
            foreach (string path in GetList())
            {
                var type = m_PathsToType[path];
                if (addedParents.Contains(type) == false)
                {
                    m_ParentEntries.Add(path);
                    addedParents.Add(type);
                }
            }
        }

        protected override void OnExplorerGUI() { }

        protected override string DrawElement(string filePath)
        {
            bool foldout = GetFoldout(filePath);
            foldout = DrawTypeFoldout(filePath, foldout);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (foldout == true)
                {
                    GUILayout.Space(16);

                    string fileName = Utils.IO.GetFileName(filePath);
                    var style = GetButtonStyle(IsSelected(filePath));
                    var width = GUILayout.Width(ExplorerViewSize.Size - (26 + 16));
                    if (GUILayout.Button(fileName, style, width) == true)
                    {
                        OnDataSelected(filePath);
                    }
                }
            }
            return filePath;
        }

        public Type GetDataType(string filePath) => m_PathsToType.Get(filePath);

        private bool GetFoldout(string filePath)
        {
            return m_FoldoutEntries.Contains(GetDataType(filePath));
        }

        private bool DrawTypeFoldout(string filePath, bool foldout)
        {
            if (m_ParentEntries.Contains(filePath) == true)
            {
                var type = GetDataType(filePath);
                foldout = EditorGUILayout.Foldout(foldout, type.Name, true);

                if (foldout == true)
                {
                    m_FoldoutEntries.Add(type);
                }
                else
                {
                    m_FoldoutEntries.Remove(type);
                }
            }

            return foldout;
        }

        protected override void OnAdd() { }

        private static List<Type> GetGizmoTypes() => typeof(ProfileData<>).GetConcreteChildren();
    }
}