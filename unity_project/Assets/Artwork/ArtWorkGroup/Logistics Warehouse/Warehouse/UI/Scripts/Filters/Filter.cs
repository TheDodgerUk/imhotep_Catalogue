using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Filter : MonoBehaviour
{
    public Action OnFilterStateChange;

    [SerializeField]  private bool bToggle = false;
    private string m_sFilterName;
    private TextMeshProUGUI title;
    private Image filterBackground;
    private Button filterButton;
    private Color selectedColour = new Color (0.635f, 0.392f, 0.929f, 1);
    private Color deselectedColour = new Color(1, 1, 1, 1);
    public bool Selected => bToggle;

    public string FilterName => m_sFilterName;

    public void Init(string filterName)
    {
        bToggle = false;
        m_sFilterName = filterName;

        title = GetComponentInChildren<TextMeshProUGUI>();
        title.text = m_sFilterName;
        filterBackground = GetComponentInChildren<Image>();
        filterButton = GetComponent<Button>();

        filterButton.onClick.AddListener(() => OnClick());
        ChangeColourState();
    }

    private void OnClick()
    {
        bToggle = !bToggle;
        ChangeColourState();

        OnFilterStateChange?.Invoke();
    }

    private void ChangeColourState()
    {
        if (true == bToggle)
        {
            filterBackground.color = selectedColour;

            ColorBlock colorBlock = filterButton.colors;
            colorBlock.normalColor = new Color(1, 1, 1, 1);
            colorBlock.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1);
            colorBlock.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1);
            filterButton.colors = colorBlock;
        }
        else
        {
            filterBackground.color = deselectedColour;

            ColorBlock colorBlock = filterButton.colors;
            colorBlock.normalColor = new Color(1, 1, 1, 0);
            colorBlock.highlightedColor = new Color(1, 1, 1, 0.2f);
            colorBlock.pressedColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
            filterButton.colors = colorBlock;
        }
    }

    public void SetPosition(ref int index, Transform filterList)
    {
        transform.position = filterList.TransformPoint(new Vector3(-100, (index * FilterList.filterPadding) + FilterList.offset, 0));
        index++;
    }

    public void IsActive(bool active)
    {
        gameObject.SetActive(active);
    }
}