using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class SetupProduct : MonoBehaviour
{
    public bool isopen = false;

    private GameObject product;
    private Animator productAni;
    private GameObject productBack;
    Button productBackButton;
    Button closeButton;
    TextMeshProUGUI title;
    RawImage objectImage;
    TextMeshProUGUI about;

    GameObject total;
    ProductItemTotal itemTotal;

    public void Init()
    {
        product = transform.Search("Product").gameObject;
        title = product.transform.SearchComponent<TextMeshProUGUI>("TitleText");
        objectImage = product.transform.SearchComponent<RawImage>("ObjImage");
        about = product.transform.SearchComponent<TextMeshProUGUI>("AboutText");

        total = product.transform.Search("Header/Total").gameObject;
        itemTotal = total.ForceComponent<ProductItemTotal>();


        productBack = transform.Search("ProductBack").gameObject;
        productBackButton = productBack.GetComponent<Button>();
        productBackButton.onClick.AddListener(() => ToggleProduct(false));

        product.SetActive(true);
        productAni = product.GetComponent<Animator>();

        Transform close = product.transform.Find("Header/Close");
        closeButton = close.GetComponent<Button>();
        closeButton.onClick.AddListener( () =>ToggleProduct(false));

        itemTotal.Init();
    }

    public void OnViewInfo(CatalogueItemData data)
    {
        ToggleProduct(true);
        itemTotal.Init(data);
        title.text = data.sName;
        objectImage.texture = data.Image;
        about.text = data.sDescription;
    }

    [InspectorButton]
    public void ToggleProduct(bool show)
    {
        isopen = show; 

        if (isopen == true)
        {
            bool isOpen = productAni.GetBool("Open");
            productBack.SetActive(true);
            productAni.SetBool("Open", true);
            closeButton.interactable = true;
            //moreinfoButton.interactable = false;
        }
        if (isopen == false)
        {
            bool isOpen = productAni.GetBool("Open");
            productBack.SetActive(false);
            productAni.SetBool("Open", false);
            closeButton.interactable = false;
            //moreinfoButton.interactable = true;
        }
    }
}