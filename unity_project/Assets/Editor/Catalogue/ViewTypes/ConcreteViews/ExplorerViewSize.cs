using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public static class ExplorerViewSize
    {
        private static readonly string DisplayKey = $"{nameof(ExplorerViewSize)}_display".ShaHash();
        private static readonly string SizeKey = $"{nameof(ExplorerViewSize)}_size".ShaHash();
        public const int MinWidth = 272;

        public static SearchDisplayMode Display { get; private set; }
        public static float Size { get; private set; }

        private static Environments.EnvironmentDataManager m_EnvironmentDataManager;
        private static List<string> m_EnvirmentFilterList = new List<string>();
        public const string FILTER_ALL = "ALL";

        public static string CurrentEnvirmentFilter { get; private set; }

        public static MetaDataType CurrentMetaData { get; private set; }
        public static bool ShowAllData { get; private set; }

        public static void LoadStatics()
        {
            Display = Utils.Unity.GetPlayerPrefEnum(DisplayKey, SearchDisplayMode.ShortName);
            Size = PlayerPrefs.GetFloat(SizeKey, MinWidth);
            CurrentEnvirmentFilter = FILTER_ALL;

            m_EnvironmentDataManager = new Environments.EnvironmentDataManager();
            m_EnvironmentDataManager.LoadFromFile();

            m_EnvirmentFilterList.Clear();
            m_EnvirmentFilterList.Add(FILTER_ALL);
            foreach(var enviro in m_EnvironmentDataManager.Environments)
            {
                foreach (var Variant in enviro.Variants)
                {
                    m_EnvirmentFilterList.Add(Variant.CombinedName);
                }              
            }
        }

        public static GUILayoutOption GetWidth() => GUILayout.Width(Size);

        public static void DrawHeaderControls(bool drawSize = true, bool drawDisplay = true)
        {
            using (new LabelWidthScope(96))
            {
                OnSizeGUI(drawSize);
                OnDisplayGUI(drawDisplay);
            }
        }

        private static void OnSizeGUI(bool drawSize)
        {
            if (drawSize == true)
            {
                float size = (int)GUILayout.HorizontalSlider(Size, 0, Size + 1).Max(MinWidth);
                if (Size != size)
                {
                    PlayerPrefs.SetFloat(SizeKey, size);
                    Size = size;
                }
            }
        }

        private static void OnDisplayGUI(bool drawDisplay)
        {
            if (drawDisplay == true)
            {
                CurrentEnvirmentFilter = EditorHelper.Popup(CurrentEnvirmentFilter, m_EnvirmentFilterList);
                CurrentMetaData = EditorHelper.EnumPopup("CurrentMetaData", CurrentMetaData);
                var displayMode = EditorHelper.EnumPopup("Display Mode", Display);
                if (Display != displayMode)
                {
                    PlayerPrefs.SetInt(DisplayKey, (int)displayMode);
                    Display = displayMode;
                }
                ShowAllData = EditorGUILayout.Toggle("ShowAllData", ShowAllData);
            }
        }
    }
}