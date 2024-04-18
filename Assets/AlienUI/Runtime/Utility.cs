using AlienUI.Models;
using AlienUI.UIElements;
using UnityEngine;

namespace AlienUI
{
    public static class Utility
    {
        public static void PerformRectTransform(this UIElement uiElement)
        {
            var anchorMin = new Vector2();
            var anchorMax = new Vector2();
            var pivot = new Vector2();
            switch (uiElement.Horizontal)
            {
                case eHorizontalAlign.Left:
                    anchorMin.x = 0; anchorMax.x = 0; pivot.x = 0; break;
                case eHorizontalAlign.Middle:
                    anchorMin.x = 0.5f; anchorMax.x = 0.5f; pivot.x = 0.5f; break;
                case eHorizontalAlign.Right:
                    anchorMin.x = 1; anchorMax.x = 1; pivot.x = 1f; break;
                case eHorizontalAlign.Stretch:
                    anchorMin.x = 0; anchorMax.x = 1; pivot.x = 0.5f; break;
            }
            switch (uiElement.Vertical)
            {
                case eVerticalAlign.Bottom:
                    anchorMin.y = 0; anchorMax.y = 0; pivot.y = 0f; break;
                case eVerticalAlign.Middle:
                    anchorMin.y = 0.5f; anchorMax.y = 0.5f; pivot.y = 0.5f; break;
                case eVerticalAlign.Top:
                    anchorMin.y = 1; anchorMax.y = 1; pivot.y = 1f; break;
                case eVerticalAlign.Stretch:
                    anchorMin.y = 0; anchorMax.y = 1; pivot.y = 0.5f; break;
            }

            uiElement.Rect.anchorMax = anchorMax;
            uiElement.Rect.anchorMin = anchorMin;
            uiElement.Rect.pivot = pivot;
        }
        public static void SetPivotStayPosition(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
            deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
            deltaPosition.Scale(rectTransform.localScale);          // apply scaling
            deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

            rectTransform.pivot = pivot;                            // change the pivot
            rectTransform.localPosition -= deltaPosition;           // reverse the position change
        }

        public static void SetStretchModeOffsets(this RectTransform rectTransform, float top, float bottom, float left, float right)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(-right, -top);
        }

        public static void SetStretchModeOffsets(this RectTransform rectTransform, Border borderInfo)
        {
            rectTransform.SetStretchModeOffsets(borderInfo.top, borderInfo.bottom, borderInfo.left, borderInfo.right);
        }

    }
}
