using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Delete : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Action<GameObject> OnFinishedFade;
    private CanvasGroup panel;
    private RectTransform backgroundSize;
    private float backgroundWidth;

    private Animator buttonAni;
    private Animator panelBubbleAni;
    private float clipLength;
    public Action onUpdateCount;

    private Button m_DeleteButton;
    private GameObject content;

    float clickTime;
    bool clicked;
    bool canBeClicked;
    bool m_bDestroying = false;

    public void Init()
    {
        panel = transform.parent.parent.GetComponent<CanvasGroup>();
        backgroundSize = transform.parent.GetComponent<RectTransform>();
        backgroundWidth = backgroundSize.rect.width;
        buttonAni = gameObject.GetComponent<Animator>();
        panelBubbleAni = transform.parent.Find("Content/Total/ItemBubble").GetComponent<Animator>();

        content = transform.parent.Find("Content").gameObject;
        clipLength = GetAnimationClipLength(buttonAni, "Button");

        m_DeleteButton = GetComponent<Button>();
    }

    void Update()
    {
        if (clicked == true && false == m_bDestroying)
        {
            float v = Mathf.InverseLerp(0, 1, Time.time - clickTime);
            buttonAni?.Play("Delete", 0, v);
            panelBubbleAni?.Play("PanelDelete", 0, v);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clickTime = Time.time;
        clicked = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
        if (Time.time - clickTime > 1 && true == canBeClicked)
        {
            m_bDestroying = true;
            m_DeleteButton.interactable = false;
            StartCoroutine(FadePanel());
        }
        else
        {
            float v = Mathf.InverseLerp(1, 0, Time.time - clickTime);
            buttonAni.Play("Reverse", 0, v);
            panelBubbleAni.Play("Reverse", 0, v);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonAni.Play("On");
        canBeClicked = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonAni.Play("Off");
        clicked = false;
        canBeClicked = false;
    }


    public void SetCallback(Action<GameObject> onFinished)
    {
        OnFinishedFade = onFinished;
    }

    private IEnumerator FadePanel()
    {
        float speed = 2;
        float alpha = 1;
        Destroy(content);

        while (true)
        {
            alpha = alpha - Time.deltaTime * speed;
            panel.alpha = alpha;
            if (alpha <= 0)
            {
                OnFinishedFade?.Invoke(transform.parent.parent.gameObject);
                break;
            }
            yield return null;
        }
    }

    private float GetAnimationClipLength(Animator animator, string clipName)
    {
        var clips = animator.runtimeAnimatorController;
        for (int i = 0; i < clips.animationClips.Length; i++)
        {
            if (clips.animationClips[i].name == clipName)
            {
                return clips.animationClips[i].length;
            }
        }

        return 0f;
    }

    //[InspectorButton]
    //public void Clicked()
    //{
    //    Bin();
    //}
}