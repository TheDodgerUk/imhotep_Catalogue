using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class FilterData
{
    public List<string> Filters = new List<string>();
}

public class SetupFilters : MonoBehaviour
{
    public Action<List<string>> OnFiltersUpdated;
    public bool isopen = false;

    private GameObject filters;
    private Animator filtersAni;
    private AnimatorPlayer m_AnimatorPlayer;
    private GameObject backgroundBack;
    Button closeButton;
    Button filtersButton;

    private FilterList m_FilterList;
    private Transform m_FilterCache;
    
    public void Init(Dictionary<string, GameObject> content)
    {
        m_FilterCache = new GameObject("FilterCache").transform;
        m_FilterCache.SetParent(transform);
        m_FilterCache.transform.localScale = Vector3.one;
        m_FilterCache.transform.localPosition = Vector3.zero;
        m_FilterCache.transform.localRotation = Quaternion.identity;

        filters = transform.Search("Filters").gameObject;
        m_FilterList = filters.ForceComponent<FilterList>();
        m_FilterList.Init(m_FilterCache, content);
        m_FilterList.OnFiltersUpdated += (e) => OnFiltersUpdated?.Invoke(e);

        backgroundBack = transform.parent.Search("BackgroundBack").gameObject;
        filtersButton = transform.parent.Search("FilterIcon").GetComponent<Button>();
        filtersButton.onClick.AddListener(()=> ToggleFilters(true));

        filters.SetActive(true);
        filtersAni = filters.GetComponent<Animator>();
        m_AnimatorPlayer = filters.ForceComponent<AnimatorPlayer>();

        Transform close = filters.transform.Find("Header/Close");
        closeButton = close.GetComponent<Button>();
        closeButton.onClick.AddListener(()=> ToggleFilters(false));

        ToggleFilters(false);
    }

    [InspectorButton]
    public void ToggleFilters(bool bShow)
    {
        isopen = bShow; 

        if (isopen == true)
        {
            //bool isOpen = filtersAni.GetBool("Open");
            m_AnimatorPlayer.Play("OpenFilters");
            closeButton.interactable = true;
            filtersButton.interactable = false;
        }
        if (isopen == false)
        {
            //bool isOpen = filtersAni.GetBool("Open");
            m_AnimatorPlayer.Play("CloseFilters");
            closeButton.interactable = false;
            filtersButton.interactable = true;
        }
    }
}