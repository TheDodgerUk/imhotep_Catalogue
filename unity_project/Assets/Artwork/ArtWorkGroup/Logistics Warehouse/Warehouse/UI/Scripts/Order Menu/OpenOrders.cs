using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenOrders : MonoBehaviour
{
    public bool isopen = false;
    private GameObject orders;
    private Animator orderAni;
    private GameObject backgroundBack;
    Button closeButton;
    Button basketButton;

    private AnimatorPlayer m_AnimatorPlayer;
    
    public void Init(GameObject order, GameObject backButton, GameObject basket)
    {
        orders = order;
        backgroundBack = backButton;
        basketButton = basket.GetComponent<Button>();
        basketButton.onClick.AddListener(()=> ToggleOrders());

        orders.SetActive(true);
        orderAni = orders.GetComponent<Animator>();
        m_AnimatorPlayer = orders.ForceComponent<AnimatorPlayer>();

        Animator filtersAni = transform.Search("Filters").GetComponent<Animator>();

        Transform close = orders.transform.Find("Header/Close");
        closeButton = close.GetComponent<Button>();
        closeButton.onClick.AddListener(ToggleOrders);

        Button backbutton = backgroundBack.GetComponent<Button>();
        backbutton.onClick.AddListener(ToggleOrders);
    }

    public void ToggleOrders()
    {
        isopen = !isopen;

        if (isopen == true)
        {
            //bool isOpen = orderAni.GetBool("Open");
            m_AnimatorPlayer.Play("OpenOrders");
            backgroundBack.SetActive(true);
            closeButton.interactable = true;
            basketButton.interactable = false;
        }
        if (isopen == false)
        {
            m_AnimatorPlayer.Play("CloseOrders");
            backgroundBack.SetActive(false);
            closeButton.interactable = false;
            basketButton.interactable = true;
        }
    }


    [InspectorButton]
    public void ToggleShowOrders()
    {
        ToggleOrders();
    }
}