using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Track : UserControl
    {
        public override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.Track");



        public EnumDirection Direction
        {
            get { return (EnumDirection)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(EnumDirection), typeof(Track), new PropertyMetadata(default(EnumDirection)), OnLayoutParamDirty);


        public float Progress
        {
            get { return (float)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(float), typeof(Track), new PropertyMetadata(0f), OnLayoutParamDirty);

        private void UpdateElementPosition()
        {
            if (m_trackRoot == null) return;

            foreach (var trackChild in m_trackRoot.UIChildren)
            {
                var parent = trackChild.Rect.parent as RectTransform;

                float start = default;
                float end = default;

                if (Direction == EnumDirection.Horizontal)
                {
                    switch (trackChild.Horizontal)
                    {
                        case eHorizontalAlign.Left:
                            start = 0f;
                            end = parent.rect.width - trackChild.ActualWidth;
                            break;
                        case eHorizontalAlign.Right:
                            start = (parent.rect.width - trackChild.ActualWidth) * -1;
                            end = 0f;
                            break;
                        case eHorizontalAlign.Stretch:
                        case eHorizontalAlign.Middle:
                            start = (parent.rect.width - trackChild.ActualWidth) * -0.5f;
                            end = (parent.rect.width - trackChild.ActualWidth) * 0.5f;
                            break;
                    }

                    var temp = trackChild.Offset;
                    temp.x = Mathf.LerpUnclamped(start, end, Progress);
                    trackChild.Offset = temp;
                }
                else
                {
                    switch (trackChild.Vertical)
                    {
                        case eVerticalAlign.Bottom:
                            start = 0f;
                            end = parent.rect.height - trackChild.ActualHeight;
                            break;
                        case eVerticalAlign.Top:
                            start = (parent.rect.height - trackChild.ActualHeight) * -1;
                            end = 0f;
                            break;
                        case eVerticalAlign.Stretch:
                        case eVerticalAlign.Middle:
                            start = (parent.rect.height - trackChild.ActualHeight) * -0.5f;
                            end = (parent.rect.height - trackChild.ActualHeight) * 0.5f;
                            break;
                    }

                    var temp = trackChild.Offset;
                    temp.y = Mathf.LerpUnclamped(start, end, Progress);
                    trackChild.Offset = temp;
                }

            }

        }

        private UIElement m_trackRoot;

        protected override void OnTemplateApply()
        {
            m_trackRoot = m_templateInstance.TemplateRoot.Get(m_templateInstance) as UIElement;
            UpdateElementPosition();
        }

        protected override void OnInitialized()
        {
        }

        public override void CalcChildrenLayout()
        {
            base.CalcChildrenLayout();
            UpdateElementPosition();
        }
    }
}