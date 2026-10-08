using UnityEngine;
using UnityEngine.UI;

namespace CustomUiElements
{
    public class MaximumSlider : Slider
    {
        private float offset = 0.1f;
        public MinimumSlider MinSlider;
        public Text Indicator;
        public string NumberFormat;

        public float RealValue;
        private bool assignedRealValue = false;

        protected override void Awake()
        {
            base.Awake();
            targetGraphic = transform.SearchComponent<Graphic>("Handle");
        }

        protected override void Start()
        {
            RealValue = maxValue;
            base.Start();
        }

        protected override void Set(float input, bool sendCallback)
        {
            if (MinSlider == null)
            {
                MinSlider = transform.parent.GetComponentInChildren<MinimumSlider>();
            }
            if (!assignedRealValue)
            {
                RealValue = maxValue;
                assignedRealValue = true;
            }
            else
            {
                RealValue = maxValue - input + minValue;
            }

            if (wholeNumbers)
            {
                RealValue = Mathf.Round(RealValue);
            }
            if (input - offset <= MinSlider.value)
            {
                // invalid value
                input = MinSlider.value + offset;
            }
            if (Indicator != null)
            {
                Indicator.text = RealValue.ToString(NumberFormat);
            }
            base.Set(input, sendCallback);
        }

        public void Refresh(float input)
        {
            Set(input, false);
        }
    }
}