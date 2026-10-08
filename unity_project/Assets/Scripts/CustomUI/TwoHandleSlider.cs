using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoHandleSlider : Slider
{
    private Slider m_MaxSlider;

    // Start is called before the first frame update
    void Awake()
    {
        m_MaxSlider = transform.SearchComponent<Slider>("MaxSlider");
        base.Awake();

    }

    protected override void Set(float input, bool sendCallback)
    {

        base.Set(input, sendCallback);
    }

    public void OnUpdate()
    {

    }

    public void SetMinMaxBounds(float min, float max)
    {
    }
}
