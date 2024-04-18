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
            DependencyProperty.Register("GridDefine", typeof(GridDefine), typeof(Grid), GridDefine.Default, OnLayoutParamDirty);

        protected override Float2 CalcDesireSize()
        {
            Float2 result = new Float2(Padding.left + Padding.right, Padding.top + Padding.bottom);

            foreach (UIElement child in Children)
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
            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = (UIElement)Children[i];
                int x = i % GridDefine.Column;
                int y = i / GridDefine.Column;
                var size = GridDefine.GetCellSize(x, y);
                child.ActualWidth = size.x;
                child.ActualHeight = size.y;
                child.Rect.pivot = new Vector2(0, 1);
                child.Rect.anchorMin = new Vector2(0, 1);
                child.Rect.anchorMax = new Vector2(0, 1);
                child.Rect.anchoredPosition = GridDefine.GetCellOffset(x, y);

                child.CalcChildrenLayout();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}
