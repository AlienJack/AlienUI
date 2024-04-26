using AlienUI.UIElements;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core.Triggers
{
    public class EventTrigger : Trigger
    {
        private string m_eventName;

        public override void ParseByXml(XmlNode xnode)
        {
            var xatt = xnode.Attributes["Event"];
            if (xatt == null) return;

            m_eventName = xatt.Value;
        }

        protected override void OnInit(UIElement host)
        {
            host.OnEventInvoke += Host_OnEventInvoke;
        }

        private void Host_OnEventInvoke(object sender, Events.Event e, string eventName)
        {
            if (eventName == m_eventName)
            {
                Execute();
            }
        }
    }
}
