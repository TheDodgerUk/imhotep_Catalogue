using EditorTools.Bundles;
using FilePathHelper;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorTools.CatalogueViewer
{
    public class CatalogueEntrySearchView : SearchView<CatalogueEntry>
    {
        public event Action SaveCatalogueAll = null;
        public event Action SaveCatalogueLast = null;
        private Catalogue m_Catalogue;
        public CatalogueEntrySearchView(List<CatalogueEntry> entries, Catalogue cat, CatalogueEntry selected) : base(entries, selected)
        {
            m_Catalogue = cat;
        }

        protected override void OnExplorerGUI()
        {
            base.OnExplorerGUI();

            var model = Selection.activeGameObject;
            var importer = model?.GetImporter<ModelImporter>();
            if (GUILayout.Button("ThumbnailGenerator") == true)
            {
                string SceneName = "ThumbnailGenerator";
                var unityFiles = Directory.GetFiles(Application.dataPath, $"{SceneName}.unity", SearchOption.AllDirectories);
                if (unityFiles.Length == 1)
                {
                    var scenePath = FilePath.FromAbsolute(unityFiles[0]);
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
                    {
                        EditorSceneManager.OpenScene(scenePath.Relative, OpenSceneMode.Single);
                        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName));
                    }
                }
            }
            if (GUILayout.Button("Delete Thumbnail") == true)
            {
#if UNITY_EDITOR
                if (true == EditorUtility.DisplayDialog("Thumbnail", $"Do you want to delete the thumbnail: {m_Selected.ShortName}", "Delete", "Cancel"))
                {
                    string folderName = $"{Application.dataPath}/Artwork/CatalogueThumbnails/{m_Selected.Guid}";
                    Directory.Delete(folderName, true);
                }
#endif
            }
            if (GUILayout.Button("Upload item") == true)
            {
#if UNITY_EDITOR
                if (true == EditorUtility.DisplayDialog("Upload item", $"Do you want to upload the item asset bundle?", "Upload", "Cancel"))
                {
                    BundleBuilderWindow.UploadSingleCatItem(m_Selected);
                }
#endif
            }


            using (new GuiEnabledScope(importer != null))
            {
                GUILayout.Label("Select your newly imported model first FBX", EditorStyles.boldLabel);
                if (GUILayout.Button("Add New Model (fbx)") == true)
                {
                    AddSelectedModel(model);
                }
                if (GUILayout.Button("Update Model (fbx)") == true)
                {
                    UpdateModel(model);
                }
            }
            using (new GuiEnabledScope(importer == null && model != null))
            {
                GUILayout.Space(10);
                GUILayout.Label("Select your prefab", EditorStyles.boldLabel);
                if (GUILayout.Button("Add prefab") == true)
                {
                    AddSelectedModel(model);
                }
            }


        }

        private void UpdateModel(GameObject model)
        {
            var entry = GetSelected();
            var renderMetaData = entry.GetMetaData<RenderMetaData>();
            if (renderMetaData == null)
            {
                entry.AddMetaData(MetaDataType.Render);
                renderMetaData = entry.GetMetaData<RenderMetaData>();
            }

            var prefab = PrefabManager.LoadPrefab(renderMetaData.Main);
            prefab = PrefabManager.CreatePrefab(entry, prefab);

            PrefabManager.ApplyModelToPrefab(entry, renderMetaData.Main, model);

            SaveCatalogueLast?.Invoke();
        }

        private void AddSelectedModel(GameObject model)
        {
#if UNITY_EDITOR
            if(model.name.CaseInsensitiveContains("-") ==true)
            {
                EditorUtility.DisplayDialog("Model Error", "The system does not like model names which contain '-', name must be updated.", "OK");
                return;
            }
#endif

            string name = model.name.Strip("_LOD0");
            using (var progressBar = new ProgressBar($"Creating Entry for {name}", 4))
            {
                // create catalogue entry
                progressBar.UpdateIncrement("Generating Catalogue Entry");

                var entry = new CatalogueEntry();
                entry.FullName.Text = name.Replace("_", " ");
                entry.ShortName.Text = name.SplitList("_").Last();

                entry.Attributes.Add(MetaAttribute.Moveable);
                entry.AddMetaData(MetaDataType.Render);

                progressBar.UpdateIncrement("Generating Prefab");
                CreateColliders.ChangeChildrenMeshsToColliders(model);
                var renderItem = PrefabManager.CreateNewPrefabFromModel(entry, model);
                if (renderItem != null)
                {
                    entry.GetMetaData<RenderMetaData>().Main = renderItem;
                    progressBar.UpdateIncrement("Saving Catalogue");

                    AddNew(entry);
                    
                    SaveCatalogueLast?.Invoke();
                }
                else
                {
                    EditorUtility.DisplayDialog("Item Error", "It needs to be in a subdirectory from Catalogue\n Artwork/Models/Catalogue/..." +
                        "Examples:\n" +
                        "Artwork/Models/Catalogue/Behringer\n" +
                        "Artwork/Models/Catalogue/Warehouse ", "Ok");
                }
            }
        }
    }
}