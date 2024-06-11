using AlienUI.Models;
using AlienUI.UIElements.Containers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class WeightDock : Container
    {
        public EnumDirection LayoutDirection
        {
            get { return (EnumDirection)GetValue(LayoutDirectionProperty); }
            set { SetValue(LayoutDirectionProperty, value); }
        }

        public static readonly DependencyProperty LayoutDirectionProperty =
            DependencyProperty.Register("LayoutDirection", typeof(EnumDirection), typeof(WeightDock), new PropertyMetadata(EnumDirection.Horizontal), OnLayoutParamDirty);



        public static float GetWeight(DependencyObject obj)
        {
            return (float)obj.GetValue(WeightProperty);
        }

        public static void SetWeight(DependencyObject obj, float value)
        {
            obj.SetValue(WeightProperty, value);
        }

        public static readonly DependencyProperty WeightProperty =
            DependencyProperty.RegisterAttached("Weight", typeof(float), typeof(WeightDock), new PropertyMetadata(1f), OnLayoutParamDirty);

        protected override Vector2 OnCalcDesireSize(IReadOnlyList<UIElement> children)
        {
            Vector2 rect = new Vector2();
            rect.x += Padding.left + Padding.right;
            rect.y += Padding.top + Padding.bottom;

            foreach (var child in children)
            {
                var childDesireSize = child.GetDesireSize();
                if (LayoutDirection == EnumDirection.Horizontal)
                {
                    rect.x += childDesireSize.x;
                    rect.y = Mathf.Max(rect.y, childDesireSize.y);
                }
                else if (LayoutDirection == EnumDirection.Vertical)
                {
                    rect.y += child.GetDesireSize().x;
                    rect.x = Mathf.Max(rect.x, childDesireSize.x);
                }
            }

            return rect;
        }

        protected override void OnCalcChildrenLayout(IReadOnlyList<UIElement> children)
        {
            Vector2 localPos = default;
            Vector2 contentRect = m_childRoot.rect.size;

            var totalWeight = children.Count > 0 ? children.Sum(c => GetWeight(c)) : 1;

            foreach (var child in children)
            {
                if (LayoutDirection == EnumDirection.Horizontal)
                {
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.anchorMin = new Vector2(0, 1);
                    child.Rect.anchorMax = new Vector2(0, 1);
                    child.Rect.anchoredPosition = localPos;

                    child.ActualWidth = contentRect.x * (GetWeight(child) / totalWeight);
                    child.ActualHeight = contentRect.y;

                    localPos.x += child.ActualWidth;
                }
                else if (LayoutDirection == EnumDirection.Vertical)
                {
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.anchorMin = new Vector2(0, 1);
                    child.Rect.anchorMax = new Vector2(0, 1);
                    child.Rect.anchoredPosition = localPos;


                    child.ActualHeight = contentRect.y * (GetWeight(child) / totalWeight);
                    child.ActualWidth = contentRect.x;

                    localPos.y += child.ActualHeight;
                }
                child.CalcChildrenLayout();
            }
        }
    }
}
