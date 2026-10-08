using EditorTools.Views;
using FilePathHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class CatalogueEntryView
    {
        private EnumListView<MetaAttribute> m_MetaAttributeView;
        private EnumListView<MetaDataType> m_MetaDataView;
        private List<IMetaDataView> m_MetaDataViews;
        private Vector2 m_ScrollPos;


        private SubItemView m_SubItemView;
        private Vector2 m_ScrollPosSubItems;

        private CatalogueEntry m_Selected;
        private Catalogue m_Catalogue;
        private Type m_AddedMetaDataType;

        private Vector2 m_VariantScrollPos;
        private Vector2 m_PartScrollPos;

        private int m_EnvironmentIndex = 0;
        string[] EnvironmentPopupOptions = new string[] { "Data", "Not", "Found" };

        private int CatalogueFilterIndex = 0;
        string[] CatalogueFilterOptions = new string[] { "Data", "Not", "Found" };

        List<GameObject> m_TempGameObjects = new List<GameObject>();
        GameObject m_RootGameObject = null;

        public CatalogueEntryView(Catalogue catalogue, CatalogueEntry selected)
        {
            string environmentDataStructure = $"{GlobalConsts.BUNDLE_STREAMING_STRUCTURE_FILE}";
            GlobalConsts.EnvironmentDataStructure structure = Json.JsonNet.ReadFromFile<GlobalConsts.EnvironmentDataStructure>(environmentDataStructure);
            if (null != structure)
            {
                EnvironmentPopupOptions = structure.EnvironmentPath.Select(e => e.Path).ToArray();
            }
            LoadFilters();

            m_Catalogue = catalogue;
            SetSelected(selected);
        }

        private void LoadFilters()
        {
            string filterFolder = Catalogue.CatalogueFiltersFolder;
            string[] filterFiles = System.IO.Directory.GetFiles(filterFolder, "*json");
            List<string> filterList = new List<string>();
            for (int i = 0; i < filterFiles.Length; i++)
            {
                string fileName = Utils.IO.GetFileName(filterFiles[i]);
                var data = Json.JsonNet.ReadFromFile<FilterData>($"{filterFolder}/{fileName}");

                //if(fileName == "Brands")
                //{
                //    SaveBrandFile(data);
                //}

                foreach (string filter in data.Filters)
                {
                    filterList.Add($"{fileName}/{filter}");
                }
            }

            CatalogueFilterOptions = filterList.ToArray();
        }

        //private void SaveBrandFile(FilterData data)
        //{
        //    BrandData brandData = new BrandData();
        //    brandData.BrandNames = data.Filters;
        //    string sBrandFolder = Application.streamingAssetsPath + "/data/Brands";
        //    Utils.IO.ForceFolder(sBrandFolder);
        //    Json.Unity.WriteToFile(brandData, $"{sBrandFolder}/BrandData", true);
        //}

        public void OnShow()
        {
            RefreshMetaDataViewsFromSelected();
        }

        public void SetSelected(CatalogueEntry entry)
        {
            m_Selected = entry;

            var m_RenderMetaData = m_Selected.GetMetaData<RenderMetaData>();
            m_SubItemView = new SubItemView("SubItem", entry, m_Selected.m_SubItems);
            if (m_RenderMetaData == null || m_RenderMetaData.Main == null)
            {
                return;
            }

            m_RootGameObject = PrefabManager.LoadPrefab(m_RenderMetaData.Main);

            foreach (var item in m_Selected.m_SubItems)
            {

                item.m_GameObject = MetaDataView<GameObject>.StringToGameObject(item.m_ObjectName, m_RootGameObject);
            }
            RefreshMetaDataViewsFromSelected();

            m_MetaAttributeView = new EnumListView<MetaAttribute>("Meta Attributes", entry.Attributes);

            m_MetaDataView = new EnumListView<MetaDataType>("MetaData", entry.GetMetaDataTypes());
            m_MetaDataView.RemoveFromList(MetaDataType.NOTHING);

            m_MetaDataView.ListModified += RefreshMetaDataViewsFromSelected;
            m_MetaDataView.ItemModified += (originalValue, newValue) =>
            {
                entry.MetaData.Remove(originalValue);
                var metaData = entry.AddMetaData(newValue);
                m_AddedMetaDataType = metaData.GetType();
            };
            m_MetaDataView.AddItem += metaType =>
            {
                var metaData = entry.AddMetaData(metaType);
                EditorUtility.DisplayDialog("MetaData Added", $"{metaType}", "Ok");
                m_AddedMetaDataType = metaData.GetType();
                return entry.GetMetaDataTypes();
            };
            m_MetaDataView.RemoveItem += metaType =>
            {
                entry.MetaData.Remove(metaType);
                return entry.GetMetaDataTypes();
            };

            UpdateLinkage();
        }

        private void RefreshMetaDataViewsFromSelected()
        {
            var dataViews = m_MetaDataViews.Clone() ?? new List<IMetaDataView>();

            var viewTypes = GetInterfaceImplementors<IMetaDataView>();
            var constructorArgs = new object[] { m_Catalogue, m_Selected, dataViews };

            var metaData = m_Selected.MetaData.Values.ToList();
            m_MetaDataViews = new List<IMetaDataView>(metaData.Count);

            foreach (var data in metaData)
            {
                // get meta data view type
                var dataType = data.GetType();
                var tuple = viewTypes.FirstOrDefault(t => t.B == dataType);
                if (tuple != null)
                {
                    // create a view for this meta data
                    var view = Activator.CreateInstance(tuple.A, constructorArgs);
                    var metaDataView = view as IMetaDataView;
                    if (m_AddedMetaDataType == tuple.B)
                    {
                        metaDataView.ToggleFoldout(true);
                    }
                    m_MetaDataViews.Add(metaDataView);
                }
            }
            m_AddedMetaDataType = null;
        }

        private void DrawFilters()
        {
            //
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Catalogue Filters");

                CatalogueFilterIndex = EditorGUILayout.Popup(CatalogueFilterIndex, CatalogueFilterOptions);

                if (GUILayout.Button("Add Filter") == true)
                {
                    m_Selected.CatalogueFilters.Add(CatalogueFilterOptions[CatalogueFilterIndex]);
                }

                for (int i = 0; i < m_Selected.CatalogueFilters.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"filter: {i}", m_Selected.CatalogueFilters[i]);
                    if (true == GUILayout.Button("Remove Filter"))
                    {
                        m_Selected.CatalogueFilters.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }


        private void UpdateLinkage()
        {
            var m_RenderMetaData = m_Selected.GetMetaData<RenderMetaData>();
            if (m_RenderMetaData != null)
            {
                m_RootGameObject = PrefabManager.LoadPrefab(m_RenderMetaData.Main);

                m_TempGameObjects.Clear();
                for (int i = 0; i < m_Selected.m_SubItems.Count; i++)
                {
                    var item = MetaDataView<GameObject>.StringToGameObject(m_Selected.m_SubItems[i].m_ObjectName, m_RootGameObject);
                    m_TempGameObjects.Add(item);

                }
            }
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (true == GUILayout.Button("Add Item ?", GUILayout.Width(100)))
            {
                Application.OpenURL("https://dev.azure.com/MusicTribe/MI_R00398/_wiki/wikis/MI_R00398/150/Catalogue-Editor");
            }
            if (true == GUILayout.Button("Add Furniture ?", GUILayout.Width(100)))
            {
                Application.OpenURL("https://dev.azure.com/MusicTribe/MI_R00398/_wiki/wikis/MI_R00398/176/Add-Furniture");
            }
            EditorGUILayout.EndHorizontal();


            Utils.View.DrawGuid(m_Selected.Guid, () =>
            {
                if (GUILayout.Button("Select Folder", GUILayout.Width(100)) == true)
                {
                    string entryPath = Catalogue.GetEntryPath(m_Selected.Guid);
                    var path = FilePath.FromAbsolute($"{entryPath}.json");
                    EditorHelper.SelectAndPingObject(path.Relative);
                }

                if (true == GUILayout.Button("Open Generated file", GUILayout.Width(150)))
                {
                    string path = Catalogue.GetEntryPath(m_Selected.Guid);
                    System.Diagnostics.Process.Start($"{path}.json");
                }

                if (true == GUILayout.Button("Guid and Name", GUILayout.Width(150)))
                {
                    string path = Catalogue.GetEntryPath(m_Selected.Guid);
                    EditorGUIUtility.systemCopyBuffer = $"{m_Selected.FullName}: {m_Selected.Guid}";
                }

            });

            // name and prefab type
            m_Selected.FullName.Text = EditorGUILayout.TextField("Full Name", m_Selected.FullName.Text);
            m_Selected.ShortName.Text = EditorGUILayout.TextField("Short Name", m_Selected.ShortName.Text);
            m_Selected.ReferenceNumber = EditorGUILayout.TextField("ReferenceNumber", m_Selected.ReferenceNumber);
            m_Selected.Version = EditorGUILayout.IntField("Version", m_Selected.Version);
            m_Selected.HasVideo = EditorGUILayout.Toggle("Create Video", m_Selected.HasVideo);
            m_Selected.IncludeInUnitTests = EditorGUILayout.Toggle("Include Unit Test", m_Selected.IncludeInUnitTests);

            m_Selected.FlipThumbnail = EditorGUILayout.Toggle("Flip Thumbnail", m_Selected.FlipThumbnail);


            if (GUILayout.Button("View Item", GUILayout.Width(200)) == true)
            {
                TestSceneManager.EditPrefabInScene(m_RootGameObject);
            }


            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPosSubItems, GUIStyle.none, GUI.skin.verticalScrollbar))
                {
                    m_SubItemView.OnGUI();
                    m_ScrollPosSubItems = s.scrollPosition;
                }
            }
            //otNames.OnGUI();
            //////////m_Selected.m_SubItemsCount = EditorGUILayout.IntField("m_SubItemsCount", m_Selected.m_SubItemsCount);
            //////////m_TempGameObjects.Resize(m_Selected.m_SubItemsCount);
            //////////m_Selected.m_SubItems.Resize(m_Selected.m_SubItemsCount);

            //////////for (int i = 0; i < m_Selected.m_SubItemsCount; i++)
            //////////{
            //////////    if(m_Selected.m_SubItems[i] == null)
            //////////    {
            //////////        m_Selected.m_SubItems[i] = new CatalogueEntry.SubItems(); 
            //////////    }
            //////////    GameObject temp = m_TempGameObjects[i];
            //////////    MetaDataView<GameObject>.GameObjectToString("Screen GameObject", ref temp, ref m_Selected.m_SubItems[i].m_ObjectName);
            //////////    m_TempGameObjects[i] = temp;
            //////////    m_Selected.m_SubItems[i].m_GUID = EditorGUILayout.TextField($"Guid", m_Selected.m_SubItems[i].m_GUID);
            //////////    EditorGUILayout.Space();
            //////////}

            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                for (int i = 0; i < m_Selected.EnvironmentsAvailable.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"env: {i}", m_Selected.EnvironmentsAvailable[i]);
                    if (true == GUILayout.Button("Remove Environment"))
                    {
                        m_Selected.EnvironmentsAvailable.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                //Dropdown menu for all environments and add button to add to current environment
                EditorGUILayout.BeginHorizontal();
                m_EnvironmentIndex = EditorGUILayout.Popup(m_EnvironmentIndex, EnvironmentPopupOptions);
                if (true == GUILayout.Button("Add Environment"))
                {
                    m_Selected.EnvironmentsAvailable.Add(EnvironmentPopupOptions[m_EnvironmentIndex]);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            DrawFilters();

            EditorGUILayout.Space();

            // internal & external meta data 
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar))
                {
                    m_MetaAttributeView.OnGUI();
                    m_MetaDataView.OnGUI();

                    EditorGUILayout.Space();

                    // draw editors for internal meta data
                    foreach (var metaDataView in m_MetaDataViews)
                    {
                        metaDataView.OnGUI();
                        EditorGUILayout.Space();
                    }

                    // draw preview of default prefab
                    var renderMetaData = m_Selected.GetMetaData<RenderMetaData>();
                    if (renderMetaData != null)
                    {
                        var width = GUILayout.Width(128);
                        var height = GUILayout.Height(128);
                        var scrollHeight = GUILayout.Height(128 + EditorGUIUtility.singleLineHeight + GUI.skin.scrollView.lineHeight + GUI.skin.horizontalScrollbar.lineHeight);

                        // main + variants
                        using (var vs = new EditorGUILayout.ScrollViewScope(m_VariantScrollPos, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUIStyle.none, scrollHeight))
                        {
                            EditorGUILayout.LabelField("Variants", EditorStyles.boldLabel);
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                DrawAssetPreview(renderMetaData.Main, width, height);
                                foreach (var variant in renderMetaData.Variants)
                                {
                                    DrawAssetPreview(variant, width, height);
                                }
                                DrawBtnAdd(renderMetaData.Variants, width, height);
                            }
                            vs.handleScrollWheel = false;
                            m_VariantScrollPos = vs.scrollPosition;
                        }

                        // parts
                        using (var ps = new EditorGUILayout.ScrollViewScope(m_PartScrollPos, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUIStyle.none, scrollHeight))
                        {
                            EditorGUILayout.LabelField("Parts", EditorStyles.boldLabel);
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                foreach (var part in renderMetaData.Parts)
                                {
                                    DrawAssetPreview(part, width, height);
                                }
                                DrawBtnAdd(renderMetaData.Parts, width, height);
                            }
                            ps.handleScrollWheel = false;
                            m_PartScrollPos = ps.scrollPosition;
                        }
                    }

                    m_ScrollPos = s.scrollPosition;
                }
            }
        }

        private void DrawBtnAdd(List<RenderItemData> renderItems, GUILayoutOption width, GUILayoutOption height)
        {
            using (new GuiSkinFontSizeScope(GUI.skin.button, 40))
            {
                if (GUILayout.Button("+", width, height) == true)
                {
                    var selected = Selection.activeGameObject;
                    if (selected == null)
                    {
                        // nothing selected, create empty prefab
                        var prefab = PrefabManager.CreatePrefab(m_Selected, null);
                        renderItems.Add(PrefabManager.CreateRenderItem(prefab));
                    }
                    else
                    {
                        var prefabType = PrefabUtility.GetPrefabType(selected);
                        if (prefabType == PrefabType.Prefab)
                        {
                            // a prefab is selected, add it as variant
                            renderItems.Add(PrefabManager.CreateRenderItem(selected));
                        }
                        else if (PrefabManager.HasModelSelected() == true)
                        {
                            // a model is selected, create a prefab from it
                            renderItems.Add(PrefabManager.CreateNewPrefabFromModel(m_Selected, selected));
                        }
                    }
                }
            }
        }

        private void DrawAssetPreview(RenderItemData renderItem, GUILayoutOption width, GUILayoutOption height)
        {
            var preview = GetAssetPreview(renderItem);
            if (GUILayout.Button(preview, width, height) == true)
            {
                var prefab = PrefabManager.LoadPrefab(renderItem);
                if (prefab != null)
                {
                    TestSceneManager.EditPrefabInScene(prefab);
                }
            }
        }

        public static Texture GetAssetPreview(RenderItemData renderItem)
        {
            var prefab = PrefabManager.LoadPrefab(renderItem);
            if (prefab != null)
            {
                return AssetPreview.GetAssetPreview(prefab);
            }
            return null;
        }

        private static List<Tuple<Type, Type>> GetInterfaceImplementors<T>()
        {
            var interfaceTypes = Utils.Reflection.GetInterfaceImplementors<T>();
            interfaceTypes.RemoveAll(t => t.IsGenericType == true);
            interfaceTypes.RemoveAll(t => t.GetGenericBaseType() == null);
            return interfaceTypes.Extract(t => Tuple.New(t, t.GetGenericBaseType().GetGenericArguments()[0]));
        }
    }

    class SubItemView : DefaultListView<CatalogueEntry.SubItems>
    {
        private readonly CatalogueEntry m_Entry;

        public SubItemView(string name, CatalogueEntry entry, List<CatalogueEntry.SubItems> list) : base(name, list)
        {
            m_Entry = entry;
        }

        protected override CatalogueEntry.SubItems DrawElement(CatalogueEntry.SubItems renderItem)
        {
            DrawRenderItem(m_Entry, renderItem);
            return renderItem;
        }

        public static void DrawRenderItem(CatalogueEntry entry, CatalogueEntry.SubItems dataItem)
        {
            MetaDataView<GameObject>.GameObjectToString("Screen GameObject", ref dataItem.m_GameObject, ref dataItem.m_ObjectName);
            dataItem.m_GUID = EditorGUILayout.TextField($"Guid", dataItem.m_GUID);
            EditorGUILayout.Space();
        }
    }
}