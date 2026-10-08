using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FilterList : MonoBehaviour
{
    public Action<List<string>> OnFiltersUpdated;
    public const float filterPadding = -48;
    public const float offset = -127;

    private string FilterFolder = Catalogue.CatalogueFiltersFolder;
    private Dictionary<string, FilterData> m_DictOfFilters = new Dictionary<string, FilterData>();
    private List<FilterGroup> m_FilterGroups = new List<FilterGroup>();
    private List<string> m_ActiveFilters = new List<string>();

    private Transform filterList;
    private ScrollHeight m_ScrollHeight;
    private GameObject backgroundMask;
    private GameObject thread;
    private RectTransform threadRect;

    public void Init(Transform cacheParent, Dictionary<string, GameObject> content)
    {
        //Load all data for the filters
        LoadFilters();
        filterList = transform.Search("FilterList");
        m_ScrollHeight = filterList.gameObject.ForceComponent<ScrollHeight>();
        backgroundMask = filterList.Search("ContentBackground").gameObject;
        m_ScrollHeight.Init(backgroundMask, filterPadding);
        thread = transform.Search("Thread").gameObject;
        threadRect = thread.GetComponent<RectTransform>();

        //Create visuals for all filters
        CreateFilters(cacheParent, content);
        UpdateFilters();
    }

    private void CreateFilters(Transform cacheParent, Dictionary<string, GameObject> content)
    {
        foreach (string key in m_DictOfFilters.Keys)
        {
            //Create temp object... will load actual graphics eventually
            var filterGroup = GameObject.Instantiate(content["FilterGroup"]);
            filterGroup.transform.SetParent(cacheParent);
            filterGroup.transform.localScale = Vector3.one;
            filterGroup.transform.localPosition = Vector3.zero;
            filterGroup.transform.localRotation = Quaternion.identity;
            filterGroup.SetActive(true);

            //Setup filter group
            var fg = filterGroup.ForceComponent<FilterGroup>();
            fg.OnFilterGroupUpdated = UpdateFilters;
            fg.OnFilterUpdated = ProcessFilters;
            fg.Init(key, m_DictOfFilters[key], content);
            m_FilterGroups.Add(fg);
        }
    }

    private void LoadFilters()
    {
        //Load all filter files
        string[] files = System.IO.Directory.GetFiles(FilterFolder, "*json");

        for (int i = 0; i < files.Length; i++)
        {
            string fileName = Utils.IO.GetFileName(files[i]);
            FilterData data = Json.JsonNet.ReadFromFile<FilterData>($"{FilterFolder}/{fileName}");
            if (null != data)
            {
                if (false == m_DictOfFilters.ContainsKey(fileName))
                {
                    m_DictOfFilters.Add(fileName, data);
                }
            }
        }
    }

    public void UpdateThread(int total)
    {
        threadRect.SetHeight((48 * total) - 18);
    }

    private void ProcessFilters()
    {
        m_ActiveFilters.Clear();
        foreach (FilterGroup group in m_FilterGroups)
        {
            m_ActiveFilters.AddRange(group.GetActiveFilters);
        }

        OnFiltersUpdated?.Invoke(m_ActiveFilters);
    }

    public void UpdateFilters()
    {
        int i = 0;
        int totalActive = 0;
        foreach (FilterGroup group in m_FilterGroups)
        {
            totalActive += group.GetCount();
        }

        UpdateThread(totalActive);

        m_ScrollHeight.UpdateScroll(totalActive);

        foreach (FilterGroup group in m_FilterGroups)
        {
            group.SetPosition(ref i, filterList);
        }

    }
}


