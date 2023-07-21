using UnityEngine;
using UnityEngine.UI;

namespace UI.Utilities
{
    public class ContentResizer : MonoBehaviour
    {
        private float _contentWidth;
        private GridLayoutGroup _gridLayoutGroup;
        private int _lastChildCount;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _gridLayoutGroup = GetComponent<GridLayoutGroup>();
            _contentWidth = _rectTransform.rect.width;

            Resize();
        }

        private void LateUpdate()
        {
            if (_lastChildCount != GetActiveChildCount())
                Resize();
        }

        private void Resize()
        {
            var spacing = _gridLayoutGroup.spacing;
            var cellSize = _gridLayoutGroup.cellSize;
            var numberOfActiveChildren = GetActiveChildCount();
            _lastChildCount = numberOfActiveChildren;
            var newHeight = (cellSize.y + spacing.y) * numberOfActiveChildren / 2f;
            _rectTransform.sizeDelta = new Vector2(0, newHeight);
        }

        private int GetActiveChildCount()
        {
            var activeCount = 0;

            // Loop through each child of the parent Transform
            for (var i = 0; i < transform.childCount; i++)
                // Check if the child is active
                if (transform.GetChild(i).gameObject.activeInHierarchy)
                    activeCount++;

            return activeCount;
        }
    }
}