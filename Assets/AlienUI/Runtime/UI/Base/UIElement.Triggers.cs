using AlienUI.Core.Triggers;
using System.Collections.Generic;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : XmlNodeElement
    {
        private List<Trigger> m_triggers = new List<Trigger>();

        void AddTrigger(Trigger trigger)
        {
            m_triggers.Add(trigger);
            trigger.Init(this);
        }
    }
}
