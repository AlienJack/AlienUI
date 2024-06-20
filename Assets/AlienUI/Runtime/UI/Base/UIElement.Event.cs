using AlienUI.Events;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : AmlNodeElement
    {
        public delegate void OnEventInvokeHandle(object sender, Event e);
        internal event OnEventInvokeHandle OnEventForTriggerInvoke;

        public delegate void OnEventHandle<EV>(object sender, EV e) where EV : Event;

        public void RaiseCustomEvent(Event evt)
        {
            OnEventForTriggerInvoke?.Invoke(this, evt);
        }

        protected event OnEventHandle<OnShowEvent> OnShow;
        internal void RaiseShow()
        {
            var evt = new OnShowEvent(this);

            //不要在编辑器模式下触发show,避免一些trigger动画改变属性
            if (UnityEngine.Application.isPlaying) OnEventForTriggerInvoke?.Invoke(this, evt);
            OnShow?.Invoke(this, evt);

            foreach (var child in UIChildren)
                child.RaiseShow();
        }

        public event OnEventHandle<OnCloseEvent> OnClose;
        internal void RaiseClose()
        {
            var evt = new OnCloseEvent(this);
            OnEventForTriggerInvoke?.Invoke(this, evt);
            OnClose?.Invoke(this, evt);

            foreach (var child in UIChildren) child.RaiseClose();
        }

        #region UGUI Events
        /*  由于Unity的事件机制问题,PointerEnter和PointerExit总会向上传递
        *   所以AlienUI的事件传递机制,总是不传递PointerEnter和PointerExit事件,而是交由Unity自己传递
        *   在Unity2021之后的版本,这个传递机制可以在EventSystem的InputModule上被关闭,为了保持逻辑统一
        *   应该总是打开向上传递功能(Send Pointer Hover To UIParent)
        */

        protected event OnEventHandle<OnPointerEnterEvent> OnPointerEnter;
        internal void RaisePointerEnterEvent(object sender, OnPointerEnterEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);
            OnPointerEnter?.Invoke(sender, e);
        }

        protected event OnEventHandle<OnPointerExitEvent> OnPointerExit;
        internal void RaisePointerExitEvent(object sender, OnPointerExitEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);
            OnPointerExit?.Invoke(sender, e);
        }

        protected event OnEventHandle<OnPointerDownEvent> OnPointerDown;
        internal void RaisePointerDownEvent(object sender, OnPointerDownEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnPointerDown != null)
            {
                OnPointerDown.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaisePointerDownEvent(sender, e);
            }
            else UIParent?.RaisePointerDownEvent(sender, e);
        }

        protected event OnEventHandle<OnPointerUpEvent> OnPointerUp;
        internal void RaisePointerUpEvent(object sender, OnPointerUpEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnPointerUp != null)
            {
                OnPointerUp.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaisePointerUpEvent(sender, e);
            }
            else UIParent?.RaisePointerUpEvent(sender, e);
        }

        protected event OnEventHandle<OnPointerClickEvent> OnPointerClick;
        internal void RaisePointerClickEvent(object sender, OnPointerClickEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnPointerClick != null)
            {
                OnPointerClick.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaisePointerClickEvent(sender, e);
            }
            else UIParent?.RaisePointerClickEvent(sender, e);
        }

        protected event OnEventHandle<OnInitializePotentialDragEvent> OnInitializePotentialDrag;
        internal void RaiseInitializePotentialDragEvent(object sender, OnInitializePotentialDragEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnInitializePotentialDrag != null)
            {
                OnInitializePotentialDrag.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaiseInitializePotentialDragEvent(sender, e);
            }
            else UIParent?.RaiseInitializePotentialDragEvent(sender, e);
        }

        protected event OnEventHandle<OnBeginDragEvent> OnBeginDrag;
        internal void RaiseBeginDrag(object sender, OnBeginDragEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnBeginDrag != null)
            {
                OnBeginDrag.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaiseBeginDrag(sender, e);
            }
            else UIParent?.RaiseBeginDrag(sender, e);
        }

        protected event OnEventHandle<OnDragEvent> OnDrag;
        internal void RaiseDrag(object sender, OnDragEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnDrag != null)
            {
                OnDrag.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaiseDrag(sender, e);
            }
            else UIParent?.RaiseDrag(sender, e);
        }

        protected event OnEventHandle<OnEndDragEvent> OnEndDrag;
        internal void RaiseEndDrag(object sender, OnEndDragEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnEndDrag != null)
            {
                OnEndDrag.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaiseEndDrag(sender, e);
            }
            else UIParent?.RaiseEndDrag(sender, e);
        }

        protected event OnEventHandle<OnDropEvent> OnDrop;
        internal void RaiseDrop(object sender, OnDropEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnDrop != null)
            {
                OnDrop.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaiseDrop(sender, e);
            }
            else UIParent?.RaiseDrop(sender, e);
        }

        protected event OnEventHandle<OnScrollEvent> OnScroll;
        internal void RaiseScroll(object sender, OnScrollEvent e)
        {
            OnEventForTriggerInvoke?.Invoke(sender, e);

            if (OnScroll != null)
            {
                OnScroll.Invoke(sender, e);
                if (e.Canceled && UIParent != null) UIParent.RaiseScroll(sender, e);
            }
            else UIParent?.RaiseScroll(sender, e);
        }

        #endregion
    }
}
