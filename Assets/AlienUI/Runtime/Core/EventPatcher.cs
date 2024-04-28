using AlienUI.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AlienUI.Events
{
    public abstract class Event
    {
        public bool Canceled { get; private set; }
        /// <summary> 设置事件取消,以向父级传递 </summary>
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

        public static implicit operator OnMouseEnterEvent(PointerEventData uguiEvtData) => new OnMouseEnterEvent(uguiEvtData);
    }

    public class OnMouseExitEvent : Event<PointerEventData>
    {
        public OnMouseExitEvent(PointerEventData eventData) : base(eventData) { }

        public static implicit operator OnMouseExitEvent(PointerEventData uguiEvtData) => new OnMouseExitEvent(uguiEvtData);
    }

    public class OnMouseDownEvent : Event<PointerEventData>
    {
        public OnMouseDownEvent(PointerEventData eventData) : base(eventData) { }
        public static implicit operator OnMouseDownEvent(PointerEventData uguiEvtData) => new OnMouseDownEvent(uguiEvtData);
    }

    public class OnMouseUpEvent : Event<PointerEventData>
    {
        public OnMouseUpEvent(PointerEventData eventData) : base(eventData) { }
        public static implicit operator OnMouseUpEvent(PointerEventData uguiEvtData) => new OnMouseUpEvent(uguiEvtData);
    }
}
