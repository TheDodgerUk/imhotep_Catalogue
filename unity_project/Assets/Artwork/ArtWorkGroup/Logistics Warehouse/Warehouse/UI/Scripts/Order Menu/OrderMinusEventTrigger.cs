using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class OrderMinusEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Button minusButton;
    Animator minusAnimation;
    CatalogueItemTotal total;
    
    // Start is called before the first frame update
    public void Init()
    {
        minusButton = gameObject.GetComponent<Button>();
        minusAnimation = gameObject.GetComponent<Animator>();
        total = transform.FindParent("Total").GetComponent<CatalogueItemTotal>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        minusAnimation.Play((true == minusButton.interactable) ? "On" : null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        minusAnimation.Play((true == minusButton.interactable) ? "Off" : null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        minusAnimation.Play((true == minusButton.interactable) ? "Off" : null);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        minusAnimation.Play((total.total > 2 && true == minusButton.interactable) ? "On" : null);
    }
}
