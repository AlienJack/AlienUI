using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core.Triggers
{
    public abstract class Trigger : DependencyObject
    {
        private UIElement m_host;
        private List<TriggerAction> m_actions = new List<TriggerAction>();

        public void Init(UIElement host)
        {
            m_host = host;
            OnInit(m_host);
        }

        protected override void OnAddChild(DependencyObject childObj)
        {
            switch (childObj)
            {
                case TriggerAction action: m_actions.Add(action); break;
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

        protected abstract void OnInit(UIElement host);

    }
}