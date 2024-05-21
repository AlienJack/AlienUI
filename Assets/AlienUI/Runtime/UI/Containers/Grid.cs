using AlienUI.Models;
using AlienUI.Models.Attributes;
using AlienUI.UIElements.Containers;
using UnityEngine;

namespace AlienUI.UIElements
{
    [Description(Icon = "grid")]
    public class Grid : Container
    {
        public static Vector2Int GetGridPos(DependencyObject obj)
        {
            return (Vector2Int)obj.GetValue(PositionProperty);
        }

        public static void SetGridPos(DependencyObject obj, Vector2Int value)
        {
            obj.SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("GridPos", typeof(Vector2Int), typeof(Grid), new PropertyMetadata(Vector2Int.zero), OnValueChanged);

        private static void OnValueChanged(DependencyObject sender, object oldValue, object newValue)
        {
            if (sender is UIElement element && element.Parent is Grid grid)
            {
                grid.SetLayoutDirty();
            }
        }

        public GridDefine GridDefine
        {
            get { return (GridDefine)GetValue(GridDefineProperty); }
            set { SetValue(GridDefineProperty, value); }
        }

        public static readonly DependencyProperty GridDefineProperty =
            DependencyProperty.Register("GridDefine", typeof(GridDefine), typeof(Grid), new PropertyMetadata(GridDefine.Default), OnLayoutParamDirty);

        protected override Vector2 CalcDesireSize()
        {
            Vector2 result = new Vector2(Padding.left + Padding.right, Padding.top + Padding.bottom);

            foreach (var child in UIChildren)
            {
                var childSize = child.GetDesireSize();
                result.x += childSize.x;
                result.y += childSize.y;
            }

            return result;
        }

        public override void CalcChildrenLayout()
        {
            GridDefine.CalcCellSizes(ActualWidth - Padding.left - Padding.right, ActualHeight - Padding.top - Padding.bottom);
            for (int i = 0; i < UIChildren.Count; i++)
            {
                UIElement uiChild = UIChildren[i];

                var pos = GetGridPos(uiChild);

                int x = pos.x;
                int y = pos.y;
                var size = GridDefine.GetCellSize(x, y);
                uiChild.ActualWidth = size.x;
                uiChild.ActualHeight = size.y;
                uiChild.Rect.pivot = new Vector2(0, 1);
                uiChild.Rect.anchorMin = new Vector2(0, 1);
                uiChild.Rect.anchorMax = new Vector2(0, 1);
                uiChild.Rect.anchoredPosition = GridDefine.GetCellOffset(x, y);

                uiChild.CalcChildrenLayout();
            }
        }
    }
}
