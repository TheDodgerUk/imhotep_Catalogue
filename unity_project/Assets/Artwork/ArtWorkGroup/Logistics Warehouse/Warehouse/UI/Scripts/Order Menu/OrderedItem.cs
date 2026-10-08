using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OrderedItem : MonoBehaviour
{
    private TextMeshProUGUI m_ItemTitle;
    private RawImage m_ObjImage;
    private OrderDeletePanel m_DeleteButton;

    public OrderDeletePanel GetDeleteButton => m_DeleteButton;

    public void SetName(string sName, Texture rawImage)
    {
        m_ItemTitle.text = sName;
        m_ObjImage.texture = rawImage;
    }

    public void Init()
    {
        m_ItemTitle = transform.Find("Background/Content/Text").GetComponent<TextMeshProUGUI>();
        m_ObjImage = transform.Find("Background/Content/ObjImage").GetComponent<RawImage>();

        RectTransform panelAnchor = gameObject.GetComponent<RectTransform>();
        panelAnchor.anchorMin = new Vector2(0.5f, 1);
        panelAnchor.anchorMax = new Vector2(0.5f, 1);

        m_DeleteButton = gameObject.AddComponent<OrderDeletePanel>();
        m_DeleteButton.Init();
    }

    public void Init(Action<GameObject> OnRemove, CatalogueItemData catalogueItem)
    {
        m_DeleteButton.Init((e) =>
        {
            catalogueItem.orderedCount = 0;
        }, catalogueItem);
    }
}
