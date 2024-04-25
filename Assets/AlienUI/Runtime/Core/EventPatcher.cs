using AlienUI.UIElements;
using UnityEngine;

namespace AlienUI.Events
{
    public abstract class Event
    {
        public bool Cancel { get; set; } = true;
    }

    public class OnMouseEnterEvent : Event
    {

    }
}
