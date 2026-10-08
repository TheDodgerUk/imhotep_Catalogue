#if UNITY_EDITOR
#if CATALOG_PROGRAM
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;
using FilePathHelper;
using UnityEditorInternal;

namespace Museum
{
    public class EnvironmentDataStructure
    {
        public List<string> EnvironmentPath = new List<string>();
    }

    public class MuseumLayoutStart : MonoBehaviour
    {
        public static MuseumLayoutItemListsEditorWindow curEditor;
        void Start()
        {
            curEditor = (MuseumLayoutItemListsEditorWindow)EditorWindow.GetWindow(typeof(MuseumLayoutItemListsEditorWindow));
            curEditor.Close();
            curEditor = (MuseumLayoutItemListsEditorWindow)EditorWindow.GetWindow(typeof(MuseumLayoutItemListsEditorWindow));
            curEditor.Init(this);
            curEditor.Show();
            UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
        }

        private void OnDestroy()
        {
            if (null != curEditor)
            {
                curEditor.Close();
                Destroy(curEditor);
            }
        }
    }

    public class MuseumLayoutItemListsEditorWindow : EditorWindow
    {
        public readonly Vector2 m_WindowSize = new Vector2(800, 600);
        private Vector2 m_ScrollPosition;
        public List<GameObject> m_PlinthsCreated = new List<GameObject>();

        public static MuseumLayoutItemListsEditorWindow curEditor;
        public MuseumFloorData m_MuseumFloorData;
        private int m_ItemListIndex = 1;
        private string m_FloorName = "Enter a name";
        private MuseumFloorDataContainer m_MuseumFloorDataContainer = new MuseumFloorDataContainer();
        private Vector2 scrollPos;
        private MuseumCataloge m_MuseumCataloge;
        private int m_ProductNameIndex = 0;
        private MonoBehaviour m_MonoBehaviour;

        private UnityEditorInternal.ReorderableList m_OptionalScenes;

        //lift videos
        private UnityEditorInternal.ReorderableList m_LiftVideoList;
        private int m_dropDownLiftVideoListIndex = 0;
        private List<string> m_VideoLoaderFileNames = new List<string>();
        private string[] m_m_VideoLoaderFileNamesArray;
        private GUIStyle m_BoldFont = new GUIStyle();

        //Emissive
        private Material m_LiftRingsEmissiveMaterail;
        private Material m_CeilingLightingEmissiveMaterail;

        public void Init(MonoBehaviour monoBehaviour)
        {
            VideoLoader.CreateInstance();
            VideoLoader.Instance.OnVideoSequenceLoaded(null);

            m_BoldFont.fontStyle = FontStyle.Bold;
            m_MonoBehaviour = monoBehaviour;
            curEditor = (MuseumLayoutItemListsEditorWindow)EditorWindow.GetWindow(typeof(MuseumLayoutItemListsEditorWindow));
            curEditor.titleContent.text = "Editor";

            CachedObjects.CreateInstance();
            foreach (GameObject item in GameObject.FindObjectsOfType<GameObject>())
            {
                if (item.name == CachedObjects.TAG)
                {
                    CachedObjects.Instance.AddObjectsToCache(item.transform);
                }
            }

            m_MuseumCataloge = m_MonoBehaviour.GetComponent<MuseumCataloge>();
            if (null == m_MuseumCataloge)
            {
                m_MuseumCataloge = m_MonoBehaviour.AddComponent<MuseumCataloge>();
            }

            m_MuseumCataloge.Initilise();
            FindEmissiveMaterail(ref m_LiftRingsEmissiveMaterail, "LIft");
            FindEmissiveMaterail(ref m_CeilingLightingEmissiveMaterail, "CeilingLighting");
        }



        #region  Lift Videos
        private void DrawLiftElementVideos(Rect rect, int index, bool active, bool focused)
        {
            string item = m_MuseumFloorData.m_LiftVideos[index];

            EditorGUI.BeginChangeCheck();

            m_dropDownLiftVideoListIndex = m_VideoLoaderFileNames.FindIndex(x => x == item);
            m_dropDownLiftVideoListIndex = EditorGUI.Popup(new Rect(rect.x + 18, rect.y, rect.width - 18, rect.height), m_dropDownLiftVideoListIndex, m_m_VideoLoaderFileNamesArray);

            if (EditorGUI.EndChangeCheck())
            {
                if (m_dropDownLiftVideoListIndex != -1)
                {
                    m_MuseumFloorData.m_LiftVideos[index] = m_VideoLoaderFileNames[m_dropDownLiftVideoListIndex];
                }
            }
        }

        private void DrawOptionalElement(Rect rect, int index, bool active, bool focused)
        {
            SceneAsset item = m_MuseumFloorData.m_OptionalScenesList[index];
            m_MuseumFloorData.m_OptionalScenesList[index] = (SceneAsset)EditorGUI.ObjectField(new Rect(rect.x + 18, rect.y, rect.width - 18, rect.height), item, typeof(SceneAsset), false);
        }

        private void DrawOptionalHeader(Rect rect)
        {
            GUI.Label(rect, "Optional Scenes", m_BoldFont);
        }

        private void AddOptional(ReorderableList list)
        {
            Debug.LogError("AddOptional");
            m_OptionalScenes.list.Add(null);
        }

        private void RemoveOptional(ReorderableList list)
        {
            m_OptionalScenes.list.RemoveAt(list.index);
        }




        private void DrawLiftHeaderVideos(Rect rect)
        {
            GUI.Label(rect, "Lift Videos", m_BoldFont);
        }

        private void AddLiftVideos(ReorderableList list)
        {
            m_LiftVideoList.list.Add(string.Empty);
        }

        private void RemoveLiftVideos(ReorderableList list)
        {
            m_LiftVideoList.list.RemoveAt(list.index);
        }
        #endregion

        void OnEnable()
        {
            this.minSize = m_WindowSize;
            this.maxSize = m_WindowSize;
            LoadItemList();
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (m_MuseumFloorData == null)
                GUILayout.Label("Item Lists Editor", EditorStyles.label);
            else
                GUILayout.Label("Item Lists Editor - " + m_MuseumFloorData.m_Name, EditorStyles.label);


            if (m_MuseumFloorData != null && GUILayout.Button("Close Save Item Lists", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                CloseItemList();
            }
            GUILayout.EndHorizontal();

            //
            if (m_MuseumFloorData == null)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Enter Floor Name:", EditorStyles.label, GUILayout.ExpandWidth(false));
                m_FloorName = EditorGUILayout.TextField("", m_FloorName, EditorStyles.textField, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Create Floor", GUILayout.Width(150)))
                {
                    if (m_FloorName != "Enter a name" && !string.IsNullOrEmpty(m_FloorName))
                    {
                        CreateNewItemList(m_FloorName);
                        m_FloorName = "Enter a name";
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Item Lists Editor Message", "Please enter a valid name!", "OK");
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Floors", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Refresh", GUILayout.Width(100)))
                {
                    RefreshItemList();
                }
                GUILayout.EndHorizontal();
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                if (m_MuseumFloorDataContainer != null)
                {
                    for (int i = 0; i < m_MuseumFloorDataContainer.GetCount(); i++)
                    {
                        MuseumFloorData museumFloorDataaRef = m_MuseumFloorDataContainer.GetAt(i);
                        if (museumFloorDataaRef != null)
                        {
                            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                            EditorGUILayout.LabelField(museumFloorDataaRef.m_Name, EditorStyles.label, GUILayout.ExpandWidth(true));
                            if (GUILayout.Button("Open", GUILayout.Width(53)))
                            {
                                m_MuseumFloorData = museumFloorDataaRef;
                                SetEmissiveMaterails();
                                ReloadGUIIndexItem();
                                PlinthCreateAndMove(true);
                                SetFirstPosition();
                                Selection.activeGameObject = m_PlinthsCreated[0];

                                m_OptionalScenes = new UnityEditorInternal.ReorderableList(m_MuseumFloorData.m_OptionalScenesList, typeof(Scene));
                                m_OptionalScenes.drawHeaderCallback -= DrawOptionalHeader;
                                m_OptionalScenes.drawHeaderCallback += DrawOptionalHeader;
                                m_OptionalScenes.drawElementCallback -= DrawOptionalElement;
                                m_OptionalScenes.drawElementCallback += DrawOptionalElement;
                                m_OptionalScenes.onAddCallback -= AddOptional;
                                m_OptionalScenes.onAddCallback += AddOptional;
                                m_OptionalScenes.onRemoveCallback -= RemoveOptional;
                                m_OptionalScenes.onRemoveCallback += RemoveOptional;


                                m_LiftVideoList = new UnityEditorInternal.ReorderableList(m_MuseumFloorData.m_LiftVideos, typeof(string));

                                m_LiftVideoList.drawHeaderCallback -= DrawLiftHeaderVideos;
                                m_LiftVideoList.drawHeaderCallback += DrawLiftHeaderVideos;
                                m_LiftVideoList.drawElementCallback -= DrawLiftElementVideos;
                                m_LiftVideoList.drawElementCallback += DrawLiftElementVideos;
                                m_LiftVideoList.onAddCallback -= AddLiftVideos;
                                m_LiftVideoList.onAddCallback += AddLiftVideos;
                                m_LiftVideoList.onRemoveCallback -= RemoveLiftVideos;
                                m_LiftVideoList.onRemoveCallback += RemoveLiftVideos;

                                m_VideoLoaderFileNames = VideoLoader.Instance.FileNames;
                                m_m_VideoLoaderFileNamesArray = m_VideoLoaderFileNames.ToArray();
                            }
                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                ConfirmDeleteItemList(museumFloorDataaRef);
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }

            IndividualItems();
        }

        private void IndividualItems()
        {
            if (null != m_MuseumFloorData)
            {
                m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, false, true, GUILayout.Width(m_WindowSize.x), GUILayout.Height(m_WindowSize.y));
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.BeginVertical();
                GUILayout.Space(10);
                EmmisiveColour();
                m_LiftVideoList.DoLayoutList();
                GUILayout.Space(2);
                m_OptionalScenes.DoLayoutList();
                GUILayout.Space(15);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.Space(15);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                PlinthCreateAndMove(false);
                AddDeleteItemsInterface();

                if (m_MuseumFloorData.m_ItemDataList.Count > 0)
                {
                    bool remake = ItemName();
                    remake |= ItemPlinth();
                    ItemPosition();

                    if (true == remake)
                    {
                        ForceSetAllItems();
                    }
                }
                else
                {
                    GUILayout.Label("This Item List is Empty.");
                }
                GUILayout.EndScrollView();
            }
            else
            {
                m_ProductNameIndex = -1;
                for (int i = 0; i < m_PlinthsCreated.Count; i++)
                {
                    Destroy(m_PlinthsCreated[i]);
                }
                m_PlinthsCreated.Clear();
            }
        }

        private void EmmisiveColour()
        {
            EditorGUI.BeginChangeCheck();
            m_MuseumFloorData.m_EmissiveColour = EditorGUILayout.ColorField("EmissiveColour", m_MuseumFloorData.m_EmissiveColour);
            if (EditorGUI.EndChangeCheck())
            {
                SetEmissiveMaterails();
            }
        }

        private void SetEmissiveMaterails()
        {
            m_LiftRingsEmissiveMaterail.SetColor(ShaderID.EMISSION_COLOR, m_MuseumFloorData.m_EmissiveColour);
            m_CeilingLightingEmissiveMaterail.SetColor(ShaderID.EMISSION_COLOR, m_MuseumFloorData.m_EmissiveColour);
        }

        private void AddDeleteItemsInterface()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Prev", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                if (m_ItemListIndex > 1)
                {
                    m_ItemListIndex--;
                    ReloadGUIIndexItem();
                    Selection.activeGameObject = m_PlinthsCreated[m_ItemListIndex - 1];
                }
            }
            if (GUILayout.Button("Next", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                if (m_ItemListIndex < m_MuseumFloorData.m_ItemDataList.Count)
                {
                    m_ItemListIndex++;
                    ReloadGUIIndexItem();
                    Selection.activeGameObject = m_PlinthsCreated[m_ItemListIndex - 1];
                }
            }

            GUILayout.Space(15);

            if (GUILayout.Button("Add Item", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                AddItem();
            }
            if (GUILayout.Button("Delete Item", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false)))
            {
                DeleteItem(m_ItemListIndex - 1);
            }

            GUILayout.Space(15);

            if (m_MuseumFloorData.m_ItemDataList.Count > 0)
            {
                m_ItemListIndex = EditorGUILayout.IntSlider(m_ItemListIndex, 1, m_MuseumFloorData.m_ItemDataList.Count, GUILayout.ExpandWidth(true), GUILayout.Height(14));
                EditorGUILayout.LabelField("of   " + m_MuseumFloorData.m_ItemDataList.Count.ToString() + "  items", "", EditorStyles.label, GUILayout.ExpandWidth(false));
            }

            GUILayout.EndHorizontal();
        }

        private bool ItemName()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.BeginVertical();
            GUILayout.Label($"Item Details Current Name {m_MuseumFloorData.m_ItemDataList[m_ItemListIndex - 1].m_ProductShortName}", EditorStyles.boldLabel);

            int startProductIndex = m_ProductNameIndex;

            if (m_ProductNameIndex >= 0 && m_ProductNameIndex < m_MuseumCataloge.m_CatalogueNameList.Count)
            {
                m_ProductNameIndex = EditorGUILayout.Popup("Product Name", m_ProductNameIndex, m_MuseumCataloge.m_CatalogueNameList.ToArray(), GUILayout.ExpandWidth(true));
                m_MuseumFloorData.m_ItemDataList[m_ItemListIndex - 1].m_ProductShortName = m_MuseumCataloge.m_CatalogueNameList[m_ProductNameIndex];
            }
            else
            {
                GUILayout.Label($"Environment ERROR", EditorStyles.boldLabel);
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            return (startProductIndex != m_ProductNameIndex);
        }

        private void ReloadGUIIndexItem()
        {
            m_ProductNameIndex = m_MuseumCataloge.m_CatalogueNameList.FindIndex(x => x.Equals(m_MuseumFloorData.m_ItemDataList[m_ItemListIndex - 1].m_ProductShortName));

            if (m_ProductNameIndex == -1)
            {
                m_ProductNameIndex = 0;
            }
        }

        private bool ItemPlinth()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.BeginVertical();
            GUILayout.Label("Item Plinth", EditorStyles.boldLabel);
            CachedObject original = m_MuseumFloorData.m_ItemDataList[m_ItemListIndex - 1].m_PlinthType;
            m_MuseumFloorData.m_ItemDataList[m_ItemListIndex - 1].m_PlinthType = (CachedObject)EditorGUILayout.EnumPopup("Primitive to create:", original);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            return (original != m_MuseumFloorData.m_ItemDataList[m_ItemListIndex - 1].m_PlinthType);
        }

        private void ItemPosition()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.BeginVertical();
            GUILayout.Label("Item Position", EditorStyles.boldLabel);
            m_MuseumFloorData.m_ItemDataList[m_ItemListIndex - 1].m_Position = m_PlinthsCreated[m_ItemListIndex - 1].transform.position;
            EditorGUILayout.Vector3Field("Position", m_MuseumFloorData.m_ItemDataList[m_ItemListIndex - 1].m_Position, GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }


        private void ForceSetAllItems()
        {
            PlinthCreateAndMove(true);
            SetFirstPosition();
            Selection.activeGameObject = m_PlinthsCreated[m_ItemListIndex - 1];
        }


        private void PlinthCreateAndMove(bool force)
        {
            if ((m_MuseumFloorData.m_ItemDataList.Count != m_PlinthsCreated.Count) || (true == force))
            {
                for (int i = 0; i < m_PlinthsCreated.Count; i++)
                {
                    Destroy(m_PlinthsCreated[i]);
                }
                m_PlinthsCreated.Clear();

                for (int i = 0; i < m_MuseumFloorData.m_ItemDataList.Count; i++)
                {
                    GameObject plinthRef = CachedObjects.Instance.InstantiateCachedGameObject(m_MuseumFloorData.m_ItemDataList[i].m_PlinthType.ToString());
                    m_PlinthsCreated.Add(plinthRef);
                    m_PlinthsCreated[i].SetActive(true);
                    CatalogueEntry item = Catalogue.Instance.Entries.Find(x => x.ShortName.ToString() == m_MuseumFloorData.m_ItemDataList[i].m_ProductShortName);

                    if (null != item)
                    {
                        GameObject GameObjectRef = m_MuseumCataloge.LoadPrefab(item);
                        Transform connectionPoint = m_PlinthsCreated[i].transform.SearchChildren(PlinthConsts.CATALOGE_DISPLAY_POINT)[0];
                        GameObjectRef.transform.SetParent(connectionPoint.transform);
                        GameObjectRef.transform.localPosition = Vector3.zero;
                    }
                }
            }
        }


        private void SetFirstPosition()
        {
            for (int i = 0; i < m_MuseumFloorData.m_ItemDataList.Count; i++)
            {
                m_PlinthsCreated[i].transform.position = m_MuseumFloorData.m_ItemDataList[i].m_Position;
            }
        }


        void CreateNewItemList(string _name)
        {
            m_ItemListIndex = 1;
            m_MuseumFloorData = MuseumLayoutListUtils.Create(_name);
            if (null != m_MuseumFloorData)
            {
                m_MuseumFloorData.m_ItemDataList = new List<PlinthItemData>();
                m_MuseumFloorDataContainer.Add(m_MuseumFloorData);
                m_MuseumFloorData.m_Name = _name;
                AddItem();
                ForceSetAllItems();
            }
            ReloadGUIIndexItem();
        }


        void CloseItemList()
        {
            m_MuseumFloorData = null;
            EditorPrefs.SetString("ObjectPath", null);
            RefreshItemList();
        }

        void AddItem()
        {
            PlinthItemData newItem = new PlinthItemData();
            newItem.m_ProductShortName = "New Item";
            m_MuseumFloorData.m_ItemDataList.Add(newItem);
            m_ItemListIndex = m_MuseumFloorData.m_ItemDataList.Count;
            ForceSetAllItems();
        }

        void RefreshItemList()
        {
            Museum.MuseumLayoutListUtils.Save(m_MuseumFloorDataContainer);
            m_MuseumFloorDataContainer = Museum.MuseumLayoutListUtils.LoadData();
        }

        void LoadItemList()
        {
            m_MuseumFloorDataContainer = Museum.MuseumLayoutListUtils.LoadData();
        }


        void DeleteItem(int index)
        {
            m_MuseumFloorData.m_ItemDataList.RemoveAt(index);
            ForceSetAllItems();
        }

        void DeleteItemList(MuseumFloorData _inv)
        {
            m_MuseumFloorDataContainer.Remove(_inv);
            RefreshItemList();
        }

        void ConfirmDeleteItemList(MuseumFloorData _inv)
        {
            if (true == EditorUtility.DisplayDialog("Are you sure to delete this Item List?",
                     "Please choose one of the following options.",
                     "Delete Item",
                     "Cancel"))
            {
                DeleteItemList(_inv);
            }
        }


        private void FindEmissiveMaterail(ref Material materialRef, string name)
        {
            foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
            {
                if ((gameObj.name == name) && (true == gameObj.activeInHierarchy))
                {
                    var renderer = gameObj.GetComponent<MeshRenderer>();
                    Material[] materials = renderer.sharedMaterials;
                    int index = Array.FindIndex(materials, e => e.name.Contains("_Emissive"));
                    if (index > -1)
                    {
                        Debug.LogWarning($"This is a slow way to find it at moment as there are 2 lifts in the project");
                        materialRef = materials[index];
                        return;
                    }
                }
            }
        }
    }
}
#endif
#endif