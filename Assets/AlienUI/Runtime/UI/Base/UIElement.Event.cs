using AlienUI.Events;
using AlienUI.Models;
using System;
using UnityEngine.EventSystems;
using static AlienUI.UIElements.UIElement;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : DependencyObject
    {
        public delegate void OnEventInvokeHandle(object sender, Event e);
        internal event  OnEventInvokeHandle OnEventInvoke;

        public delegate void OnEventHandle<EV>(object sender, EV e) where EV : Event;


        /*  由于Unity的事件机制问题,PointerEnter和PointerExit总会向上传递
        *   所以AlienUI的事件传递机制,总是不传递PointerEnter和PointerExit事件,而是交由Unity自己传递
        *   在Unity2021之后的版本,这个传递机制可以在EventSystem的InputModule上被关闭,为了保持逻辑统一
        *   应该总是打开向上传递功能(Send Pointer Hover To Parent)
        */

        protected event  OnEventHandle<OnPointerEnterEvent> OnPointerEnter;
        internal void RaisePointerEnterEvent(object sender, OnPointerEnterEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);
            OnPointerEnter?.Invoke(sender, e);
        }

        protected event  OnEventHandle<OnPointerExitEvent> OnPointerExit;
        internal void RaisePointerExitEvent(object sender, OnPointerExitEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);
            OnPointerExit?.Invoke(sender, e);
        }

        protected event  OnEventHandle<OnPointerDownEvent> OnPointerDown;
        internal void RaisePointerDownEvent(object sender, OnPointerDownEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnPointerDown != null)
            {
                OnPointerDown.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaisePointerDownEvent(sender, e);
            }
            else Parent?.RaisePointerDownEvent(sender, e);
        }

        protected event  OnEventHandle<OnPointerUpEvent> OnPointerUp;
        internal void RaisePointerUpEvent(object sender, OnPointerUpEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnPointerUp != null)
            {
                OnPointerUp.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaisePointerUpEvent(sender, e);
            }
            else Parent?.RaisePointerUpEvent(sender, e);
        }

        protected event OnEventHandle<OnPointerClickEvent> OnPointerClick;
        internal void RaisePointerClickEvent(object sender, OnPointerClickEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnPointerClick != null)
            {
                OnPointerClick.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaisePointerClickEvent(sender, e);
            }
            else Parent?.RaisePointerClickEvent(sender, e);
        }

        protected event  OnEventHandle<OnInitializePotentialDragEvent> OnInitializePotentialDrag;
        internal void RaiseInitializePotentialDragEvent(object sender, OnInitializePotentialDragEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnInitializePotentialDrag != null)
            {
                OnInitializePotentialDrag.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseInitializePotentialDragEvent(sender, e);
            }
            else Parent?.RaiseInitializePotentialDragEvent(sender, e);
        }

        protected event  OnEventHandle<OnBeginDragEvent> OnBeginDrag;
        internal void RaiseBeginDrag(object sender, OnBeginDragEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnBeginDrag != null)
            {
                OnBeginDrag.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseBeginDrag(sender, e);
            }
            else Parent?.RaiseBeginDrag(sender, e);
        }

        protected event  OnEventHandle<OnDragEvent> OnDrag;
        internal void RaiseDrag(object sender, OnDragEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnDrag != null)
            {
                OnDrag.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseDrag(sender, e);
            }
            else Parent?.RaiseDrag(sender, e);
        }

        protected event  OnEventHandle<OnEndDragEvent> OnEndDrag;
        internal void RaiseEndDrag(object sender, OnEndDragEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnEndDrag != null)
            {
                OnEndDrag.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseEndDrag(sender, e);
            }
            else Parent?.RaiseEndDrag(sender, e);
        }

        protected event  OnEventHandle<OnDropEvent> OnDrop;
        internal void RaiseDrop(object sender, OnDropEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnDrop != null)
            {
                OnDrop.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseDrop(sender, e);
            }
            else Parent?.RaiseDrop(sender, e);
        }

        protected event  OnEventHandle<OnScrollEvent> OnScroll;
        internal void RaiseScroll(object sender, OnScrollEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnScroll != null)
            {
                OnScroll.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseScroll(sender, e);
            }
            else Parent?.RaiseScroll(sender, e);
        }

        protected event  OnEventHandle<OnUpdateSelectedEvent> OnUpdateSelected;
        internal void RaiseUpdateSelected(object sender, OnUpdateSelectedEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnUpdateSelected != null)
            {
                OnUpdateSelected.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseUpdateSelected(sender, e);
            }
            else Parent?.RaiseUpdateSelected(sender, e);
        }

        protected event  OnEventHandle<OnSelectEvent> OnSelect;
        internal void RaiseSelected(object sender, OnSelectEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnSelect != null)
            {
                OnSelect.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseSelected(sender, e);
            }
            else Parent?.RaiseSelected(sender, e);
        }

        protected event  OnEventHandle<OnDeselectEvent> OnDeselect;
        internal void RaiseDeselect(object sender, OnDeselectEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnDeselect != null)
            {
                OnDeselect.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseDeselect(sender, e);
            }
            else Parent?.RaiseDeselect(sender, e);
        }

        protected event  OnEventHandle<OnMoveEvent> OnMove;
        internal void RaiseMove(object sender, OnMoveEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnMove != null)
            {
                OnMove.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseMove(sender, e);
            }
            else Parent?.RaiseMove(sender, e);
        }

        protected event  OnEventHandle<OnSubmitEvent> OnSubmit;
        internal void RaiseSubmit(object sender, OnSubmitEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnSubmit != null)
            {
                OnSubmit.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseSubmit(sender, e);
            }
            else Parent?.RaiseSubmit(sender, e);
        }

        protected event  OnEventHandle<OnCancelEvent> OnCancel;
        internal void RaiseCancel(object sender, OnCancelEvent e)
        {
            OnEventInvoke?.Invoke(sender, e);

            if (OnCancel != null)
            {
                OnCancel.Invoke(sender, e);
                if (e.Canceled && Parent != null) Parent.RaiseCancel(sender, e);
            }
            else Parent?.RaiseCancel(sender, e);
        }
    }
}
