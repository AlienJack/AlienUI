using AlienUI.Core.Triggers;
using AlienUI.Models;
using System.Collections.Generic;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : DependencyObject
    {
        private List<Trigger> m_triggers = new List<Trigger>();

        void AddTrigger(Trigger trigger)
        {
            m_triggers.Add(trigger);
            trigger.Init(this);
        }
    }
}
