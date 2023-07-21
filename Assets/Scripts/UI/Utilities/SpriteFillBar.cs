using UnityEngine;

namespace UI.Utilities
{
    public class SpriteFillBar : MonoBehaviour
    {
        [SerializeField] private float start;
        [SerializeField] private float min;
        [SerializeField] private float max;

        public Transform fillImage;

        private void Start()
        {
            var newScale = fillImage.localScale;
            newScale.x = CalculateFillAmount(start);
            fillImage.localScale = newScale;
        }

        private float CalculateFillAmount(float percentage)
        {
            return Mathf.Lerp(min, max, percentage);
        }

        public void SetFillBar(float percentage)
        {
            var newScale = fillImage.localScale;
            newScale.x = CalculateFillAmount(percentage);
            fillImage.localScale = newScale;
        }
    }
}