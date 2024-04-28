using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : DependencyObject
    {
        public Number Width
        {
            get { return (Number)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(Number), typeof(UIElement), Number.Identity, OnLayoutParamDirty);

        public Number Height
        {
            get { return GetValue<Number>(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(Number), typeof(UIElement), Number.Identity, OnLayoutParamDirty);

        public eHorizontalAlign Horizontal
        {
            get { return (eHorizontalAlign)GetValue(HorizontalProperty); }
            set { SetValue(HorizontalProperty, value); }
        }

        public static readonly DependencyProperty HorizontalProperty =
            DependencyProperty.Register("Horizontal", typeof(eHorizontalAlign), typeof(UIElement), eHorizontalAlign.Middle, OnLayoutParamDirty);

        public eVerticalAlign Vertical
        {
            get { return (eVerticalAlign)GetValue(VerticalProperty); }
            set { SetValue(VerticalProperty, value); }
        }

        public static readonly DependencyProperty VerticalProperty =
            DependencyProperty.Register("Vertical", typeof(eVerticalAlign), typeof(UIElement), eVerticalAlign.Middle, OnLayoutParamDirty);



        public Border Padding
        {
            get { return (Border)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Border), typeof(UIElement), default(Border), OnLayoutParamDirty);

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

        public Float2 GetDesireSize()
        {
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

        private static void PerformUIElement(UIElement ui)
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

        protected void SetLayoutDirty()
        {
            Engine?.SetDirty(this);
        }

        protected abstract Float2 CalcDesireSize();
    }
}
