using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderMenuSetup : MonoBehaviour
{
    private OrderList m_OrderList;
    public OrderList OrderList => m_OrderList;

    [SerializeField] private Transform OrderListRoot;
    [SerializeField] private Transform EmptyRoot;
    [SerializeField] private Transform ProgressRoot;
    [SerializeField] private Transform ProgCompRoot;
    [SerializeField] private Transform HomeBubbleRoot;
    [SerializeField] private Transform OrderBubbleRoot;
    [SerializeField] private Transform CompleteRoot;
    [SerializeField] private Transform OrdersRoot;
    [SerializeField] private Transform BackgroundBack;
    [SerializeField] private Transform MenuCentre;
    [SerializeField] private Transform orderMenu;

    [SerializeField]
    private OpenOrders m_OpenOrders;

    // Start is called before the first frame update
    public void Init(Dictionary<string, GameObject> content)
    {
        orderMenu = transform.Find("Catalogue/OrdersMenu");
        OrdersRoot = transform.Search("Orders");
        BackgroundBack = transform.Search("BackgroundBack");
        m_OpenOrders = gameObject.ForceComponent<OpenOrders>();

        var catalogue = transform.Search("Catalogue");
          //catalogue.gameObject.ForceComponent<VRUICanvas>();

        var basketGo = transform.Search("Basket");
        m_OpenOrders.Init(OrdersRoot.gameObject, BackgroundBack.gameObject, basketGo.gameObject);

        EmptyRoot = transform.Search("Empty");
        ProgressRoot = transform.Search("OrderProgress");
        ProgCompRoot = transform.Search("OrderComplete");
        HomeBubbleRoot = transform.Search("HomeBubble");
        OrderBubbleRoot = transform.Search("OrderBubble");
        CompleteRoot = transform.Search("Complete");
        OrderListRoot = transform.Search("OrderList");
        MenuCentre = OrdersRoot.Search("Background");
        m_OrderList = OrderListRoot.gameObject.ForceComponent<OrderList>();
        m_OrderList.Init(content["OrderedItem"], -150f, -120f, orderMenu, EmptyRoot, ProgressRoot, ProgCompRoot, HomeBubbleRoot.gameObject, OrderBubbleRoot.gameObject, CompleteRoot.gameObject, MenuCentre.gameObject);
    }
}