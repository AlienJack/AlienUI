using AlienUI.UIElements;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core.Triggers
{
    public abstract class Trigger
    {
        private UIElement m_host;
        private List<TriggerAction> m_actions;

        public abstract void ParseByXml(XmlNode xnode);

        public void Init(UIElement host)
        {
            m_host = host;
            OnInit(m_host);
        }

        public void AddAction(TriggerAction action)
        {
            m_actions.Add(action);
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
