using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CompleteEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    private Button completeButton;
    private Animator completeAnimation;

    public bool Hovering = false;

    // Start is called before the first frame update
    public void Init()
    {
        completeButton = gameObject.GetComponent<Button>();
        completeAnimation = gameObject.GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (completeButton.interactable == true)
        {
            completeAnimation.Play("On");
        }
        else
        {
            completeAnimation.Play(null);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (completeButton.interactable == true)
        {
            completeAnimation.Play("Off");
        }
        else
        {
            completeAnimation.Play(null);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (completeButton.interactable == true)
        {
            completeAnimation.Play("Off");
        }
        else
        {
            completeAnimation.Play(null);
        }
    }
}
