using AlienUI.Models;
using AlienUI.Models.Attributes;
using AlienUI.UIElements.Containers;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.UIElements
{
    [Description(Icon = "grid", Des = "You can divide an area into a grid of x rows and y columns, and you can independently set the height of any row and the width of any column. These dimensions can be specified either by <b>weight</b> or by <b>absolute values</b>. UI Children can determine their own grid position using the <b>AttachedProperty</b> <b>GridPos</b> provided by <b>Grid</b>.")]
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
            DependencyProperty.RegisterAttached("GridPos", typeof(Vector2Int), typeof(Grid), new PropertyMetadata(Vector2Int.zero), OnChildGridPosChanged);

        private static void OnChildGridPosChanged(DependencyObject sender, object oldValue, object newValue)
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

        protected override Vector2 OnCalcDesireSize(IReadOnlyList<UIElement> children)
        {
            return new Vector2(100, 100);
        }

        protected override void OnCalcChildrenLayout(IReadOnlyList<UIElement> children)
        {
            GridDefine.CalcCellSizes(ActualWidth - Padding.left - Padding.right, ActualHeight - Padding.top - Padding.bottom);

            Dictionary<Vector2Int, bool> slots = new Dictionary<Vector2Int, bool>();
            for (int y = 0; y < GridDefine.Row; y++)
            {
                for (int x = 0; x < GridDefine.Column; x++)
                {
                    slots[new Vector2Int(x, y)] = false;
                }
            }

            for (int i = 0; i < UIChildren.Count; i++)
            {
                UIElement uiChild = UIChildren[i];

                var pos = GetGridPos(uiChild);

                pos.x = Mathf.Clamp(pos.x, 0, GridDefine.Column);
                pos.y = Mathf.Clamp(pos.y, 0, GridDefine.Row);

                if (slots[pos])
                {
                    foreach (var item in slots)
                    {
                        if (!item.Value)
                        {
                            pos = item.Key;
                            break;
                        }
                    }
                }

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

                slots[pos] = true;
            }
        }
    }
}
