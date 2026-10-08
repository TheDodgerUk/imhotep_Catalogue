using UnityEngine;
using UnityEngine.UI;

namespace CustomUiElements
{
    public class MinimumSlider : Slider
    {
        private float offset = 0.1f;
        public MaximumSlider MaxSlider;
        public Text Indicator;
        public string NumberFormat;

        protected override void Awake()
        {
            base.Awake();
            targetGraphic = transform.SearchComponent<Graphic>("Handle");
        }

        protected override void Set(float input, bool sendCallback)
        {
            if (MaxSlider == null)
            {
                MaxSlider = transform.parent.GetComponentInChildren<MaximumSlider>();
            }

            float newValue = input;
            if (wholeNumbers)
            {
                newValue = Mathf.Round(newValue);
            }
            if (newValue + offset >= MaxSlider.value)
            {
                // invalid
                newValue = MaxSlider.value - offset;
            }
            if (Indicator != null)
            {
                Indicator.text = newValue.ToString(NumberFormat);
            }
            base.Set(newValue, sendCallback);
        }

        public void Refresh(float input)
        {
            Set(input, false);
        }
    }
}