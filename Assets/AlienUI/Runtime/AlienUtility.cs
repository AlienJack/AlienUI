using AlienUI.Models;
using UnityEngine;

namespace AlienUI.UIElements.ToolsScript
{
    internal static class AlienUtility
    {
        internal static void PerformRectTransform(this UIElement uiElement)
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
            uiElement.Rect.anchoredPosition = uiElement.Offset;
        }
        internal static void SetPivotStayPosition(this RectTransform rectTransform, Vector2 pivot)
        {
            Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
            deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
            deltaPosition.Scale(rectTransform.localScale);          // apply scaling
            deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

            rectTransform.pivot = pivot;                            // change the pivot
            rectTransform.localPosition -= deltaPosition;           // reverse the position change
        }
        internal static void SetStretchModeOffsets(this RectTransform rectTransform, float top, float bottom, float left, float right)
        {
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(-right, -top);
        }
        internal static void SetStretchModeOffsets(this RectTransform rectTransform, BorderData borderInfo)
        {
            rectTransform.SetStretchModeOffsets(borderInfo.top, borderInfo.bottom, borderInfo.left, borderInfo.right);
        }
        internal static TextAnchor ConvertToTextAnchor(TextAlignHorizontal hori, TextAlignVertical verti)
        {
            if (verti == TextAlignVertical.Top)
            {
                if (hori == TextAlignHorizontal.Left)
                {
                    return TextAnchor.UpperLeft;
                }
                else if (hori == TextAlignHorizontal.Middle)
                {
                    return TextAnchor.UpperCenter;
                }
                else if (hori == TextAlignHorizontal.Right)
                {
                    return TextAnchor.LowerRight;
                }
            }
            else if (verti == TextAlignVertical.Middle)
            {
                if (hori == TextAlignHorizontal.Left)
                {
                    return TextAnchor.MiddleLeft;
                }
                else if (hori == TextAlignHorizontal.Middle)
                {
                    return TextAnchor.MiddleCenter;
                }
                else if (hori == TextAlignHorizontal.Right)
                {
                    return TextAnchor.MiddleRight;
                }
            }
            else if (verti == TextAlignVertical.Bottom)
            {
                if (hori == TextAlignHorizontal.Left)
                {
                    return TextAnchor.LowerLeft;
                }
                else if (hori == TextAlignHorizontal.Middle)
                {
                    return TextAnchor.LowerCenter;
                }
                else if (hori == TextAlignHorizontal.Right)
                {
                    return TextAnchor.LowerRight;
                }
            }

            return default;
        }
        internal static GameObject CreateEmptyUIGameObject(string name)
        {
            var go = new GameObject(name);
            var rect = go.AddComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            return go;
        }
    }
}
