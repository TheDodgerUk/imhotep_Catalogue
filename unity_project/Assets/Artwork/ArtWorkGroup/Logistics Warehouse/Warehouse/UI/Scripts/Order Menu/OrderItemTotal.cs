using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class OrderItemTotal : MonoBehaviour
{
    private GameObject Amount;
    public int total => m_catalogueItemData.orderedCount;
    public Button minusButton;
    public Image minusColour;

    private CatalogueItemData m_catalogueItemData;
    TextMeshProUGUI amountText;

    // Start is called before the first frame update
    public void Init(CatalogueItemData catalogueItem)
    {
        m_catalogueItemData = catalogueItem;
        m_catalogueItemData.OnOrderCountUpdated += OnOrderCountUpdated;
        Amount = transform.Find("ItemBubble/Amount").gameObject;
        amountText = Amount.GetComponent<TextMeshProUGUI>();

        var addbtn = transform.SearchComponent<Button>("Add");
        minusButton = transform.SearchComponent<Button>("Minus");
        minusButton.gameObject.ForceComponent<OrderMinusEventTrigger>();
        minusColour = transform.SearchComponent<Image>("Minus");

        addbtn.onClick.AddListener(() => Add());
        minusButton.onClick.AddListener(() => Minus());

        OnOrderCountUpdated();
    }

    private void OnDestroy()
    {
        m_catalogueItemData.OnOrderCountUpdated -= OnOrderCountUpdated;
    }

    private void OnOrderCountUpdated()
    {
        amountText.text = total.ToString();

        if (total <= 1)
        {
            minusButton.interactable = false;
            minusColour.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        if (total > 1)
        {
            minusButton.interactable = true;
            minusColour.color = new Color(0.93f, 0.68f, 0.21f, 1f);
        }
    }

    public void Add()
    {
        m_catalogueItemData.orderedCount += 1;    
    }

    public void Minus()
    {
        if (m_catalogueItemData.orderedCount > 1)
        {
            m_catalogueItemData.orderedCount -= 1;
        }
    }
}
