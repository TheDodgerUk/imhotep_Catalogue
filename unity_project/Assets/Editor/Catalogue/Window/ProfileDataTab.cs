using EditorTools.Views;
using FilePathHelper;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    [Serializable]
    public class ProfileDataTab : IEditorView
    {
        public string Name => "Data Profiles";

        private ProfileDataExplorerView m_ExplorerView;
        private IDataView m_DataView;
        private object m_LoadedData;

        public ProfileDataTab()
        {
            OnShow();
        }

        public void OnShow()
        {
            var profiles = new Dictionary<string, Type>();
            foreach (var gizmoType in GetProfileTypes())
            {
                var folder = Catalogue.GetProfileFolder(gizmoType);
                if (Utils.IO.FolderExists(folder) == true)
                {
                    var filePaths = Utils.IO.GetFiles(folder, "*.json");
                    filePaths.Sort(Comparisons.Natural);
                    filePaths.ForEach(p => profiles.Add(p, gizmoType));
                }
            }

            var selected = m_ExplorerView?.GetSelected();

            m_ExplorerView = new ProfileDataExplorerView(profiles);
            m_ExplorerView.DataSelected += OnDataSelected;

            OnDataSelected(selected);
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                m_ExplorerView.OnGUI();

                using (new EditorGUILayout.VerticalScope())
                {
                    m_DataView?.OnGUI();

                    if (m_LoadedData != null)
                    {
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Save") == true)
                        {
                            string filePath = m_ExplorerView.GetSelected();
                            var path = FilePath.FromAbsolute(filePath).RemoveExt();
                            Json.JsonNet.WriteToFile(m_LoadedData, path.Absolute, true);
                        }
                    }
                }
            }
        }

        private void OnDataSelected(string filePath)
        {
            m_ExplorerView.SetSelected(filePath);
            m_DataView = GetDataView(filePath);
        }

        private IDataView GetDataView(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) == false)
            {
                if (Utils.IO.FileExists(filePath) == true)
                {
                    var type = m_ExplorerView.GetDataType(filePath);
                    var path = FilePath.FromAbsolute(filePath).RemoveExt();
                    m_LoadedData = Json.JsonNet.ReadFromFile(path.Absolute, type);

                    var viewType = typeof(DataView<>).GetConcreteChildren(type).First();
                    return Activator.CreateInstance(viewType, m_LoadedData) as IDataView;
                }
            }
            return null;
        }

        private static List<Type> GetProfileTypes()
        {
            var profileDataTypes = typeof(ProfileData<>).GetConcreteChildren();
            return profileDataTypes.Extract(t => t.GetGenericBaseType().GetGenericArguments()[0]);
        }
    }
}
