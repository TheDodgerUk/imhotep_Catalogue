using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CatalogueSetup : MonoBehaviour
{
    [SerializeField]
    private List<CatalogueItemData> m_CatalogueData = new List<CatalogueItemData>();
    public List<CatalogueItem> m_CatalogueItems = new List<CatalogueItem>();

    private int m_iCacheCount = 25;
    private List<CatalogueItem> m_CatalogueCache = new List<CatalogueItem>();
    private Transform m_CacheParent;

    private GameObject m_Content;
    private Transform m_ContentParent;

    [SerializeField] float widthSpace = 235;
    [SerializeField] float heightSpace = 285f;
    [SerializeField] int gridXAmount = 4;
    [SerializeField] Vector3 startPos = new Vector3(-350f, -220f, 0);

    private RectTransform ListRect;
    private RectTransform ContentBackground;

    private List<string> m_CurrentFilters = new List<string>();
    private OrderList orderList;

    public Action<CatalogueItemData> OnViewInfo;

    private ScrollRect m_ScrollRect;
    [SerializeField]
    private int m_fLastScrollHeight;
    [SerializeField]
    private float m_fDistanceValue = 0.4f;

    [SerializeField]
    private int m_iLastMinScrollIndex = 0;
    [SerializeField]
    private int m_iLastMaxScrollIndex;

    [SerializeField]
    private float m_fMinUpdateDistance = 0.05f;

    public void Init(Dictionary<string, GameObject> content)
    {
        GenerateCatalogueItemData();

        m_Content = content["CatalogueItem"];
        m_ContentParent = transform.Find("Catalogue/Catalogue/ScrollArea/CatalogueMask/CatalogueList");

        ListRect = transform.Find("Catalogue/Catalogue/ScrollArea/CatalogueMask/CatalogueList").gameObject.GetComponent<RectTransform>();
        ContentBackground = transform.Find("Catalogue/Catalogue/ScrollArea/CatalogueMask/CatalogueList/ContentBackground").gameObject.GetComponent<RectTransform>();

        orderList = transform.Find("Catalogue/OrdersMenu/Orders/ScrollArea/OrdersMask/OrderList").gameObject.GetComponent<OrderList>();

        m_ScrollRect = transform.SearchComponent<ScrollRect>("Catalogue/ScrollArea");
        m_ScrollRect.onValueChanged.AddListener((e) => OnScrollBarMoved(e, false));
        m_fLastScrollHeight = -1;

        m_iLastMinScrollIndex = 0;
        m_iLastMaxScrollIndex = m_CatalogueData.Count;
        GenerateCatalogueCache();

        RefreshCatalogue();
    }

    void OnScrollBarMoved(Vector2 value, bool bForce = false)
    {
        //From count of catalogue items calculate number of rows and which row the user is currently scrolling near
        int iCatalogueCount = m_CatalogueItems.Count;
        int iRowCount = (int)(iCatalogueCount * 0.25f);
        int scrollRow = (int)(iRowCount * value.y);
        //Flip direction as top of list = max row... top of list is now row 0 and bottom is max row
        scrollRow = Mathf.Abs(scrollRow - iRowCount);

        //If we have scrolled enough for a new row to be potentially visible
        if (scrollRow != m_fLastScrollHeight ||
            bForce == true)
        {
            //Calculate visible rows
            int iStartIndex = Mathf.Max(0, Mathf.Min(scrollRow, scrollRow - 2) * 4);
            int iEndIndex = Mathf.Min(scrollRow + 2, iRowCount) * 4;

            //Include previous visible rows in our iteration to turn them off
            int iStartCount = Mathf.Min(iStartIndex, m_iLastMinScrollIndex);
            int iEndCount = Mathf.Max(iEndIndex, m_iLastMaxScrollIndex);

            //Can iterate at rate of 4 as UI is 4 wide
            for (int i = iStartCount; i <= iEndCount;)
            {
                //Currently iterating through visible rows?
                bool visible = (i >= iStartIndex && i <= iEndIndex);

                for (int x = 0; x < 4; x++)
                {
                    if (i < iCatalogueCount)
                        m_CatalogueItems[i].Show(visible);

                    i++;
                }
            }

            m_iLastMinScrollIndex = iStartIndex;
            m_iLastMaxScrollIndex = iEndIndex;
            m_fLastScrollHeight = scrollRow;
        }
    }

    private void GenerateCatalogueCache()
    {
        m_CacheParent = new GameObject("CatalogueCache").transform;
        m_CacheParent.SetParent(transform);
        for (int i = 0; i < m_iCacheCount; i++)
        {
            m_CatalogueCache.Add(GenerateCatalogueItem());
        }
    }

    private CatalogueItem GenerateCatalogueItem()
    {
        GameObject catalogueContent = Instantiate(m_Content);

        catalogueContent.transform.SetParent(m_CacheParent);
        catalogueContent.transform.localScale = Vector3.one * 0.75f;
        catalogueContent.transform.localRotation = Quaternion.identity;

        //Add catalogue item class to handle the catalogue entry
        var catalogueItem = catalogueContent.ForceComponent<CatalogueItem>();
        catalogueItem.Init();
        catalogueItem.gameObject.SetActive(true);
        catalogueItem.Show(false);
        return catalogueItem;
    }

    private CatalogueItem GetNext()
    {
        if (m_CatalogueCache.Count > 0)
        {
            var item = m_CatalogueCache[0];
            m_CatalogueCache.Remove(item);
            return item;
        }

        //Generate new item
        return GenerateCatalogueItem();
    }

    private void ReturnCatalogueItem(CatalogueItem item)
    {
        item.DeInit();
        m_CatalogueItems.Remove(item);
        m_CatalogueCache.Add(item);
        item.Show(false);
        //Reparent
        item.transform.SetParent(m_CacheParent);
        item.OnViewInfo = null;
    }

    public void OnFilterChange(List<string> filterList)
    {
        m_CurrentFilters = filterList;

        //Refresh
        RefreshCatalogue();
    }

    private void OnCatalogueEntryClicked(CatalogueItemData catalogueItem)
    {
        orderList.CreateNewPanel(catalogueItem);
    }

    private void Update()
    {
        //  PositionCatalogue();
        //   UpdateScroll();
    }

    private void PositionCatalogue()
    {
        for (int i = 0; i < m_CatalogueItems.Count; i++)
        {
            int y = i / gridXAmount;
            int x = i % gridXAmount;

            Vector3 offset = new Vector3(x * widthSpace, -y * heightSpace, 0);
            m_CatalogueItems[i].transform.localPosition = startPos + offset;
        }
    }

    private void ResetCatalogue()
    {
        while (m_CatalogueItems.Count > 0)
        {
            var item = m_CatalogueItems[0];
            ReturnCatalogueItem(item);
        }
    }

    private void RefreshCatalogue()
    {
        if (null == m_CatalogueData)
            return;

        ResetCatalogue();

        for (int i = 0; i < m_CatalogueData.Count; i++)
        {
            var data = m_CatalogueData[i];
            //If we have no filters active... or filters match this catalogue item
            if (m_CurrentFilters.Count == 0 ||
                true == data.MatchFilters(m_CurrentFilters))
            {
                var item = GetNext();

                item.transform.SetParent(m_ContentParent);
                item.transform.localScale = Vector3.one * 0.75f;
                item.transform.localRotation = Quaternion.identity;

                //Initialise with data
                item.Show(false);
                item.Init(this);
                item.SetData(data);
                data.OnOrderCountUpdated += () => OnCatalogueItemCountChanged(data);

                item.OnViewInfo += (e) => OnViewInfo?.Invoke(e);

                m_CatalogueItems.Add(item);

                data.OnOrderCountUpdated?.Invoke();
            }
        }

        UpdateScroll();
        PositionCatalogue();
        OnScrollBarMoved(Vector2.zero, true);
    }

    private void OnCatalogueItemCountChanged(CatalogueItemData e)
    {
        if (e.orderedCount == 1)
        {
            //Has been added
            OnCatalogueEntryClicked(e);
        }
        else if (e.orderedCount == 0)
        {
            orderList.OnRemoveOrder(e);
        }
    }

    private void UpdateScroll()
    {
        ListRect.sizeDelta = new Vector2(1000, ListClamp());
        ContentBackground.sizeDelta = new Vector2(1000, BackgroundClamp());
    }

    private float ListClamp()
    {
        int i = m_CatalogueItems.Count;
        float height = ListRect.rect.height;
        float rowAmount = Mathf.CeilToInt((i / (float)gridXAmount));
        float rowHeight = heightSpace;
        float newRowHeight = 25 + (rowAmount * rowHeight) + 150;

        if (newRowHeight < 730)
        {
            newRowHeight = 730;
            return newRowHeight;
        }
        else
        {
            return newRowHeight;
        }
    }

    private float BackgroundClamp()
    {
        float rowAmount = Mathf.CeilToInt((float)(m_CatalogueItems.Count / (float)gridXAmount));
        float rowHeight = heightSpace;
        float newRowHeight = 50 + (rowAmount * rowHeight);

        if (newRowHeight < 620)
        {
            newRowHeight = 620;
            return newRowHeight;
        }
        else
        {
            return newRowHeight;
        }
    }

    private void GenerateCatalogueItemData()
    {
        m_CatalogueData.Clear();


        m_CatalogueData = new List<CatalogueItemData>();

        for (int i = 0; i < Catalogue.Instance.Entries.Count; i++)
        {
            CatalogueEntry entry = Catalogue.Instance.Entries[i];
            m_CatalogueData.Add(GenerateCatalogueEntry(entry));
        }
    }

    private CatalogueItemData GenerateCatalogueEntry(CatalogueEntry entry)
    {
        CatalogueItemData data = new CatalogueItemData();

        data.Init(entry.CatalogueFilters);
        data.sName = entry.ShortName.ToString();
        data.sDescription = entry.FullName.ToString();
        Texture2D image = null;
#if UNITY_EDITOR
        image = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Artwork/CatalogueThumbnails/{entry.Guid}.png");
#endif
        data.Image = image;
        data.GUID = entry.Guid;
        //Generate data                
        data.OnOrderCountUpdated += () => OnCatalogueItemCountChanged(data);

        return data;
    }

    private CatalogueItemData GenerateCatalogueEntry(string shortName)
    {
        CatalogueItemData data = new CatalogueItemData();
        data.Init(new List<string>());
        data.sName = shortName;
        data.sUIImageName = "ImageNotFound";

        return data;
    }
}