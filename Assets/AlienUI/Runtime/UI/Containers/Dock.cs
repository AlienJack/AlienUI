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



        public float Space
        {
            get { return (float)GetValue(SpaceProperty); }
            set { SetValue(SpaceProperty, value); }
        }
        public static readonly DependencyProperty SpaceProperty =
            DependencyProperty.Register("Space", typeof(float), typeof(Dock), new PropertyMetadata(0f), OnLayoutParamDirty);

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

            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];

                var childDesireSize = child.GetDesireSize();
                if (LayoutDirection == EnumDirection.Horizontal)
                {
                    rect.x += childDesireSize.x + (i == children.Count - 1 ? 0 : Space);
                    rect.y = Mathf.Max(rect.y, childDesireSize.y);
                }
                else if (LayoutDirection == EnumDirection.Vertical)
                {
                    rect.y += childDesireSize.y + (i == children.Count - 1 ? 0 : Space);
                    rect.x = Mathf.Max(rect.x, childDesireSize.x);
                }
            }

            return rect;
        }

        protected override void OnCalcChildrenLayout(IReadOnlyList<UIElement> children)
        {
            Vector2 localPos = default;
            Vector2 contentRect = m_childRoot.rect.size;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (LayoutDirection == EnumDirection.Horizontal)
                {
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.anchorMin = new Vector2(0, 1);
                    child.Rect.anchorMax = new Vector2(0, 1);
                    child.Rect.anchoredPosition = localPos;

                    var childDesireSize = child.GetDesireSize();
                    child.ActualWidth = childDesireSize.x;
                    child.ActualHeight = ControlChildSize ? contentRect.y : childDesireSize.y;

                    localPos.x += child.ActualWidth + (i == children.Count - 1 ? 0 : Space);
                }
                else if (LayoutDirection == EnumDirection.Vertical)
                {
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.pivot = new Vector2(0, 1);
                    child.Rect.anchorMin = new Vector2(0, 1);
                    child.Rect.anchorMax = new Vector2(0, 1);
                    child.Rect.anchoredPosition = localPos;

                    var childDesireSize = child.GetDesireSize();
                    child.ActualHeight = childDesireSize.y;
                    child.ActualWidth = ControlChildSize ? contentRect.x : childDesireSize.y;

                    localPos.y -= child.ActualHeight + (i == children.Count - 1 ? 0 : Space);
                }
                child.CalcChildrenLayout();
            }
        }
    }
}
