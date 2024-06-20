using AlienUI.Core.Commnands;
using AlienUI.Models;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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

        public bool IsOpened
        {
            get { return (bool)GetValue(IsOpenedProperty); }
            set { SetValue(IsOpenedProperty, value); }
        }
        public static readonly DependencyProperty IsOpenedProperty =
            DependencyProperty.Register("IsOpened", typeof(bool), typeof(Curtain), new PropertyMetadata(false).AmlDisable());

        protected override void OnInitialized()
        {
            OnDrag += Curtain_OnDrag;
            OnEndDrag += Curtain_OnEndDrag;
        }

        private TweenerCore<float, float, FloatOptions> rollBackTween;
        private void Curtain_OnEndDrag(object sender, Events.OnEndDragEvent e)
        {
            if (IsOpened) return;

            if (CurtainProgress < OpenThreshold)
            {
                if (rollBackTween == null)
                    rollBackTween =
                        DOTween.To(
                            () => CurtainProgress,
                            (x) => CurtainProgress = x,
                            0f,
                            0.5f)
                        .SetEase(Ease.OutQuint).SetAutoKill(false);
                else
                {
                    rollBackTween.ChangeStartValue(CurtainProgress);
                    rollBackTween.Rewind();
                    rollBackTween.Restart();
                }
            }
            else
            {
                IsOpened = true;
                CurtainProgress = 1f;
                Command?.Execute();
                RaiseCustomEvent(new OnCurtainOpened());
            }
        }

        private void Curtain_OnDrag(object sender, Events.OnDragEvent e)
        {
            if (IsOpened) return;

            var tempRoot = m_templateInstance.TemplateRoot.Get(m_templateInstance.Document) as UIElement;

            var deltaValue = e.EvtData.delta.y / Canvas.scaleFactor;
            var endValue = tempRoot.ActualHeight;

            var deltaValuePer = deltaValue / endValue;
            CurtainProgress = Mathf.Clamp01(CurtainProgress + deltaValuePer);

            if (rollBackTween != null)
            {
                rollBackTween.Pause();
            }
        }

        public class OnCurtainOpened : AlienUI.Events.Event
        {
            public override string EventName => nameof(OnCurtainOpened);
        }
    }
}