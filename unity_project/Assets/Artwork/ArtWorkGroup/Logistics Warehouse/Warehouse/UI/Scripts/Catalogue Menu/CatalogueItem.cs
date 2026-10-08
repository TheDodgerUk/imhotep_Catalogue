using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//Data converted from Catalogue Entry to Catalogue Item...
//Data needed to make the UI
[Serializable]
public class CatalogueItemData
{
    public Action OnOrderCountUpdated;
    public string sUIImageName;
    public Texture2D Image;
    public string sName;
    public string sDescription;

    private HashSet<string> m_FilterHash;

    private int m_OrderedCount;
    public int orderedCount
    {
        get { return m_OrderedCount; }
        set
        {
            m_OrderedCount = value;
            OnOrderCountUpdated?.Invoke();
        }
    }

    public bool MatchFilters(List<string> filters)
    {
        //TODO: Contvert from list => HashSet to be more efficient
        for(int i = 0;i < filters.Count; i++)
        {
            if(false == m_FilterHash.Contains(filters[i]))
            {
                return false;
            }
        }

        return true;
    }

    public void Init(List<string> filters)
    {
        m_FilterHash = new HashSet<string>(filters);
    }

    public string GUID;
}

/// <summary>
/// Catalogue item... displayed in the catalogue in the catalogue window
/// This contains images and data about the catalogue item on the UI and 
/// allows the user to select and add catalogue items to the "shopping cart".
/// </summary>
public class CatalogueItem : MonoBehaviour
{
    public Action<CatalogueItemData> OnClicked;
    private RawImage Icon;

    public Action<CatalogueItemData> OnViewInfo;
    private Button m_infoButton;

    private TextMeshProUGUI m_NameText;
    private Button m_Button;
    private Button m_BackgroundButton;

    private GameObject totalGo;
    private GameObject smallAmountBubble;
    private Image minusColour;
    private Button minusButton;
    private CatalogueItemTotal m_CatalogueItemTotal;
    public Action onUpdateCount;
    private CatalogueItemData m_catalogueItemData;
    Animator amountBubble;
    public bool isSelected;
    Button basketButton;
    Animator goBubPos;
    private Canvas m_ItemCanvas;

    private CatalogueSetup m_catatlogueSetup;

    public void Init()
    {
        m_ItemCanvas = gameObject.GetComponent<Canvas>();
        m_Button = transform.SearchComponent<Button>("CatalogueContent");
        m_infoButton = transform.SearchComponent<Button>("InfoButton");

        Icon = transform.SearchComponent<RawImage>("CatalogueImage");
        m_NameText = transform.SearchComponent<TextMeshProUGUI>("Title");

        totalGo = transform.Search("Total").gameObject;
        smallAmountBubble = transform.Search("Position/ItemBubble").gameObject;
        goBubPos = transform.Search("Position").GetComponent<Animator>();

        amountBubble = smallAmountBubble.GetComponent<Animator>();
        minusColour = totalGo.transform.Find("Minus").GetComponent<Image>();
        minusButton = totalGo.transform.Find("Minus").GetComponent<Button>();
        m_CatalogueItemTotal = totalGo.gameObject.ForceComponent<CatalogueItemTotal>();
        m_CatalogueItemTotal.Init();
    }

    public void Init(CatalogueSetup catalogueSetup)
    {
        m_BackgroundButton = transform.parent.SearchComponent<Button>("ContentBackground");
        basketButton = transform.root.SearchComponent<Button>("Basket");
        
        //Get all required components        
        m_CatalogueItemTotal.onCatalogueUpdateCount += () => onUpdateCount?.Invoke();

        m_Button.onClick.AddListener(() => OnButtonPressed());
        m_BackgroundButton.onClick.AddListener(() => OnBackgroundPressed());
        basketButton.onClick.AddListener(OnBackgroundPressed);

        m_catatlogueSetup = catalogueSetup;
    }

    public void DeInit()
    {
        BubbleSelected(false);
        //Shutdown total class
        m_CatalogueItemTotal.DeInit();

        m_catalogueItemData.OnOrderCountUpdated = null;
        m_catatlogueSetup = null;
        m_CatalogueItemTotal.onCatalogueUpdateCount = null;
        m_Button.onClick.RemoveAllListeners();
        m_BackgroundButton.onClick.RemoveAllListeners();
        basketButton.onClick.RemoveListener(OnBackgroundPressed);
    }

    public void Show(bool bShow)
    {
        if (null != m_ItemCanvas)
        {
            m_ItemCanvas.enabled = bShow;
        }
    }

    public void SetData(CatalogueItemData entry)
    {
        //Set data
        m_catalogueItemData = entry;
        m_catalogueItemData.OnOrderCountUpdated += OnOrderCountUpdated;
        m_CatalogueItemTotal.Init(m_catalogueItemData);
        m_infoButton.onClick.AddListener(() => OnViewInfo?.Invoke(m_catalogueItemData));

        if (null != m_catalogueItemData.Image)
        {
            Icon.texture = m_catalogueItemData.Image;
        }
        PopulateData();
    }

    private void PopulateData()
    {
        if (null == m_catalogueItemData)
            return;

        m_NameText.text = m_catalogueItemData.sName;
    }

    private void OnOrderCountUpdated()
    {
        if(m_catalogueItemData.orderedCount > 0)
        {
            minusButton.interactable = true;
            minusColour.color = new Color(0.93f, 0.68f, 0.21f, 1f);
            amountBubble.Play("Open");
        }
    }

    private void OnButtonPressed()
    {
        List<CatalogueItem> catalogueContents = m_catatlogueSetup.m_CatalogueItems;

        foreach (CatalogueItem items in catalogueContents)
        {
            items.BubbleSelected(false);
        }

        if (m_catalogueItemData.orderedCount <= 0)
        {
            m_catalogueItemData.orderedCount += 1;
        }

        if (totalGo.active == false)
        {
            GameObject curObj = totalGo;
            curObj.SetActive(true);

            if (curObj.active == true)
            {
                GameObject curObjBub = curObj.transform.parent.Search("Position/ItemBubble").gameObject;
                Animator curObjBubPos = curObj.transform.parent.Search("Position").GetComponent<Animator>();

                curObjBubPos.SetBool("isActive", true);
                //curObjBub.SetActive(false);
            }
        }
    }

    public void BubbleSelected(bool bSelected)
    {
        totalGo.SetActive(bSelected);
        
        smallAmountBubble.SetActive(!bSelected);
        goBubPos.SetBool("isActive", bSelected);
        
        if (m_CatalogueItemTotal.total > 0)
        {
            amountBubble.Play("Active");
        }
    }

    private void OnBackgroundPressed()
    {
        List<CatalogueItem> catalogueContents = m_catatlogueSetup.m_CatalogueItems;

        foreach (CatalogueItem items in catalogueContents)
        {
            items.BubbleSelected(false);
        }
    }

    [InspectorButton]
    private void Add()
    {
        if (null != m_catalogueItemData)
        {
            //OnClicked?.Invoke(m_catalogueItemData);
            m_catalogueItemData.orderedCount += 1;
        }
    }

    [InspectorButton]
    private void Remove()
    {
        if (null != m_catalogueItemData)
        {
            //OnClicked?.Invoke(m_catalogueItemData);
            m_catalogueItemData.orderedCount -= 1;
        }
    }
}