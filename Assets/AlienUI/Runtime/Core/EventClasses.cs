using AlienUI.UIElements;
using UnityEngine.EventSystems;

namespace AlienUI.Events
{
    public abstract class Event
    {
        public abstract string EventName { get; }
        public bool Canceled { get; private set; }
        /// <summary> �����¼�ȡ��,���򸸼����� </summary>
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

    public class OnPointerEnterEvent : Event<PointerEventData>
    {
        public OnPointerEnterEvent(PointerEventData eventData) : base(eventData) { }

        public static implicit operator OnPointerEnterEvent(PointerEventData uguiEvtData) => new(uguiEvtData);

        public override string EventName => "OnPointerEnter";
    }

    public class OnPointerExitEvent : Event<PointerEventData>
    {
        public OnPointerExitEvent(PointerEventData eventData) : base(eventData) { }

        public static implicit operator OnPointerExitEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
        public override string EventName => "OnPointerExit";
    }

    public class OnPointerDownEvent : Event<PointerEventData>
    {
        public OnPointerDownEvent(PointerEventData eventData) : base(eventData) { }
        public static implicit operator OnPointerDownEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
        public override string EventName => "OnPointerDown";
    }

    public class OnPointerUpEvent : Event<PointerEventData>
    {
        public OnPointerUpEvent(PointerEventData eventData) : base(eventData) { }
        public static implicit operator OnPointerUpEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
        public override string EventName => "OnPointerUp";
    }

    public class OnPointerClickEvent : Event<PointerEventData>
    {
        public OnPointerClickEvent(PointerEventData eventData) : base(eventData) { }
        public override string EventName => "OnPointerClick";
        public static implicit operator OnPointerClickEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
    }

    public class OnInitializePotentialDragEvent : Event<PointerEventData>
    {
        public OnInitializePotentialDragEvent(PointerEventData eventData) : base(eventData) { }

        public override string EventName => "OnInitializePotentialDrag";
        public static implicit operator OnInitializePotentialDragEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
    }

    public class OnBeginDragEvent : Event<PointerEventData>
    {
        public OnBeginDragEvent(PointerEventData eventData) : base(eventData) { }

        public override string EventName => "OnBeginDrag";
        public static implicit operator OnBeginDragEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
    }

    public class OnDragEvent : Event<PointerEventData>
    {
        public OnDragEvent(PointerEventData eventData) : base(eventData) { }

        public override string EventName => "OnDrag";
        public static implicit operator OnDragEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
    }
    public class OnEndDragEvent : Event<PointerEventData>
    {
        public OnEndDragEvent(PointerEventData eventData) : base(eventData) { }

        public override string EventName => "OnEndDrag";
        public static implicit operator OnEndDragEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
    }
    public class OnDropEvent : Event<PointerEventData>
    {
        public OnDropEvent(PointerEventData eventData) : base(eventData) { }

        public override string EventName => "OnDrop";
        public static implicit operator OnDropEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
    }

    public class OnScrollEvent : Event<PointerEventData>
    {
        public OnScrollEvent(PointerEventData eventData) : base(eventData) { }

        public override string EventName => "OnScroll";
        public static implicit operator OnScrollEvent(PointerEventData uguiEvtData) => new(uguiEvtData);
    }

    public class OnShowEvent : Event<UIElement>
    {
        public OnShowEvent(UIElement eventData) : base(eventData) { }

        public override string EventName => "OnShow";
    }

    public class OnCloseEvent : Event<UIElement>
    {
        public OnCloseEvent(UIElement eventData) : base(eventData) { }

        public override string EventName => "OnClose";
    }

}
