using EditorTools.Views;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class RenderDataView : MetaDataView<RenderMetaData>
    {
        private RenderItemView m_VariantsView;
        private RenderItemView m_PartsView;

        private bool m_ShownBundleWarning = false;
        private CatalogueEntry m_Entry;

        public RenderDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_VariantsView = new RenderItemView("Variants", m_Entry, m_MetaData.Variants);
            m_PartsView = new RenderItemView("Parts", m_Entry, m_MetaData.Parts);

            m_VariantsView.ToggleFoldout(false);
            m_PartsView.ToggleFoldout(false);
            m_Entry = entry;
        }

        protected override void OnMetaDataUpdate()
        {
            m_MetaData.Version = m_Entry.Version;
        }

        protected override void OnMetaDataGUI()
        {
            RenderItemView.DrawRenderItem(m_Entry, m_MetaData.Main, ref m_ShownBundleWarning);
            EditorGUILayout.Space();

            m_VariantsView.OnGUI();
            m_PartsView.OnGUI();
        }

        private class RenderItemView : DefaultListView<RenderItemData>
        {
            protected override bool Spacing => true;

            private readonly CatalogueEntry m_Entry;

            private bool m_ShownBundleWarning = false;

            public RenderItemView(string name, CatalogueEntry entry, List<RenderItemData> list) : base(name, list)
            {
                m_Entry = entry;
            }

            protected override RenderItemData DrawElement(RenderItemData renderItem)
            {
                DrawRenderItem(m_Entry, renderItem, ref m_ShownBundleWarning);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, RenderItemData renderItem, ref bool shownBundleWarning)
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    var assignedPrefab = PrefabManager.LoadPrefab(renderItem);
                    var newPrefab = EditorHelper.ObjectField("Prefab", assignedPrefab);

                    if (newPrefab != assignedPrefab)
                    {
                        renderItem.Guid = newPrefab?.GetAssetGuid();                      
                        renderItem.Prefab = newPrefab?.name;

                        // comment out as messes up         "Bundle": 
                        //renderItem.Bundle = newPrefab?.GetAssetBundleName();
                        //if (newPrefab != null)
                        //{
                        //    if (true == EditorUtility.DisplayDialog("ERROR: Invalid Prefab", $"\"{newPrefab.name}\" must be labelled for an AssetBundle or in a folder labelled for one.", "Make AssetBundle", "Cancel"))
                        //    {
                        //        string assetPath = AssetDatabase.GetAssetPath(newPrefab);

                        //        string bundleName = assetPath;
                        //        Debug.LogError($"bundleName1: {bundleName}");
                        //        bundleName = bundleName.Replace(".prefab", "");
                        //        Debug.LogError($"bundleName2: {bundleName}");
                        //        bundleName += "Ex";
                        //        bundleName = $"{bundleName}.unity3d";
                        //        Debug.LogError($"bundleName3: {bundleName}");

                        //        AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, "");

                        //        bundleName = bundleName.ToLower();
                        //        string searchFor = "assets/artwork/models/catalogue/".ToLower();
                        //        if (bundleName.StartsWith(searchFor, System.StringComparison.InvariantCultureIgnoreCase) == true)
                        //        {
                        //            bundleName = bundleName.Replace(searchFor, "catalogue/");
                        //        }
                        //        Debug.LogError($"bundleName4: {bundleName}");
                        //        renderItem.Bundle = bundleName;
                        //    }
                        //}
                    }

                    // comment out as messes up         "Bundle": 
                    //var assetBundleNames = AssetDatabase.GetAllAssetBundleNames().ToList();
                    //if (string.IsNullOrEmpty(renderItem.Bundle) == false && assetBundleNames.Contains(renderItem.Bundle) == false)
                    //{
                    //    Debug.LogWarning($"This is no asset bundle labelled {renderItem.Bundle}.\n");
                    //    var prefab = newPrefab ?? assignedPrefab;
                    //    if (prefab != null)
                    //    {
                    //        string bundleLabel = newPrefab.GetAssetBundleName();
                    //        Debug.LogWarning($"Replacing with {bundleLabel}\n");
                    //        renderItem.Bundle = bundleLabel;
                    //    }
                    //}

                    DrawPrefabControls(entry, renderItem, newPrefab);
                }
            }

            public static void DrawPrefabControls(CatalogueEntry entry, RenderItemData renderItem, GameObject prefab)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorHelper.IndentSpace();

                    // create prefab(s)
                    if (GUILayout.Button("Create Empty", GUILayout.Width(100)) == true)
                    {
                        const string Message = "There is already a prefab assigned, would you like to create a new one anyway?";
                        if (prefab == null || EditorUtility.DisplayDialog("Prefab Already Assigned", Message, "Yes", "No") == true)
                        {
                            PrefabManager.CreatePrefab(entry, prefab);
                        }
                    }

                    using (new GuiEnabledScope(prefab != null))
                    {
                        if (GUILayout.Button("Apply Selected Model", GUILayout.Width(150)) == true)
                        {
                            PrefabManager.ApplySelectedModelToPrefab(entry, renderItem);
                        }
                        if (GUILayout.Button("Edit", GUILayout.Width(50)) == true)
                        {
                            TestSceneManager.EditPrefabInScene(prefab);
                        }
                        if (GUILayout.Button("View in VR", GUILayout.Width(80)) == true)
                        {
                            TestSceneManager.ViewPrefabInScene(prefab);
                        }

                        if (GUILayout.Button("Check bundle name", GUILayout.Width(150)) == true)
                        {
                            var assignedPrefab = PrefabManager.LoadPrefab(renderItem);
                            var newPrefab = EditorHelper.ObjectField("Prefab", assignedPrefab);

                            string assetPath = AssetDatabase.GetAssetPath(newPrefab);

                            string bundleName = assetPath;
                            Debug.LogError($"bundleName1: {bundleName}");
                            bundleName = bundleName.Replace(".prefab", "");
                            Debug.LogError($"bundleName2: {bundleName}");
                            bundleName = $"{bundleName}.unity3d";
                            Debug.LogError($"bundleName3: {bundleName}");

                            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundleName, "");
                            renderItem.Bundle = bundleName;
                        }
                    }
                }
            }
        }
    }
}
