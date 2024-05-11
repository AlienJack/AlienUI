using AlienUI.Models;
using AlienUI.UIElements;

namespace AlienUI.Core.Triggers
{
    public class EventTrigger : Trigger
    {
        public string Event
        {
            get { return (string)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof(string), typeof(EventTrigger), new PropertyMeta(string.Empty));


        protected override void OnInit()
        {
            if (m_targetObj is UIElement uiTarget)
                uiTarget.OnEventInvoke += Host_OnEventInvoke;
        }

        private void Host_OnEventInvoke(object sender, Events.Event e)
        {
            if (e.EventName == Event)
            {
                Execute();
            }
        }
    }
}
