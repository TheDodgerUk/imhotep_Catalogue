using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class OrderList : MonoBehaviour
{
    public Action<List<CatalogueItemData>> OnProcessPurchase;
    [SerializeField] private GameObject buttonTemplate;
    [SerializeField] public float startHeight = -150;
    [SerializeField] public float spacing = -120;

    private List<OrderedItem> m_OrderItemCache = new List<OrderedItem>();

    public GameObject m_HomeBubble;
    private TextMeshProUGUI homeBubbleText;
    private Animator homeBubbleAnim;

    public GameObject m_OrderBubble;
    private TextMeshProUGUI orderBubbleText;
    private Animator orderBubbleAnim;

    private ScrollHeight m_ScrollHeight;
    private int totalCount;

    public GameObject m_Empty;
    private Animator m_EmptyAnimator;
    public GameObject m_Progress;
    private Animator m_ProgressAnimator;
    private GameObject m_ProgressEndFill;
    private Slider m_ProgressSlider;
    public GameObject m_progComp;
    private Animator m_progCompAnimator;

    public GameObject m_CompleteOrdersGO;
    private Image completeColour;
    private Button completeButton;
    private Animator completeAnimator;
    private GameObject m_MenuCentre;

    [SerializeField] private float maxHeight = 450;
    [SerializeField] private float minHeight = -450;

    public Button addBtn;
    public Transform addRoot;

    private Transform cacheRoot;

    private bool processingOrder;
    [SerializeField] public List<GameObject> m_OrderedItemsList = new List<GameObject>();
    private Dictionary<GameObject, OrderDeletePanel> m_DictOfButtons = new Dictionary<GameObject, OrderDeletePanel>();

    private Dictionary<CatalogueItemData, GameObject> uniqueOrderedEntries = new Dictionary<CatalogueItemData, GameObject>();

    public void Init(GameObject orderedItem, float fStartHeight, float fSpacing, Transform menuRoot,
        Transform empty, Transform progress, Transform progComp, GameObject homeBubble, GameObject orderBubble, GameObject completeOrders, GameObject menuCentre)
    {
        buttonTemplate = orderedItem;
        startHeight = fStartHeight;
        spacing = fSpacing;
        m_MenuCentre = menuCentre;

        cacheRoot = new GameObject("orderCache").transform;
        cacheRoot.SetParent(menuRoot);

        m_HomeBubble = homeBubble;
        homeBubbleText = homeBubble.GetComponentInChildren<TextMeshProUGUI>();
        homeBubbleAnim = homeBubble.GetComponent<Animator>();

        m_OrderBubble = orderBubble;
        orderBubbleText = orderBubble.GetComponentInChildren<TextMeshProUGUI>();
        orderBubbleAnim = orderBubble.GetComponent<Animator>();

        m_CompleteOrdersGO = completeOrders;
        var completeEventTrigger = m_CompleteOrdersGO.ForceComponent<CompleteEventTrigger>();
        completeEventTrigger.Init();
        completeColour = m_CompleteOrdersGO.GetComponent<Image>();
        completeButton = m_CompleteOrdersGO.GetComponent<Button>();
        completeAnimator = m_CompleteOrdersGO.GetComponent<Animator>();

        Action completeCallback = ConfirmAll;
        completeButton.onClick.AddListener(() => ConfirmAll());

        m_ScrollHeight = gameObject.ForceComponent<ScrollHeight>();
        var gradientMask = transform.Search("ContentBackground");
        m_ScrollHeight.Init(gradientMask.gameObject, spacing);

        //Temp button
        addRoot = m_CompleteOrdersGO.transform.parent.Find("Add");
        addBtn = addRoot.gameObject.GetComponent<Button>();
        addBtn.onClick.RemoveAllListeners();
        addBtn.onClick.AddListener(() => CreateNewPanel(new CatalogueItemData()));

        m_Empty = empty.gameObject;
        m_EmptyAnimator = m_Empty.GetComponent<Animator>();

        m_Progress = progress.gameObject;
        m_ProgressAnimator = m_Progress.GetComponent<Animator>();
        m_ProgressEndFill = m_Progress.transform.Find("ProgressFill/EndFill").gameObject;
        m_ProgressSlider = m_Progress.transform.Find("ProgressFill/Slider").GetComponent<Slider>();

        m_progComp = progComp.gameObject;
        m_progCompAnimator = m_progComp.GetComponent<Animator>();

        CacheOrderItems();
    }

    private void CacheOrderItems(int iCount = 25)
    {
        for (int i = 0; i < iCount; i++)
        {
            m_OrderItemCache.Add(CreateOrderItem());
        }
    }

    private OrderedItem GetNextItem()
    {
        if (m_OrderItemCache.Count > 0)
        {
            var item = m_OrderItemCache[0];
            m_OrderItemCache.Remove(item);
            return item;
        }
        else
        {
            var item = CreateOrderItem();
            return item;
        }
    }

    private void ReturnOrderedItem(OrderedItem item)
    {
        m_OrderItemCache.Add(item);
        item.gameObject.SetActive(false);
        item.transform.SetParent(cacheRoot);
    }

    private OrderedItem CreateOrderItem()
    {
        GameObject orderedItem = Utils.Unity.Clone(buttonTemplate) as GameObject;
        orderedItem.transform.SetParent(cacheRoot);
        var orderScript = orderedItem.ForceComponent<OrderedItem>();
        orderScript.Init();
        return orderScript;
    }

    public void CreateNewPanel(CatalogueItemData catalogueItem)
    {
        if (uniqueOrderedEntries.ContainsKey(catalogueItem) == true)
        {
            return;
        }

        totalCount = m_OrderedItemsList.Count;

        var orderdItem = GetNextItem();

        orderdItem.transform.SetParent(transform, false);
        orderdItem.transform.localPosition = new Vector3(-500, startHeight + (spacing * totalCount), 0);
        orderdItem.transform.localRotation = Quaternion.identity;
        orderdItem.transform.localScale = Vector3.one;

        orderdItem.gameObject.SetActive(true);

        orderdItem.Init((e) =>
        {
            catalogueItem.orderedCount = 0;
        }, catalogueItem);

        orderdItem.SetName(catalogueItem.sName, catalogueItem.Image);

        m_OrderedItemsList.Add(orderdItem.gameObject);
        m_ScrollHeight.UpdateScroll(m_OrderedItemsList.Count);

        catalogueItem.OnOrderCountUpdated += UpdateTotal;

        uniqueOrderedEntries.Add(catalogueItem, orderdItem.gameObject);
        m_DictOfButtons.Add(orderdItem.gameObject, orderdItem.GetDeleteButton);
        BasketCheck(m_OrderedItemsList.Count);

        UpdateTotal();
    }

    public void OnRemoveOrder(CatalogueItemData data)
    {
        if (false == processingOrder && true == uniqueOrderedEntries.ContainsKey(data))
        {
            OnRemoveOrder(uniqueOrderedEntries[data]);
            data.OnOrderCountUpdated -= UpdateTotal;
            uniqueOrderedEntries.Remove(data);
        }
    }

    private void OnRemoveOrder(GameObject obj)
    {
        m_OrderedItemsList.Remove(obj);
        m_DictOfButtons.Remove(obj);
        ReturnOrderedItem(obj.GetComponent<OrderedItem>());
        m_ScrollHeight.UpdateScroll(m_OrderedItemsList.Count);
        StopAllCoroutines();
        StartCoroutine(SlideUp());
        BasketCheck(m_OrderedItemsList.Count);
        UpdateTotal();
    }

    [InspectorButton]
    public void ConfirmAll()
    {
        StartCoroutine(RunConfirm());
    }

    private IEnumerator RunConfirm()
    {
        processingOrder = true;
        yield return null;

        foreach (GameObject inRange in m_OrderedItemsList)
        {
            OrderDeletePanel btn = m_DictOfButtons[inRange];
            float inRangeY = m_MenuCentre.transform.InverseTransformPoint(inRange.transform.position).y;
            if (inRangeY > minHeight && inRangeY < maxHeight)
            {
                btn.Confirm();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                btn.Delete();
            }
        }

        m_OrderedItemsList.Clear();

        completeColour.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        completeButton.interactable = false;

        StartCoroutine(CompleteProgress());

        m_ScrollHeight.UpdateScroll(m_OrderedItemsList.Count);

        OnProcessPurchase?.Invoke(new List<CatalogueItemData>(uniqueOrderedEntries.Keys));

        m_DictOfButtons.Clear();
        uniqueOrderedEntries.Clear();
        UpdateTotal();
        processingOrder = false;
    }

    private IEnumerator CompleteProgress()
    {
        m_ProgressEndFill.active = false;
        m_ProgressAnimator.Play("ProgressActive");
        yield return new WaitForSeconds(m_ProgressAnimator.GetCurrentAnimatorStateInfo(0).length);

        float duration = 0.5f;
        float timer = 0;
        while (duration > timer)
        {
            timer += Time.deltaTime;
            float normalisedTime = Mathf.InverseLerp(0, duration, timer);
            m_ProgressSlider.value = normalisedTime;
            yield return new WaitForEndOfFrame();
        }

        m_ProgressEndFill.active = true;

        yield return new WaitForSeconds(1);
        m_ProgressAnimator.Play("ProgressInactive");
        m_progCompAnimator.Play("CompleteActive");
        yield return new WaitForSeconds(m_progCompAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(1);
        m_progCompAnimator.Play("CompleteInactive");
        m_ProgressSlider.value = 0;
        m_ProgressEndFill.active = false;

        totalCount = totalCount - totalCount - 1;
        BasketCheck(totalCount);
    }

    private void Move(float t)
    {
        for (int i = 0; i < m_OrderedItemsList.Count; i++)
        {
            Vector3 currentPosition = m_OrderedItemsList[i].transform.localPosition;
            float newYPosition = startHeight + (spacing * i);
            currentPosition.y = Mathf.Lerp(currentPosition.y, newYPosition, t);
            m_OrderedItemsList[i].transform.localPosition = currentPosition;
        }
    }

    private IEnumerator SlideUp()
    {
        float duration = 0.5f;
        float timer = 0;

        while (duration > timer)
        {
            timer += Time.deltaTime;
            float normalisedTime = Mathf.InverseLerp(0, duration, timer);
            Move(normalisedTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void BasketCheck(int totalCount)
    {
        if (totalCount > 0)
        {
            m_EmptyAnimator.Play("EmptyInactive");
            completeColour.color = new Color(0.18f, 0.9f, 0.32f, 1f);
            completeButton.interactable = true;
        }
        else
        {
            m_EmptyAnimator.Play("EmptyActive");
            completeColour.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            completeButton.interactable = false;
        }
    }

    public void UpdateTotal()
    {
        float total = 0;
        foreach (KeyValuePair<GameObject, OrderDeletePanel> kvp in m_DictOfButtons)
        {
            total += kvp.Value.GetTotal();
            homeBubbleAnim.Play("Add");
            orderBubbleAnim.Play("Add");
        }
        if (total <= 0)
        {
            homeBubbleAnim.Play("Close");
            orderBubbleAnim.Play("Close");
        }
        if (total > 0 && homeBubbleAnim.GetCurrentAnimatorStateInfo(0).IsName("Closed"))
        {
            homeBubbleAnim.Play("Open");
            orderBubbleAnim.Play("Open");
        }
        //print(total);
        homeBubbleText.text = total.ToString();
        orderBubbleText.text = total.ToString();
    }

    [InspectorButton]
    public void AddOrder()
    {
        CreateNewPanel(new CatalogueItemData());
    }
}