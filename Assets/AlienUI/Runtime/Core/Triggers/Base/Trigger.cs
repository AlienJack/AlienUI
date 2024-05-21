using AlienUI.Models;
using AlienUI.Models.Attributes;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;

namespace AlienUI.Core.Triggers
{
    [Description(Icon = "trigger")]
    [AllowChild(typeof(TriggerAction))]
    public abstract class Trigger : AmlNodeElement
    {
        private UIElement m_host;

        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(Trigger), new PropertyMetadata(default(DependencyObjectRef)));

        protected DependencyObject m_targetObj { get; private set; }

        public void Init(UIElement host)
        {
            m_host = host;
            m_targetObj = Target.Get(Document) ?? m_host;
            OnInit();
        }


        public void Execute()
        {
            var actions = GetChildren<TriggerAction>();
            if (actions == null || actions.Count == 0) return;

            foreach (var action in actions)
            {
                action.Excute();
            }
        }

        protected abstract void OnInit();

    }
}
