using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BrandEditorWindow : EditorWindow
{
    private bool m_bAwakeRun = false;

    [MenuItem("Window/Custom/Brand Editor", false, 2)]
    public static void ShowWindow() => GetWindow<BrandEditorWindow>("Brand Editor");

    private BrandData brandData = new BrandData();
    private BrandInfo m_SelectedBrand;

    private int LogoSize = 150;
    private string[] brands;
    private int iSelectedBrand = 0;
    private string sBrandFolder;

    private Texture m_LogoTexture;

    private void Awake()
    {
        ReloadAll();
        m_bAwakeRun = true;
    }

    private void OnGUI()
    {
        if(false == m_bAwakeRun)
        {
            return;
        }

        EditorGUILayout.LabelField("Select Brand");

        int selection = EditorGUILayout.Popup(iSelectedBrand, brands);
        if (selection != iSelectedBrand)
        {
            OnChangeBrand(selection);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Save"))
        {
            SaveAll();
        }

        if (GUILayout.Button("Reload"))
        {
            ReloadAll();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (null != m_SelectedBrand)
        {
            ViewBrandInfo(m_SelectedBrand);
        }
    }

    private void SaveAll()
    {
        Json.Unity.WriteToFile(brandData, $"{sBrandFolder}/BrandData", true);
    }

    private void ReloadAll()
    {
        sBrandFolder = GlobalConsts.BUNDLE_STREAMING_BRANDS;
        Utils.IO.ForceFolder(sBrandFolder);
        brandData = Json.Unity.ReadFromFile<BrandData>($"{sBrandFolder}/BrandData");

        brands = new string[brandData.BrandInfo.Count];
        for (int i = 0; i < brandData.BrandInfo.Count; i++)
        {
            brands[i] = brandData.BrandInfo[i].BrandName;
        }

        OnChangeBrand(0);
    }

    private void OnChangeBrand(int iSelection)
    {
        m_SelectedBrand = brandData.BrandInfo[iSelection];
        iSelectedBrand = iSelection;

        m_LogoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Artwork/BrandLogos/{m_SelectedBrand.BrandName}.png") as Texture;
    }

    private void ViewBrandInfo(BrandInfo brandInfo)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Box(m_LogoTexture, GUILayout.Width(LogoSize), GUILayout.Height(LogoSize));

        GUILayout.BeginVertical();
        EditorGUILayout.Space();

        GUILayout.Label(brandInfo.BrandName);
        brandInfo.EstYear = EditorGUILayout.IntField("Year Est", brandInfo.EstYear);
        brandInfo.CountryLocation = EditorGUILayout.TextField("Country", brandInfo.CountryLocation);
        brandInfo.CityLocation = EditorGUILayout.TextField("City", brandInfo.CityLocation);

        EditorGUILayout.LabelField("Brand Description");
        brandInfo.BrandDescription = EditorGUILayout.TextArea(brandInfo.BrandDescription);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.Label("Video Information");

        brandInfo.BrandVideoName = EditorGUILayout.TextField("Logo Video", brandInfo.BrandVideoName);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}