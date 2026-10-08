using EditorTools.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using static EnvironmentData;

namespace EditorTools.Environments
{
    public class EnvironmentVariantDataView : DefaultListView<EnvironmentVariantData>
    {
        private enum SceneTypeEnum
        {
            Original,
            Optional,
            Generated,
        }
        protected override bool Spacing => true;

        private readonly Dictionary<EnvironmentVariantData, bool> m_Foldout;
        private readonly EnvironmentSceneData m_SceneData;


        int m_ModuleIndex = 0;
        string[] ModulePopupOptions = new string[] { "Data", "Not", "Found" };

        int m_AttributeIndex = 0;
        string[] AttributePopupOptions = new string[] { "Data", "Not", "Found" };

        public EnvironmentVariantDataView(EnvironmentSceneData sceneData) : base("Variants", sceneData.Variants)
        {
            m_Foldout = sceneData.Variants.ExtractAsKeys(v => false);

            var list = Utils.Code.GetEnumValues<MetaDataType>();
            ModulePopupOptions = new string[list.Count-1]; // -1 to remove NOTHING
            for (int i = 1; i < ModulePopupOptions.Length; i++)// start a 1 to remove NOTHING
            {
                ModulePopupOptions[i] = list[i].ToString();
            }

            var attributeList = Utils.Code.GetEnumValues<MetaAttribute>();
            AttributePopupOptions = new string[attributeList.Count];
            for (int i = 0; i < AttributePopupOptions.Length; i++)
            {
                AttributePopupOptions[i] = attributeList[i].ToString();
            }

            m_SceneData = sceneData;
        }

        public static GameObject LoadPrefab(string Guid)
        {
            if (string.IsNullOrEmpty(Guid) == false)
            {
                string path = AssetDatabase.GUIDToAssetPath(Guid);
                if (string.IsNullOrEmpty(path) == false)
                {
                    return AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
            }
            return null;
        }

        protected override EnvironmentVariantData DrawElement(EnvironmentVariantData variant)
        {

            // if this breaks, then it means you need to fix the file thats been overwritten
            var test = Autohand.HandPoseData.VALID_FINGERS;

            const int WIDTH = 200;
            var boldtext = new GUIStyle(GUI.skin.label);
            boldtext.fontStyle = FontStyle.BoldAndItalic;
            boldtext.fontSize = 15;
            boldtext.alignment = TextAnchor.MiddleCenter;

            bool foldout = m_Foldout.TryGet(variant) ?? false;
            foldout = EditorGUILayout.Foldout(foldout, $"{variant.Guid} - {variant.Name}", true);
            m_Foldout[variant] = foldout;
            if (foldout == true)
            {
                using (new VerticalStyleScope(BackgroundStyle.Box))
                {
                    Utils.View.DrawGuid(variant.Guid);
                    variant.Name.Text = EditorGUILayout.TextField("Name", variant.Name.Text);
                    variant.IncludeInBuild = EditorGUILayout.Toggle("Include In Build", variant.IncludeInBuild);
                    variant.IncludeInUnitTests = EditorGUILayout.Toggle("Include In Unit Tests", variant.IncludeInUnitTests);
                    variant.InteractionType = (GlobalConsts.InteractionType)EditorGUILayout.EnumPopup("InteractionType", variant.InteractionType);


                    EditorHelper.Separator();
                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Asset Server", boldtext, GUILayout.ExpandWidth(true));
                    GUILayout.Space(10);

                    variant.ExtraBundlesToInclude = EditorGUILayout.MaskField("ExtraBundlesToInclude", variant.ExtraBundlesToInclude, GlobalConsts.ExternalSourceAssetBundles.ToArray());

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Include Catalogue Prefabs", GUILayout.MaxWidth(WIDTH));
                        variant.IncludeCataloguePrefabs = EditorGUILayout.Toggle("", variant.IncludeCataloguePrefabs);
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Include Generic Asset Bundles", GUILayout.MaxWidth(WIDTH));
                        variant.IncludeGenericAssetBundles = EditorGUILayout.Toggle("", variant.IncludeGenericAssetBundles);
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Single Catalogue Item Only", GUILayout.MaxWidth(WIDTH));
                        variant.SingleCatalogueItemOnly = EditorGUILayout.Toggle("", variant.SingleCatalogueItemOnly);
                    }

                    EditorGUILayout.Space();
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        string[] extraFromServer = System.Enum.GetNames(typeof(GlobalConsts.ExtraFromServer));
                        EditorGUILayout.LabelField("FromServer ExtraBundles", GUILayout.MaxWidth(WIDTH));
                        variant.ExtraBundlesFromServer = EditorGUILayout.MaskField("", variant.ExtraBundlesFromServer, extraFromServer, GUILayout.MaxWidth(WIDTH));
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("FromServer CataloguePrefabs", GUILayout.MaxWidth(WIDTH));
                        variant.CataloguePrefabsFromServer = EditorGUILayout.Toggle("", variant.CataloguePrefabsFromServer);
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("FromServer CatalogueList", GUILayout.MaxWidth(WIDTH));
                        variant.CatalogueListFromServer = EditorGUILayout.Toggle("", variant.CatalogueListFromServer);
                    }

                    GUILayout.Space(10);
                    EditorGUILayout.LabelField("Asset Server Version control", boldtext, GUILayout.ExpandWidth(true));
                    GUILayout.Space(10);
                    
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        variant.CatalogueDataVersion = EditorGUILayout.IntField("CatalogueDataVersion", variant.CatalogueDataVersion, GUILayout.MaxWidth(WIDTH));
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        variant.AudioClipVersion = EditorGUILayout.IntField("AudioClipVersion", variant.AudioClipVersion, GUILayout.MaxWidth(WIDTH));
                        variant.Texture2DVersion = EditorGUILayout.IntField("Texture2DVersion", variant.Texture2DVersion, GUILayout.MaxWidth(WIDTH));                       
                        variant.MaterialVersion = EditorGUILayout.IntField("MaterialVersion", variant.MaterialVersion, GUILayout.MaxWidth(WIDTH));
                    }

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        variant.GameObjectVersion = EditorGUILayout.IntField("GameObjectVersion", variant.GameObjectVersion, GUILayout.MaxWidth(WIDTH));
                        variant.VideoClipVersion = EditorGUILayout.IntField("VideoClipVersion", variant.VideoClipVersion, GUILayout.MaxWidth(WIDTH));
                        variant.TextVersion = EditorGUILayout.IntField("TextVersion", variant.VideoClipVersion, GUILayout.MaxWidth(WIDTH));
                    }


                    EditorHelper.Separator();
                    GUILayout.Space(10);
                    EditorGUILayout.Space();


                    EditorGUILayout.Space();


                    EditorGUILayout.Space();
                    EditorGUILayout.Space();


                    EditorGUILayout.Space();




                    EditorGUILayout.Space();

                    variant.UseOverloadData = EditorGUILayout.Toggle("UseOverloadData", variant.UseOverloadData);
                    if(variant.UseOverloadData == true)
                    {
                        variant.OverloadEnvironmentGuid = EditorGUILayout.TextField("OverloadEnvironmentGuid", variant.OverloadEnvironmentGuid);
                        EditorGUILayout.LabelField($"OverloadEnvironmentName  {variant.OverloadEnvironmentName()}", EditorStyles.boldLabel);
                        variant.OverloadVariantGuid = EditorGUILayout.TextField("OverloadVariantGuid", variant.OverloadVariantGuid);
                        EditorGUILayout.LabelField($"OverloadVariantName      {variant.OverloadVariantName()}", EditorStyles.boldLabel);
                        EditorGUILayout.Space();
                    }


                    EditorGUILayout.Space();

                    var btnWidth_3Buttons = GUILayout.Width(150);
                    var btnWidth_3ButtonsHalf = GUILayout.Width(73);
                    var btnWidth_2Buttons = GUILayout.Width(227);

                    // draw original scenes
                    DrawSceneAssets("Original", variant,  variant.OriginalScenes, ref variant.OriginalFoldout, () =>
                    {
                        if (GUILayout.Button("Replace Selected", btnWidth_3Buttons) == true)
                        {
                            variant.SetScenes(GetSelectedScenes());
                        }
                        if (GUILayout.Button("Add Selected", btnWidth_3Buttons) == true)
                        {
                            variant.AddScenes(GetSelectedScenes());
                        }
                        if (GUILayout.Button("View Original", btnWidth_3Buttons) == true)
                        {
                            Bundles.BundleBuilderWindow.m_ValidBuildBundles = false;
                            Bundles.BundleBuilderWindow.m_ValidCopyToAll = false;
                            Bundles.BundleBuilderWindow.m_ValidCopyToVariant = false;
                            ClearLog();
                            EnvironmentVariantData debugData = variant;
                            string name = variant.Environment.Name.ToString();

                            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
                            environmentData.LoadFromFile();
                            environmentData.SetVariant(variant);

                            string combinedName = $"{EnvironmentDataManager.rootFolder}{variant.CombinedName}".SanitiseSlashes();                            
                            EnvironmentSceneManager.OpenScenes(variant.OriginalScenes);

                            EnvironmentSceneManager.StaticCheck(GetOnlyOriginalAndAllowedGeneratedScenes(variant));
                            EditorSceneManager.MarkAllScenesDirty();
                            EditorSceneManager.SaveOpenScenes();

                        }
                    }, SceneTypeEnum.Original);

                    DrawSceneAssets("Optional", variant, variant.OptionalScenes, ref variant.OptionalFoldout, () =>
                    {
                        if (GUILayout.Button("+ All", btnWidth_3ButtonsHalf) == true)
                        {
                            for (int i = 0; i < variant.OptionalScenes.Count; i++)
                            {
                                var path = variant.OptionalScenes[i].GetAssetPath();
                                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(variant.OptionalScenes[i].GetAssetPath().Relative, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                            }
                        }
                        if (GUILayout.Button("- All", btnWidth_3ButtonsHalf) == true)
                        {
                            for (int i = 0; i < variant.OptionalScenes.Count; i++)
                            {
                                var path = variant.OptionalScenes[i].GetAssetPath();
                                UnityEditor.SceneManagement.EditorSceneManager.CloseScene(SceneManager.GetSceneByName(variant.OptionalScenes[i].name), true);
                            }
                        }
                        if (GUILayout.Button("Replace Selected", btnWidth_3Buttons) == true)
                        {
                            variant.SetOptionalScenes(GetSelectedScenes());
                        }
                        if (GUILayout.Button("Add Selected", btnWidth_3Buttons) == true)
                        {
                            variant.AddOptionalScenes(GetSelectedScenes());
                        }
                    }, SceneTypeEnum.Optional);

                    // generated scenes
                    DrawSceneAssets("Generated", variant,  variant.GeneratedScenes, ref variant.GeneratedFoldout, () =>
                    {
                        if (GUILayout.Button("Generate Scenes", btnWidth_3Buttons) == true)
                        {
                            EnvironmentSceneGenerator.GenerateScenes(m_SceneData, variant);
                            EnvironmentSceneGenerator.GenerateDuplicateScenes(variant, variant.OptionalScenes, false);
                            EnvironmentSceneManager.OpenScenes(variant.GeneratedScenes);
                            EditorSceneManager.MarkAllScenesDirty();
                            EditorSceneManager.SaveOpenScenes();
                        }
                        if (GUILayout.Button("View Generated", btnWidth_3Buttons) == true)
                        {
                            EnvironmentSceneManager.OpenScenes(variant.GeneratedScenes);
                            EditorSceneManager.MarkAllScenesDirty();
                            EditorSceneManager.SaveOpenScenes();
                        }


                    }, SceneTypeEnum.Generated);


                    if (variant.GeneratedFoldout == false)
                    {
                        // bake
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();

                            using (new GuiEnabledScope(!EnvironmentSceneManager.IsBaking()))
                            {


                                if (GUILayout.Button(new GUIContent("Bake Lighting", "Original Baking"), btnWidth_3Buttons) == true)
                                {
                                    EnvironmentSceneManager.BakeLighting(GetOnlyOriginalAndAllowedGeneratedScenes(variant));
                                    EditorSceneManager.MarkAllScenesDirty();
                                    EditorSceneManager.SaveOpenScenes();
                                    Debug.Log("Lighting Bake Started!\n");
                                }

                                if (GUILayout.Button("Bake NavMesh", btnWidth_3Buttons) == true)
                                {
                                    EnvironmentSceneManager.BakeNavMesh(GetOnlyOriginalAndAllowedGeneratedScenes(variant));
                                    EditorSceneManager.MarkAllScenesDirty();
                                    EditorSceneManager.SaveOpenScenes();
                                    Debug.Log("NavMesh Bake Complete!\n");
                                }

                                if (GUILayout.Button("Bake Occlusion", btnWidth_3Buttons) == true)
                                {
                                    EnvironmentSceneManager.BakeOcclusion(GetOnlyOriginalAndAllowedGeneratedScenes(variant));
                                    EditorSceneManager.MarkAllScenesDirty();
                                    EditorSceneManager.SaveOpenScenes();
                                    Debug.Log("Occlusion Bake Complete!\n");
                                }
                            }
                        }
                    }
                    else
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button(new GUIContent("Bake Lighting (Custom)", "DOES NOT WORK YET Override Lighting setting using the quality option selected for each scene in the Generated Scenes."), btnWidth_3Buttons) == true)
                            {
                                EnvironmentSceneManager.BakeLightingV(variant, GetOnlyOriginalAndAllowedGeneratedScenes(variant));
                                Debug.Log("Lighting Bake Started!\n");
                            }

                            if (GUILayout.Button(new GUIContent("Delete Lightbake Data", "Delete all light bake data"), btnWidth_3Buttons) == true)
                            {
                                Lightmapping.Clear();
                                Lightmapping.ClearLightingDataAsset();
                                //var generatedRoot = EnvironmentSceneGenerator.GeneratedSceneRoot.Absolute;
                                //string[] dirs = Directory.GetDirectories(generatedRoot, $"{PlayerPrefs.GetString(Museum.MuseumCataloge.ENVIRONMENT_GUID)}*", SearchOption.TopDirectoryOnly);

                                //foreach(var item in dirs)
                                //{
                                //    Directory.Delete(item, true);
                                //}
                            }

                            if (GUILayout.Button(new GUIContent("Build Like Build Machine", "This builds with same code as \nthe build machine"), btnWidth_3Buttons) == true)
                            {
                                BuildSystem.BuildLightingJob.GoThoughGenerateScenesLighingEct();
                            }
                        }
                    }
                    DisplayEnvironmentModuleConfiguration(variant);
                }
            }
            return variant;
        }

        public void ClearLog()
        {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }

        public static List<SceneAsset> GetOnlyOriginalAndAllowedGeneratedScenes(EnvironmentVariantData variant)
        {
            // generated => na1aa4f72-78c2-43be-8af5-d683042854cd_Lighting
            // split it to last part => Lighting
            // and then checks it with the OptionalScenes
            List<string> optionalScenesName = new List<string>();           // these will NOT be baked 
            for(int i = 0; i < variant.OptionalScenes.Count; i++)
            {
                if (variant.m_OptionalBaking.ToBeBaked.Count > i)       // if the bool has been entered
                {
                    if (false == variant.m_OptionalBaking.ToBeBaked[i])     // if not to be baked 
                    {
                        optionalScenesName.Add(variant.OptionalScenes[i].name); 
                    }
                }
                else
                {
                    optionalScenesName.Add(variant.OptionalScenes[i].name); // cant find the bool then it will not be baked 
                }
            }

            if (null != variant.OptionalScenes)
            {
                List<SceneAsset> sceneList = new List<SceneAsset>();
                for (int i = 0; i < variant.GeneratedScenes.Count; i++)
                {
                    string[] nameSplit = variant.GeneratedScenes[i].name.Split('_');
                    string endPartOfName = nameSplit[nameSplit.Length - 1];
                    if (false == optionalScenesName.Exists(e => e == endPartOfName))
                    {
                        sceneList.Add(variant.GeneratedScenes[i]);
                    }
                }
                return sceneList;
            }
            else
            {
                Debug.LogError("variant.OptionalScenes is null");
                return variant.GeneratedScenes;
            }           
        }


        private void DisplayEnvironmentModuleConfiguration(EnvironmentVariantData variant)
        {
            using (new VerticalStyleScope(BackgroundStyle.Box))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Attributes Allowed");
                for (int i = 0; i < variant.AttributesAllowed.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    variant.AttributesAllowed[i] = EditorGUILayout.TextField($"env: {i}", variant.AttributesAllowed[i]);
                    if (true == GUILayout.Button("Remove Module"))
                    {
                        variant.AttributesAllowed.RemoveAt(i);
                        i--;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.BeginHorizontal();
                m_AttributeIndex = EditorGUILayout.Popup(m_AttributeIndex, AttributePopupOptions);
                if (true == GUILayout.Button("Add Module"))
                {
                    variant.AttributesAllowed.Add(AttributePopupOptions[m_AttributeIndex]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            foreach (var variant in m_SceneData.Variants)
            {
                if (IsLoaded(variant.OriginalScenes) || IsLoaded(variant.GeneratedScenes))
                {

                }
            }
        }



        private static bool IsLoaded(List<SceneAsset> scenes)
        {
            foreach (var scene in scenes)
            {
                if (scene == null)
                {
                    return false;
                }
                if (SceneManager.GetSceneByName(scene.name).IsValid() == false)
                {
                    return false;
                }
            }
            return true;
        }

        private static List<SceneAsset> GetSelectedScenes()
        {
            var scenes = Selection.objects.Select(o => o as SceneAsset).Where(s => s != null);
            return scenes.OrderBy(s => s.name, Comparisons.GetAlphanumeric()).ToList();
        }

        private static void DrawSceneAssets(string label, EnvironmentVariantData variant, List<SceneAsset> sceneAssets, ref bool foldout, Action drawButtons, SceneTypeEnum sceneType)
        {
            // original scenes
            using (new EditorGUILayout.HorizontalScope())
            {
                foldout = EditorGUILayout.Foldout(foldout, $"{label} Scenes ({sceneAssets.Count})", true);
                drawButtons();
            }
            if (foldout == true)
            {
                GUILayout.Space(4);
                using (new GuiIndentScope())
                {
                    DrawScenes(variant, sceneAssets, true, sceneType);
                }
            }
            if (sceneAssets.Exists(s => s == null) == true)
            {
                EditorGUILayout.HelpBox("Missing Scenes", MessageType.Error);
            }
        }
        private static void DrawScenes(EnvironmentVariantData variant, List<SceneAsset> scenes, bool modify, SceneTypeEnum sceneType)
        {
            if(sceneType == SceneTypeEnum.Generated)
            {
                if (null == variant.m_BakeQuality)
                {
                    variant.m_BakeQuality = new List<EnvironmentData.BakeQuality>();
                }

                for (int i = variant.m_BakeQuality.Count; i < scenes.Count; i++)
                {
                    variant.m_BakeQuality.Add(new EnvironmentData.BakeQuality());
                }
            }

            for (int i = 0; i < scenes.Count; ++i)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var scene = EditorHelper.ObjectField(scenes[i]);
                    if (modify == true)
                    {
                        scenes[i] = scene;
                    }
                    if (sceneType == SceneTypeEnum.Original)
                    {
                        if (BtnDelete() == true)
                        {
                            scenes.RemoveAt(i);
                            --i;
                        }
                    }

                    if (sceneType == SceneTypeEnum.Original)
                    {
                        if (BtnGenerate() == true)
                        {
                            List<SceneAsset> sceneList = new List<SceneAsset>();
                            sceneList.Add(scenes[i]);
                            EnvironmentSceneManager.BakeLighting(sceneList);
                        }
                    }

                    if (sceneType == SceneTypeEnum.Optional)
                    {
                        if (BtnDelete() == true)
                        {
                            scenes.RemoveAt(i);
                            --i;
                        }
                        if (null == variant.m_OptionalBaking)
                        {
                            variant.m_OptionalBaking = new EnvironmentData.OptionalBaking();
                        }
                        if(variant.m_OptionalBaking.ToBeBaked.Count <= i)
                        {
                            variant.m_OptionalBaking.ToBeBaked.Add(false);
                        }

                        if (true == variant.m_OptionalBaking.ToBeBaked[i])
                        {
                            if (BtnToLightBakeOptional() == true)
                            {
                                variant.m_OptionalBaking.ToBeBaked[i] = false;
                            }
                        }
                        else
                        {
                            if (BtnNotToLightBakeOptional() == true)
                            {
                                variant.m_OptionalBaking.ToBeBaked[i] = true;
                            }
                        }
                        

                        if (BtnAddOptional() == true)
                        {
                            var path = scenes[i].GetAssetPath();                          
                            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenes[i].GetAssetPath().Relative, UnityEditor.SceneManagement.OpenSceneMode.Additive);
                        }

                        if (BtnRemoveOptional() == true)
                        {
                            var path = scenes[i].GetAssetPath();
                            UnityEditor.SceneManagement.EditorSceneManager.CloseScene(SceneManager.GetSceneByName(scenes[i].name), true);
                        }
                    }

                    if (sceneType == SceneTypeEnum.Generated)
                    {

                        variant.m_BakeQuality[i].m_IsBaked = BoolIsBaked(variant.m_BakeQuality[i].m_IsBaked);

                        int result = 0; 
                        if(true == Int32.TryParse(TxtNumber(variant.m_BakeQuality[i].m_Index), out result))
                        {
                            variant.m_BakeQuality[i].m_Index = result;
                        }
                        else
                        {
                            variant.m_BakeQuality[i].m_Index = 0;
                        }

                        variant.m_BakeQuality[i].m_QualityEnum = EnmQuality(variant.m_BakeQuality[i].m_QualityEnum);


                    }
                }
            }
        }
        
        private static bool BtnDelete() => GUILayout.Button(new GUIContent("X", "Delete Scene"), GUILayout.Width(22));

        private static bool BtnToLightBakeOptional() => GUILayout.Button(new GUIContent("Y", "Y This scene will be light baked, N This scene will not be light baked"), GUILayout.Width(20));
        private static bool BtnNotToLightBakeOptional() => GUILayout.Button(new GUIContent("N", "Y This scene will be light baked, N This scene will not be light baked"), GUILayout.Width(20));
        private static bool BtnAddOptional() => GUILayout.Button("+", GUILayout.Width(20));
        private static bool BtnRemoveOptional() => GUILayout.Button("-", GUILayout.Width(15));

        private static bool BoolIsBaked(bool baked) => GUILayout.Toggle(baked, new GUIContent("", "Include in Bake Lighting"), GUILayout.Width(15));
        private static string TxtNumber(int number) => GUILayout.TextField(number.ToString(), GUILayout.Width(20));
        private static LightBakeQualityEnum EnmQuality(LightBakeQualityEnum quality) => (LightBakeQualityEnum)EditorGUILayout.EnumPopup(quality, GUILayout.Width(100), GUILayout.ExpandWidth(false));
        private static bool BtnGenerate() => GUILayout.Button(new GUIContent("B", "Lightbake only this scene"), GUILayout.Width(25));

    }
}
