using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FilterGroup : MonoBehaviour
{
    public Action OnFilterGroupUpdated;
    public Action OnFilterUpdated;
    private string m_sGroupName;
    private TextMeshProUGUI title;
    private List<string> m_Filters;
    private List<Filter> m_FilterList = new List<Filter>();

    private List<string> m_ActiveFilters = new List<string>();
    public List<string> GetActiveFilters => m_ActiveFilters;

    [SerializeField] private bool bToggle = false;
    public bool Selected => bToggle;

    private GameObject thread;
    private RectTransform threadRect;
    private GameObject active;

    public bool ShouldShowGroup()
    {
        //If any of the filters in this group are active... show
        for (int i = 0; i < m_FilterList.Count; i++)
        {
            if (true == m_FilterList[i].Selected)
            {
                return true;
            }
        }

        return false;
    }

    public void Init(string name, FilterData data, Dictionary<string, GameObject> content)
    {
        m_sGroupName = name;
        m_Filters = data.Filters;
        title = GetComponentInChildren<TextMeshProUGUI>();
        title.text = m_sGroupName;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => OnClick());

        thread = transform.Search("Thread").gameObject;
        threadRect = thread.GetComponent<RectTransform>();

        active = transform.Search("Active").gameObject;
        active.SetActive(false);

        CreateFilter(m_Filters, content);
    }

    private void CreateFilter(List<string> filters, Dictionary<string, GameObject> content)
    {
        foreach (string filter in filters)
        {
            var filterObj = GameObject.Instantiate(content["FilterItem"]);
            filterObj.transform.SetParent(transform);
            filterObj.transform.localScale = Vector3.one;
            filterObj.transform.localPosition = Vector3.zero;
            filterObj.transform.localRotation = Quaternion.identity;
            filterObj.SetActive(true);

            var filterScript = filterObj.ForceComponent<Filter>();
            filterScript.Init(filter);
            int iIndex = m_FilterList.Count;
            filterScript.OnFilterStateChange = () => OnFilterClicked(iIndex);
            m_FilterList.Add(filterScript);
        }
    }

    [InspectorButton]
    private void OnClick()
    {
        bToggle = !bToggle;
        OnFilterGroupUpdated.Invoke();
    }


    private void OnFilterClicked(int iIndex)
    {
        //Update which filters are updated
        Filter filter = m_FilterList[iIndex];
        string filterName = $"{m_sGroupName}/{filter.FilterName}";
        if (true == filter.Selected)
        {
            if (false == m_ActiveFilters.Contains(filterName))
            {
                m_ActiveFilters.Add(filterName);
            }
        }
        else
        {
            m_ActiveFilters.Remove(filterName);
        }

        active.SetActive(ShouldShowGroup());
        OnFilterUpdated?.Invoke();
    }

    public void SetPosition(ref int index, Transform filterList)
    {
        transform.SetParent(filterList);
        transform.position = filterList.TransformPoint(new Vector3(-120, (index * FilterList.filterPadding) + FilterList.offset, 0));
        index++;
        if (true == bToggle)
        {
            foreach (Filter filter in m_FilterList)
            {
                filter.SetPosition(ref index, filterList);
                filter.SetActive(true);
            }
        }
        else
        {
            foreach (Filter filter in m_FilterList)
            {
                filter.SetActive(false);
            }
        }
        UpdateThread(index);
    }

    public void UpdateThread(int total)
    {
        if (true == bToggle)
        {
            int i = 1;
            foreach (Filter filter in m_FilterList)
            {
                i++;
            }
            threadRect.SetHeight((48 * i) - (48));
        }
        else
        {
            threadRect.SetHeight(0);
        }
    }

    public int GetCount()
    {
        int iCount = 1;

        if (true == Selected)
        {
            iCount += m_FilterList.Count;
        }

        return iCount;
    }

}


