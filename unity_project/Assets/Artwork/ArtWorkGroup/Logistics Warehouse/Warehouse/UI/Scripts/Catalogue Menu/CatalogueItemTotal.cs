using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CatalogueItemTotal : MonoBehaviour
{
    private GameObject amount;
    private GameObject smallAmount;
    private GameObject smallAmountBubble;
    public int total => m_catalogueItemData.orderedCount;
    private Button addbtn;
    public Button minusButton;
    public Image minusColour;
    TextMeshProUGUI amountText;
    TextMeshProUGUI smallAmountText;
    Animator amountBubble;
    AnimatorPlayer m_BubbleAnimator;

    public Action onCatalogueUpdateCount;
    private CatalogueItemData m_catalogueItemData;

    private bool m_bLastValue = false;

    public void Init()
    {
        amount = transform.Find("BigItemBubble/Amount").gameObject;
        amountText = amount.GetComponent<TextMeshProUGUI>();

        smallAmount = transform.parent.Find("Position/ItemBubble/Amount").gameObject;
        smallAmountText = smallAmount.GetComponent<TextMeshProUGUI>();
        smallAmountBubble = transform.parent.Find("Position/ItemBubble").gameObject;
        m_BubbleAnimator = smallAmountBubble.ForceComponent<AnimatorPlayer>();
        amountBubble = smallAmountBubble.GetComponent<Animator>();

        addbtn = transform.SearchComponent<Button>("Add");
        minusButton = transform.SearchComponent<Button>("Minus");
        var eventTrigger = minusButton.gameObject.ForceComponent<OrderMinusEventTrigger>();
        eventTrigger.Init();
        minusColour = transform.SearchComponent<Image>("Minus");
    }

    public void DeInit()
    {
        m_catalogueItemData.OnOrderCountUpdated -= OnOrderCountUpdated;
        m_catalogueItemData = null;

        addbtn.onClick.RemoveAllListeners();
        minusButton.onClick.RemoveAllListeners();
    }

    public void Init(CatalogueItemData catalogueItem)
    {
        m_catalogueItemData = catalogueItem;
        m_catalogueItemData.OnOrderCountUpdated += OnOrderCountUpdated;        

        addbtn.onClick.AddListener(() => Add());
        minusButton.onClick.AddListener(() => Minus());
        onCatalogueUpdateCount?.Invoke();

        SetMinusButton(false, true);
    }

    private void OnDestroy()
    {
        if (null != m_catalogueItemData)
        {
            m_catalogueItemData.OnOrderCountUpdated -= OnOrderCountUpdated;
        }
    }

    private void OnOrderCountUpdated()
    {
        amountText.text = total.ToString();
        smallAmountText.text = total.ToString();

        if (total == 1)
        {
            minusButton.interactable = true;
            minusColour.color = new Color(0.93f, 0.68f, 0.21f, 1f);
            amountBubble.Play("Open");
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
            m_BubbleAnimator.Play("Closed");
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
