using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections.Generic;

namespace AlienUI.Core.Triggers
{
    public abstract class Trigger : AmlNodeElement
    {
        private UIElement m_host;
        private List<TriggerAction> m_actions = new List<TriggerAction>();

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

        protected override void OnAddChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case TriggerAction action: m_actions.Add(action); break;
            }
        }

        protected override void OnRemoveChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case TriggerAction action: m_actions.Remove(action); break;
            }
        }

        public void Execute()
        {
            if (m_actions == null || m_actions.Count == 0) return;

            foreach (var action in m_actions)
            {
                action.Excute();
            }
        }

        protected abstract void OnInit();

    }
}
