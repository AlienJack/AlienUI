using AlienUI.Models;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : AmlNodeElement
    {
        public Number Width
        {
            get { return (Number)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(Number), typeof(UIElement), new PropertyMetadata(Number.Identity, "Layout"), OnLayoutParamDirty);

        public Number Height
        {
            get { return GetValue<Number>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(Number), typeof(UIElement), new PropertyMetadata(Number.Identity, "Layout"), OnLayoutParamDirty);

        public eHorizontalAlign Horizontal
        {
            get { return (eHorizontalAlign)GetValue(HorizontalProperty); }
            set { SetValue(HorizontalProperty, value); }
        }

        public static readonly DependencyProperty HorizontalProperty =
            DependencyProperty.Register("Horizontal", typeof(eHorizontalAlign), typeof(UIElement), new PropertyMetadata(eHorizontalAlign.Middle, "Layout"), OnLayoutParamDirty);

        public eVerticalAlign Vertical
        {
            get { return (eVerticalAlign)GetValue(VerticalProperty); }
            set { SetValue(VerticalProperty, value); }
        }

        public static readonly DependencyProperty VerticalProperty =
            DependencyProperty.Register("Vertical", typeof(eVerticalAlign), typeof(UIElement), new PropertyMetadata(eVerticalAlign.Middle, "Layout"), OnLayoutParamDirty);

        public BorderData Padding
        {
            get { return (BorderData)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(BorderData), typeof(UIElement), new PropertyMetadata(default(BorderData), "Layout"), OnLayoutParamDirty);


        public Vector2 Offset
        {
            get { return (Vector2)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(Vector2), typeof(UIElement), new PropertyMetadata(Vector2.zero, "Layout"), OnLayoutParamDirty);




        public Vector2 Scale
        {
            get { return (Vector2)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(Vector2), typeof(UIElement), new PropertyMetadata(Vector2.one), OnScaleChanged);

        private static void OnScaleChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as UIElement;
            self.Rect.localScale = (Vector2)newValue;
        }


        public bool Active
        {
            get { return (bool)GetValue(ActiveProperty); }
            set { SetValue(ActiveProperty, value); }
        }

        public static readonly DependencyProperty ActiveProperty =
            DependencyProperty.Register("Active", typeof(bool), typeof(UIElement), new PropertyMetadata(true), OnActiveChanged);

        private static void OnActiveChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as UIElement;
            self.Rect.gameObject.SetActive(self.Active);

            self.SetLayoutDirty();
        }

        public float ActualWidth
        {
            get => m_rectTransform.rect.width;
            set
            {
                m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
            }
        }

        public float ActualHeight
        {
            get => m_rectTransform.rect.height;
            set
            {
                m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
            }
        }

        public Vector2 GetDesireSize()
        {
            if (!Active)
                return Vector2.zero;

            var desireSize = CalcDesireSize();

            if (!Width.Auto) desireSize.x = Width.Value;
            if (!Height.Auto) desireSize.y = Height.Value;

            return desireSize;
        }

        public void BeginLayout()
        {
            Debug.Assert(Parent == null); //只允许顶层的UI元素发起布局计算

            PerformUIElement(this);
            HandleChildRoot();

            BeginChildrenLayout();
        }

        private void BeginChildrenLayout()
        {
            CalcChildrenLayout();

            foreach (var child in UIChildren)
            {
                child.BeginChildrenLayout();
            }
        }

        private void HandleChildRoot()
        {
            m_childRoot.SetStretchModeOffsets(Padding);

            foreach (var child in UIChildren)
            {
                child.HandleChildRoot();
            }
        }

        internal static void PerformUIElement(UIElement ui)
        {
            ui.PerformRectTransform();

            var desireSize = ui.GetDesireSize();

            if (ui.Horizontal == eHorizontalAlign.Stretch)
            {
                var temp = ui.m_rectTransform.sizeDelta;
                temp.x = 0;
                ui.m_rectTransform.sizeDelta = temp;
            }
            else
                ui.ActualWidth = desireSize.x;
            if (ui.Vertical == eVerticalAlign.Stretch)
            {
                var temp = ui.m_rectTransform.sizeDelta;
                temp.y = 0;
                ui.m_rectTransform.sizeDelta = temp;
            }
            else
                ui.ActualHeight = desireSize.y;

            ui.m_rectTransform.SetPivotStayPosition(new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// 重写这个方法以控制子UI元素的布局
        /// </summary>
        public virtual void CalcChildrenLayout()
        {
            foreach (var child in UIChildren)
            {
                PerformUIElement(child);
            }
        }

        protected static void OnLayoutParamDirty(DependencyObject sender, object oldValue, object newValue)
        {
            if (sender is UIElement uiEle)
            {
                uiEle.Engine?.SetDirty(uiEle);
            }
        }

        public void SetLayoutDirty()
        {
            Engine?.SetDirty(this);
        }

        protected virtual Vector2 CalcDesireSize()
        {
            var rect = new Vector2(Padding.left + Padding.right, Padding.top + Padding.bottom);

            Vector2 addRect = default;
            foreach (var child in UIChildren)
            {
                var deSize = child.GetDesireSize();
                addRect.x = Mathf.Max(addRect.x, deSize.x);
                addRect.y = Mathf.Max(addRect.y, deSize.y);
            }

            return rect + addRect;
        }
    }
}
