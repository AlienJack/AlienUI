using AlienUI.Models;
using AlienUI.UIElements.Containers;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Dock : Container
    {
        public EnumDirection LayoutDirection
        {
            get { return (EnumDirection)GetValue(LayoutDirectionProperty); }
            set { SetValue(LayoutDirectionProperty, value); }
        }

        public static readonly DependencyProperty LayoutDirectionProperty =
            DependencyProperty.Register("LayoutDirection", typeof(EnumDirection), typeof(Dock), new PropertyMetadata(EnumDirection.Horizontal), OnLayoutParamDirty);



        public bool ControlChildSize
        {
            get { return (bool)GetValue(ControlChildSizeProperty); }
            set { SetValue(ControlChildSizeProperty, value); }
        }

        public static readonly DependencyProperty ControlChildSizeProperty =
            DependencyProperty.Register("ControlChildSize", typeof(bool), typeof(Dock), new PropertyMetadata(false), OnLayoutParamDirty);


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
            foreach (var child in children)
            {
                if (LayoutDirection == EnumDirection.Horizontal)
                {
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.anchorMin = new Vector2(0, 1);
                    child.Rect.anchorMax = new Vector2(0, 1);
                    child.Rect.anchoredPosition = localPos;

                    var childDesireSize = child.GetDesireSize();
                    child.ActualWidth = childDesireSize.x;
                    child.ActualHeight = ControlChildSize ? contentRect.y : childDesireSize.y;

                    localPos.x += child.ActualWidth;
                }
                else if (LayoutDirection == EnumDirection.Vertical)
                {
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.anchorMin = new Vector2(0, 1);
                    child.Rect.anchorMax = new Vector2(0, 1);
                    child.Rect.anchoredPosition = localPos;

                    var childDesireSize = child.GetDesireSize();
                    child.ActualWidth = childDesireSize.x;
                    child.ActualHeight = ControlChildSize ? contentRect.x : childDesireSize.y;

                    localPos.y += child.ActualHeight;
                }
                child.CalcChildrenLayout();
            }
        }
    }
}
