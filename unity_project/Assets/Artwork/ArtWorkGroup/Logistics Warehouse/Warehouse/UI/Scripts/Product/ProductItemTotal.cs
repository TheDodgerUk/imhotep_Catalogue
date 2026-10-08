using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ProductItemTotal : MonoBehaviour
{
    private GameObject amount;
    private GameObject amountBubble;
    public int total => m_catalogueItemData.orderedCount;
    public Button addbtn;
    public Button minusButton;
    public Image minusColour;
    TextMeshProUGUI amountText;
    Animator aniAmountBubble;
    AnimatorPlayer m_BubbleAnimator;

    public Action onCatalogueUpdateCount;
    private CatalogueItemData m_catalogueItemData;

    private bool m_bLastValue = false;

    public void DeInit()
    {
        m_catalogueItemData.OnOrderCountUpdated -= OnOrderCountUpdated;
        m_catalogueItemData = null;

        addbtn.onClick.RemoveAllListeners();
        minusButton.onClick.RemoveAllListeners();
    }

    public void Init()
    {
        amount = transform.Find("ItemBubble/Amount").gameObject;
        amountText = amount.GetComponent<TextMeshProUGUI>();
        amountBubble = transform.Find("ItemBubble").gameObject;
        m_BubbleAnimator = amountBubble.ForceComponent<AnimatorPlayer>();
        aniAmountBubble = amountBubble.GetComponent<Animator>();

        addbtn = transform.SearchComponent<Button>("Add");
        minusButton = transform.SearchComponent<Button>("Minus");
        var eventTrigger = minusButton.gameObject.ForceComponent<OrderMinusEventTrigger>();
        eventTrigger.Init();
        minusColour = transform.SearchComponent<Image>("Minus");
        SetMinusButton(false, true);
        addbtn.onClick.AddListener(() => Add());
        minusButton.onClick.AddListener(() => Minus());
    }

    public void Init(CatalogueItemData catalogueItem)
    {
        m_catalogueItemData = catalogueItem;
        m_catalogueItemData.OnOrderCountUpdated += OnOrderCountUpdated;

        onCatalogueUpdateCount?.Invoke();

        amountText.text = total.ToString();
        if (total >= 1)
        {
            minusButton.interactable = true;
            minusColour.color = new Color(0.93f, 0.68f, 0.21f, 1f);
        }
        else
        {
            minusButton.interactable = false;
            minusColour.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    private void OnOrderCountUpdated()
    {
        amountText.text = total.ToString();

        if (total == 1)
        {
            minusButton.interactable = true;
            minusColour.color = new Color(0.93f, 0.68f, 0.21f, 1f);
            //aniAmountBubble.Play("Open");
        }

        SetMinusButton(total > 0);
    }

    private void SetMinusButton(bool bValue, bool bForce = false)
    {
        if (bValue == m_bLastValue && false == bForce)
            return;

        if(true == bValue)
        {
            minusButton.interactable = true;
            minusColour.color = new Color(0.93f, 0.68f, 0.21f, 1f);
            m_BubbleAnimator.Play("Add");
        }
        else
        {
            minusButton.interactable = false;
            minusColour.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            m_BubbleAnimator.Play("Close");
        }

        m_bLastValue = bValue;
    }

    public void Add()
    {
        m_catalogueItemData.orderedCount += 1;
        onCatalogueUpdateCount?.Invoke();
    }

    public void Minus()
    {
        m_catalogueItemData.orderedCount -= 1;
        onCatalogueUpdateCount?.Invoke();
    }
}
