using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AddEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{

    Button addButton;
    Animator addAnimation;

    // Start is called before the first frame update
    void Start()
    {
        addButton = gameObject.GetComponent<Button>();
        addAnimation = gameObject.GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
            addAnimation.Play("On");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        addAnimation.Play("Off");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
            addAnimation.Play("Off");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
            addAnimation.Play("On");
    }
}
