using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollHeight : MonoBehaviour
{
    private float height;
    private float width;
    [SerializeField] private RectTransform rectTrans;
    [SerializeField] private float backgroundOffset;
    private float listOffset;
    [SerializeField] public GameObject backgroundMask;
    [SerializeField] private RectTransform maskScale;
    [SerializeField] float minimumBackgroundSize;
    float minimumListSize;
    float minScale;
    float padding;

    public void Init(GameObject background, float padding)
    {
        this.padding = Mathf.Abs(padding);
        backgroundMask = background;
        height = gameObject.GetComponent<RectTransform>().rect.height;
        width = gameObject.GetComponent<RectTransform>().rect.width;
        rectTrans = gameObject.GetComponent(typeof(RectTransform)) as RectTransform;
        maskScale = backgroundMask.GetComponent(typeof(RectTransform)) as RectTransform;
        minimumListSize = rectTrans.rect.height;
        listOffset = Mathf.Abs(rectTrans.offsetMax.y);

        minimumBackgroundSize = maskScale.rect.height;
        backgroundOffset = Mathf.Abs(maskScale.offsetMax.y);
        minScale = maskScale.localScale.y;
    }

    public void UpdateScroll(int total)
    {
        
        float rectHeight = Mathf.Max(minimumListSize, listOffset + (Mathf.Abs(padding) * total) + (backgroundOffset * 2.5f));
        //rectTrans.sizeDelta = new Vector2(width, rectHeight);]

        rectTrans.SetHeight(rectHeight);

        StartCoroutine(ResizeMask(maskScale.localScale, total));
        StopCoroutine(ResizeMask(maskScale.localScale, total));
    }

    private bool Expand(int count, float padding, float backgroundHeight)
    {
        if (Mathf.Abs(count * padding) > backgroundHeight)
        {
            return true;
        }

        return false;
    }

    private IEnumerator ResizeMask(Vector3 endScale, int total)
    {
        yield return null;

        float duration = .3f;
        float timer = 0;

        while (duration > timer)
        {
            timer += Time.deltaTime;
            float normalisedTime = Mathf.InverseLerp(0, duration, timer);

            Vector3 startScale = maskScale.localScale;

             float newYScale;

            float scaledOffset = (backgroundOffset / minimumBackgroundSize);
            float b = Mathf.Abs(padding / (minimumBackgroundSize));
            float a = (b * total) + scaledOffset;
            newYScale = (Mathf.Max(a, minScale));

            //if (total >= 5)
            //{
            //    //magic numbers to increse the background height enough to
            //    //encase all the orders, tested up to 100 orders, still looks good
            //    //newYScale = (0.1f + (total * 0.22855f));
            //    float scaledOffset = (offset * 0.5f) / minimumBackgroundSize;
            //    newYScale = (Mathf.Abs(padding / minimumBackgroundSize) * total) + scaledOffset;
            //    yield return newYScale;
            //}
            //else
            //{
            //    newYScale = 1f;
            //    yield return newYScale;
            //}

            endScale.y = Mathf.Lerp(startScale.y, newYScale, normalisedTime);
            maskScale.localScale = endScale;

            yield return new WaitForEndOfFrame();
        }
    }

}
