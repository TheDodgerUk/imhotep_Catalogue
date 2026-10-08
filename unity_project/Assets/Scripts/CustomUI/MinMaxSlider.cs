using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace CustomUiElements
{
    public class MinMaxSlider : MonoBehaviour
    {
        public Action<float, float> OnMinMaxValueChanged;
        private const float offset = 0.1f;

        [Header("Common settings")]
        public int DecimalPlaces = 2;
        public float MinimumValue = 0;
        public float MaximumValue = 1;
        public bool UseWholeNumbers = false;

        [Header("Configuration for MinSlider")]
        private Slider m_MinSlider;

        [Header("Configuration for MaxSlider")]
        private Slider m_MaxSlider;

        private RectTransform m_Fill;
        [SerializeField]
        private float m_MinFillValue;
        [SerializeField]
        private float m_MaxFillValue;

        // Properties
        public float CurrentLowerValue
        {
            get { return m_MinSlider.value; }
        }
        public float CurrentUpperValue
        {
            get { return m_MaxSlider.value; }
        }

        void Awake()
        {
            m_Fill = transform.SearchComponent<RectTransform>("Fill");
            m_MinFillValue = m_Fill.offsetMin.x;
            m_MaxFillValue = m_Fill.offsetMax.x;
            Transform minSlider = transform.Search("MinSlider");
            m_MinSlider = minSlider.gameObject.ForceComponent<Slider>();
            Transform maxSlider = transform.Search("MaxSlider");
            m_MaxSlider = maxSlider.gameObject.ForceComponent<Slider>();

            // Adjust Max/Min values for both sliders
            m_MinSlider.minValue = MinimumValue;
            m_MinSlider.maxValue = MaximumValue;
            m_MinSlider.wholeNumbers = UseWholeNumbers;

            m_MaxSlider.minValue = MinimumValue;
            m_MaxSlider.maxValue = MaximumValue;
            m_MaxSlider.wholeNumbers = UseWholeNumbers;

            m_MinSlider.onValueChanged.AddListener(UpdateMin);
            m_MaxSlider.onValueChanged.AddListener(UpdateMax);
        }

        private void UpdateMin(float min)
        {
            if (min + offset >= m_MaxSlider.value)
            {
                // invalid
                min = m_MaxSlider.value - offset;
            }

            m_MinSlider.value = min;
            OnUpdate();
        }

        private void UpdateMax(float max)
        {
            if (max - offset <= m_MinSlider.value)
            {
                // invalid value
                max = m_MinSlider.value + offset;
            }

            m_MaxSlider.value = max;
            OnUpdate();
        }

        private void OnUpdate()
        {
            OnMinMaxValueChanged?.Invoke(m_MinSlider.value, m_MaxSlider.value);
            Debug.Log($"Values: {m_MinSlider.value} : {m_MaxSlider.value}");

            Vector2 offsetMin = m_Fill.offsetMin;
            offsetMin.x = Mathf.Lerp(m_MinFillValue, m_MaxFillValue, m_MinSlider.value);
            m_Fill.offsetMin = offsetMin;

            Vector2 offsetMax = m_Fill.offsetMax;
            offsetMax.x = Mathf.Lerp(m_MinFillValue, m_MaxFillValue, m_MaxSlider.value);
            m_Fill.offsetMax = offsetMax;
        }
        

    }
}