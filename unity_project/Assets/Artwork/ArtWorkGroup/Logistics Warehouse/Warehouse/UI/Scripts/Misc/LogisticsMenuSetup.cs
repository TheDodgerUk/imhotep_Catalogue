using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LogisticsMenuSetup : MonoBehaviour
{
    //public Action<ModuleObject> OnCatalogueObjectSpawned;
    private Dictionary<string, GameObject> m_ContentTypes = new Dictionary<string, GameObject>();

    [SerializeField]
    private OrderMenuSetup m_OrderMenu;
    [SerializeField]
    private CatalogueSetup m_CatalogueMenu;
    [SerializeField]
    private SetupFilters m_SetupFilters;
    [SerializeField]
    private SetupProduct m_SetupProduct;
    [SerializeField]
    private LoadingScreen m_LoadingScreen;

    private Vector3 m_SpawnPosition = Vector3.zero;

    private void Awake()
    {
        Init(Vector3.zero);
    }

    // Start is called before the first frame update
    public void Init(Vector3 spawnPosition)
    {
        m_SpawnPosition = spawnPosition;

        GetContentTypes();
        SetupOrderList();
        SetupCatalogue();
        SetupFilters();
        SetupProduct();
        SetupLoading();
    }

    private void SetupProduct()
    {
        GameObject product = transform.Find("Catalogue/ProductMenu").gameObject;
        m_SetupProduct = product.ForceComponent<SetupProduct>();
        m_SetupProduct.Init();
        m_CatalogueMenu.OnViewInfo += (e) => m_SetupProduct.OnViewInfo(e);
    }

    private void SetupFilters()
    {
        GameObject filter = transform.Find("Catalogue/FiltersMenu").gameObject;
        m_SetupFilters = filter.ForceComponent<SetupFilters>();
        m_SetupFilters.Init(m_ContentTypes);

        m_SetupFilters.OnFiltersUpdated += (e) => m_CatalogueMenu.OnFilterChange(e);
    }

    private void SetupOrderList()
    {
        m_OrderMenu = gameObject.ForceComponent<OrderMenuSetup>();
        m_OrderMenu.Init(m_ContentTypes);
        m_OrderMenu.OrderList.OnProcessPurchase += ProcessOrders;
    }

    private void SetupCatalogue()
    {
        m_CatalogueMenu = gameObject.ForceComponent<CatalogueSetup>();
        m_CatalogueMenu.Init(m_ContentTypes);
    }

    private void SetupLoading()
    {
        var loadingScreen = transform.Find("LoadingScreen");
        m_LoadingScreen = loadingScreen.gameObject.ForceComponent<LoadingScreen>();
        m_LoadingScreen.Init();
    }

    private void GetContentTypes()
    {
        Transform contentHolder = transform.Find("ContentTypes");

        List<Transform> children = contentHolder.GetDirectChildren();

        foreach (var child in children)
        {
            if (false == m_ContentTypes.ContainsKey(child.name))
            {
                m_ContentTypes.Add(child.name, null);
            }

            m_ContentTypes[child.name] = child.gameObject;

            child.gameObject.SetActive(false);
        }
    }

    private void ProcessOrders(List<CatalogueItemData> orderList)
    {
        for (int i = 0; i < orderList.Count; i++)
        {
            CreateCatalogueItem(orderList[i]);
            orderList[i].orderedCount = 0;
        }
    }

    private void CreateCatalogueItem(CatalogueItemData data)
    {
        //Core.Scene.SpawnObject(data.GUID, (e) =>
        //{
        //    Vector2 offset = UnityEngine.Random.insideUnitCircle * 3f;
        //    e.transform.position = m_SpawnPosition + new Vector3(offset.x, 0f, offset.y);

        //    OnCatalogueObjectSpawned?.Invoke(e);
        //});
    }
}