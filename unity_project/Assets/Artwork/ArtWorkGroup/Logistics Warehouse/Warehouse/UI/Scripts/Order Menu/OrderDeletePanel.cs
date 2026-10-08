using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrderDeletePanel : MonoBehaviour
{
    private Delete m_DeleteBtn;
    private Animator m_Animator;
    private OrderItemTotal m_total;
    private CatalogueItemData m_CatalogueItemData;

    public void Init()
    {
        var deleteGo = transform.Search("Delete");
        m_DeleteBtn = deleteGo.gameObject.ForceComponent<Delete>();
        m_DeleteBtn.Init();

        m_Animator = gameObject.GetComponent<Animator>();
        var totalGo = transform.Search("Total");
        m_total = totalGo.gameObject.ForceComponent<OrderItemTotal>();
    }

    // Start is called before the first frame update
    public void Init(Action<GameObject> OnRemove, CatalogueItemData catalogueItem)
    {       
        m_DeleteBtn.SetCallback((e) => OnRemove?.Invoke(gameObject));
        m_Animator.Play("CreatePanel");
        m_CatalogueItemData = catalogueItem;
        m_total.Init(m_CatalogueItemData);
    }

    public float GetTotal()
    {
        return m_total.total;
    }

    public void Confirm()
    {
        StartCoroutine(PlayConfirmAnimation());
    }

    private IEnumerator PlayConfirmAnimation()
    {
        m_Animator.Play("ConfirmOrder");
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }
}
