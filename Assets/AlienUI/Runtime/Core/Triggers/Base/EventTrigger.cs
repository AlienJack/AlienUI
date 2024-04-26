using AlienUI.Models;
using AlienUI.UIElements;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core.Triggers
{
    public class EventTrigger : Trigger
    {
        public string Event
        {
            get { return (string)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Event.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof(string), typeof(EventTrigger), string.Empty);


        protected override void OnInit(UIElement host)
        {
            host.OnEventInvoke += Host_OnEventInvoke;
        }

        private void Host_OnEventInvoke(object sender, Events.Event e, string eventName)
        {
            if (eventName == Event)
            {
                Execute();
            }
        }
    }
}
