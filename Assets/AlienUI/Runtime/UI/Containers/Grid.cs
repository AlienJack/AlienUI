using AlienUI.Models;
using AlienUI.UIElements.Containers;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Grid : Container
    {
        public GridDefine GridDefine
        {
            get { return (GridDefine)GetValue(GridDefineProperty); }
            set { SetValue(GridDefineProperty, value); }
        }

        public static readonly DependencyProperty GridDefineProperty =
            DependencyProperty.Register("GridDefine", typeof(GridDefine), typeof(Grid), new PropertyMeta(GridDefine.Default, "Layout"), OnLayoutParamDirty);

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

                int x = i % GridDefine.Column;
                int y = i / GridDefine.Column;
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
