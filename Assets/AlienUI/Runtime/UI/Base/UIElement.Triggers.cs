using AlienUI.Core.Triggers;
using System.Collections.Generic;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : AmlNodeElement
    {
        private List<Trigger> m_triggers = new List<Trigger>();

        void AddTrigger(Trigger trigger)
        {
            m_triggers.Add(trigger);
            trigger.Init(this);
        }

        void RemoveTrigger(Trigger trigger)
        {
            m_triggers.Remove(trigger);
        }
    }
}
