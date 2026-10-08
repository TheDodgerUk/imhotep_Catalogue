// Copyright (c) 2015, Felix Kate All rights reserved.
// Usage of this code is governed by a BSD-style license that can be found in the LICENSE file.

/*<Description>
For the new Unity GUI System won't work on older Unity Versions.
Short script for handling the controls of the color picker GUI element.
The user can drag the slider on the ring to change the hue and the slider in the box to set the blackness and saturation.
If used without prefab add this to an image canvas element which useses the ColorWheelMaterial.
Also needs 2 subobjects with images as slider graphics and an even trigger for clicking that references the OnClick() method of this script.
*/

using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using UnityEngine.Events;

public class ColorWheelControl : MonoBehaviour
{
    public class UnityEventColor : UnityEvent<Color> { }

    public UnityEventColor OnEventColor = new UnityEventColor();
    //Output Color
    public Color m_Selection;

    //Control values
    private float m_Outer;
    private Vector2 m_Inner;

    private bool m_DragOuter, m_DragInner;

    //The Components of the wheel
    private Material m_Material;
    private RectTransform m_RectTrans, m_SelectorOut, m_SelectorIn;

    private float m_HalfSize;

    private Color m_CurrentMaterailColor;


    //Set up the transforms
    void Start()
    {
        //var item = this.gameObject.ForceComponent<FakeButton>();
        //item.OnClick.AddListener(OnClick);
        //Get the rect transform and make x and y the same to avoid streching
        m_RectTrans = GetComponent<RectTransform>();
        m_RectTrans.sizeDelta = new Vector2(m_RectTrans.sizeDelta.x, m_RectTrans.sizeDelta.x);

        //Find and scale the children
        m_SelectorOut = transform.Find("Selector_Out").GetComponent<RectTransform>();
        m_SelectorIn = transform.Find("Selector_In").GetComponent<RectTransform>();

        m_SelectorOut.sizeDelta = m_RectTrans.sizeDelta / 20.0f * this.transform.lossyScale.x;
        m_SelectorIn.sizeDelta = m_RectTrans.sizeDelta / 20.0f * this.transform.lossyScale.x;

        //Calculate the half size
        m_HalfSize = m_RectTrans.sizeDelta.x / 2;

        //Set the material
        m_Material = GetComponent<Image>().material;

        //Set first selected value to red (0° rotation and upper right corner in the box)
        m_Selection = Color.red;
        m_CurrentMaterailColor = Color.red;
        //Update the material of the box to red
        UpdateMaterial();
    }

    public void SetAlpha(bool active)
    {
        m_CurrentMaterailColor.a = active ? 1f : 0.1f;
        UpdateMaterial();
        this.enabled = active;
    }



    //Update the selectors
    void Update()
    {
        //Drag selector of outer circle
        if (m_DragOuter)
        {
            //Get mouse direction
            Vector2 dir = m_RectTrans.position - Input.mousePosition;
            dir.Normalize();

            //Calculate the radians
            m_Outer = Mathf.Atan2(-dir.x, -dir.y);

            //And update
            UpdateMaterial();
            UpdateColor();

            //On mouse release also release the drag
            if (Input.GetMouseButtonUp(0))
                m_DragOuter = false;

            //Drag selector of inner box
        }
        else if (m_DragInner)
        {
            //Get position inside the box
            Vector2 dir = m_RectTrans.position - Input.mousePosition;
            dir.x = Mathf.Clamp(dir.x, -m_HalfSize / 2, m_HalfSize / 2) + m_HalfSize / 2;
            dir.y = Mathf.Clamp(dir.y, -m_HalfSize / 2, m_HalfSize / 2) + m_HalfSize / 2;

            //Scale the value to 0 - 1;
            m_Inner = dir / m_HalfSize;

            UpdateColor();

            //On mouse release also releaste the drag
            if (Input.GetMouseButtonUp(0))
                m_DragInner = false;
        }

        //Set the selectors positions
        m_SelectorOut.localPosition = new Vector3(Mathf.Sin(m_Outer) * m_HalfSize * 0.85f, Mathf.Cos(m_Outer) * m_HalfSize * 0.85f, 1);
        m_SelectorIn.localPosition = new Vector3(m_HalfSize * 0.5f - m_Inner.x * m_HalfSize, m_HalfSize * 0.5f - m_Inner.y * m_HalfSize, 1);
    }



    //Update the material of the inner box to match the hue color
    void UpdateMaterial()
    {
        if (m_Material != null)
        {
            //Calculation of rgb from degree with a modified 3 wave function
            //Check out http://en.wikipedia.org/wiki/File:HSV-RGB-comparison.svg to understand how it should look
            m_CurrentMaterailColor.r = Mathf.Clamp(2 / Mathf.PI * Mathf.Asin(Mathf.Cos(m_Outer)) * 1.5f + 0.5f, 0, 1);
            m_CurrentMaterailColor.g = Mathf.Clamp(2 / Mathf.PI * Mathf.Asin(Mathf.Cos(2 * Mathf.PI * (1.0f / 3.0f) - m_Outer)) * 1.5f + 0.5f, 0, 1);
            m_CurrentMaterailColor.b = Mathf.Clamp(2 / Mathf.PI * Mathf.Asin(Mathf.Cos(2 * Mathf.PI * (2.0f / 3.0f) - m_Outer)) * 1.5f + 0.5f, 0, 1);

            m_Material.SetColor("_Color", m_CurrentMaterailColor);
        }
    }



    //Gets called after changes
    void UpdateColor()
    {
        Color c = Color.white;

        //Calculation of color same as above
        c.r = Mathf.Clamp(2 / Mathf.PI * Mathf.Asin(Mathf.Cos(m_Outer)) * 1.5f + 0.5f, 0, 1);
        c.g = Mathf.Clamp(2 / Mathf.PI * Mathf.Asin(Mathf.Cos(2 * Mathf.PI * (1.0f / 3.0f) - m_Outer)) * 1.5f + 0.5f, 0, 1);
        c.b = Mathf.Clamp(2 / Mathf.PI * Mathf.Asin(Mathf.Cos(2 * Mathf.PI * (2.0f / 3.0f) - m_Outer)) * 1.5f + 0.5f, 0, 1);

        //Add the colors of the inner box
        c = Color.Lerp(c, Color.white, m_Inner.x);
        c = Color.Lerp(c, Color.black, m_Inner.y);

        m_Selection = c;
        OnEventColor?.Invoke(m_Selection);
    }



    //Method for setting the picker to a given color
    public void PickColor(Color c)
    {
        //Get hsb color from the rgb values
        float max = Mathf.Max(c.r, c.g, c.b);
        float min = Mathf.Min(c.r, c.g, c.b);

        float hue = 0;
        float sat = (1 - min);

        if (max == min)
        {
            sat = 0;
        }

        hue = Mathf.Atan2(Mathf.Sqrt(3) * (c.g - c.b), 2 * c.r - c.g - c.b);

        //Set the sliders
        m_Outer = hue;
        m_Inner.x = 1 - sat;
        m_Inner.y = 1 - max;

        //And update them once
        UpdateMaterial();
    }


    //Gets called by an event trigger at a click
    public void OnClick()
    {
        if (m_RectTrans == null)
        {
            Start();
        }
        //Check if click was in outer circle
        float innerDistance = Vector2.Distance(m_RectTrans.position, Input.mousePosition);
        float innerCirlce = m_HalfSize - (m_HalfSize / 4);
        float outerCirlce = m_HalfSize;

        innerCirlce *= this.transform.lossyScale.x;
        outerCirlce *= this.transform.lossyScale.x;
        if (innerDistance >= innerCirlce && innerDistance <= outerCirlce)
        {
            m_DragOuter = true;
            return;
            //Check if click was in inner box
        }
        else
        {
            float x = Mathf.Abs(m_RectTrans.position.x - Input.mousePosition.x);
            float y = Mathf.Abs(m_RectTrans.position.y - Input.mousePosition.y);
            if (x <= (m_HalfSize / 2) && y <= (m_HalfSize / 2))
            {
                m_DragInner = true;
                return;
            }
        }
    }

}
