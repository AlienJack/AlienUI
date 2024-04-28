using AlienUI.Events;
using AlienUI.Models;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : DependencyObject
    {
        public delegate void OnEventInvokeHandle(object sender, Event e, string eventName);
        public event OnEventInvokeHandle OnEventInvoke;


        /*  由于Unity的事件机制问题,PointerEnter和PointerExit总会向上传递
        *   所以AlienUI的事件传递机制,总是不传递MouseEnter和MouseExit事件,而是交由Unity自己传递
        *   在Unity2021之后的版本,这个传递机制可以在EventSystem的InputModule上被关闭,为了保持逻辑统一
        *   应该总是打开向上传递功能(Send Pointer Hover To Parent)
        */

        public delegate void OnMouseEnterHandle(object sender, OnMouseEnterEvent e);
        public event OnMouseEnterHandle OnMouseEnter;
        public void RaiseMouseEnterEvent(object sender, OnMouseEnterEvent e)
        {
            OnEventInvoke?.Invoke(sender, e, "OnMouseEnter");
            OnMouseEnter?.Invoke(sender, e);
        }

        public delegate void OnMouseExitHandle(object sender, OnMouseExitEvent e);
        public event OnMouseExitHandle OnMouseExit;
        public void RaiseMouseExitEvent(object sender, OnMouseExitEvent e)
        {
            OnEventInvoke?.Invoke(sender, e, "OnMouseExit");
            OnMouseExit?.Invoke(sender, e);
        }

        public delegate void OnMouseDownHandle(object sender, OnMouseDownEvent e);
        public event OnMouseDownHandle OnMousePress;
        public void RaiseMouseDownEvent(object sender, OnMouseDownEvent e)
        {
            OnEventInvoke?.Invoke(sender, e, "OnMouseDown");

            if (OnMousePress != null)
            {
                OnMousePress.Invoke(sender, e);
                if (e.Canceled && Parent != null)
                {
                    Parent.RaiseMouseDownEvent(sender, e);
                }
            }
            else
            {
                Parent?.RaiseMouseDownEvent(sender, e);
            }
        }
    }
}
