using AlienUI.Core.Commnands;
using AlienUI.Events;
using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Curtain : UserControl
    {
        public override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.Curtain");

        public float OpenThreshold
        {
            get { return (float)GetValue(OpenThresholdProperty); }
            set { SetValue(OpenThresholdProperty, value); }
        }

        public static readonly DependencyProperty OpenThresholdProperty =
            DependencyProperty.Register("OpenThreshold", typeof(float), typeof(float), new PropertyMetadata(0.3f));

        public float CurtainProgress
        {
            get { return (float)GetValue(CurtainProgressProperty); }
            set { SetValue(CurtainProgressProperty, value); }
        }
        public static readonly DependencyProperty CurtainProgressProperty =
            DependencyProperty.Register("CurtainProgress", typeof(float), typeof(float), new PropertyMetadata(0f), OnCurtainProgressChanged);



        public CommandBase Command
        {
            get { return (CommandBase)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(CommandBase), typeof(Curtain), new PropertyMetadata(null));


        private static void OnCurtainProgressChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Curtain;
            var tempRoot = self.m_templateInstance.TemplateRoot.Get(self.m_templateInstance.Document) as UIElement;
            var currentOffset = tempRoot.ActualHeight * self.CurtainProgress;
            tempRoot.Offset = new Vector2(0, currentOffset);

            tempRoot.Alpha = 1f - (self.CurtainProgress / self.OpenThreshold);
        }

        protected override void OnInitialized()
        {
            OnDrag += Curtain_OnDrag;
            OnEndDrag += Curtain_OnEndDrag;
        }

        private void Curtain_OnEndDrag(object sender, Events.OnEndDragEvent e)
        {
            if (CurtainProgress < OpenThreshold)
            {
                CurtainProgress = 0f;
            }
            else
            {
                CurtainProgress = 1f;
                Command?.Execute();
                RaiseCustomEvent(new OnCurtainOpened());
            }
        }

        private void Curtain_OnDrag(object sender, Events.OnDragEvent e)
        {
            var tempRoot = m_templateInstance.TemplateRoot.Get(m_templateInstance.Document) as UIElement;

            var deltaValue = e.EvtData.delta.y / Canvas.scaleFactor;
            var endValue = tempRoot.ActualHeight;

            var deltaValuePer = deltaValue / endValue;
            CurtainProgress = Mathf.Clamp01(CurtainProgress + deltaValuePer);
        }

        public class OnCurtainOpened : AlienUI.Events.Event
        {
            public override string EventName => nameof(OnCurtainOpened);
        }
    }
}