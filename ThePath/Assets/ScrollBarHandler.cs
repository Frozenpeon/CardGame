using UnityEngine.UI;
using UnityEngine;

public class ScrollbarHandleSizer : MonoBehaviour
{
    [SerializeField] private ScrollRect _ScrollRect;

    void Start()
    {
        UpdateHandleSize();
    }

    void Update()
    {
        if (_ScrollRect.horizontalScrollbar != null)
        {
            UpdateHandleSize();
        }
    }

    private void UpdateHandleSize()
    {
        // Assuming horizontal scrolling
        float contentWidth = _ScrollRect.content.rect.width;
        float viewportWidth = _ScrollRect.viewport.rect.width;

        if (contentWidth > viewportWidth)
        {
            float visibleRatio = viewportWidth / contentWidth;
            _ScrollRect.horizontalScrollbar.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, visibleRatio * _ScrollRect.viewport.rect.width);
        }
        else
        {
            // If all content fits within the viewport, the handle size matches the viewport.
            _ScrollRect.horizontalScrollbar.handleRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _ScrollRect.viewport.rect.width);
        }
    }
}