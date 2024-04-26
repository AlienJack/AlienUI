using AlienUI.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AlienUI.Events
{
    public abstract class Event
    {
        public virtual bool Canceled { get; private set; }
        /// <summary> make event cancel for dispatch to parent</summary>
        public void Cancel()
        {
            Canceled = true;
        }
    }

    public abstract class Event<UnityEventData> : Event
    {
        public UnityEventData EvtData { get; private set; }

        public Event(UnityEventData eventData)
        {
            EvtData = eventData;
        }
    }

    public class OnMouseEnterEvent : Event<PointerEventData>
    {
        public OnMouseEnterEvent(PointerEventData eventData) : base(eventData) { }
    }

    public class OnMouseExitEvent : Event<PointerEventData>
    {
        public OnMouseExitEvent(PointerEventData eventData) : base(eventData) { }
    }
}
