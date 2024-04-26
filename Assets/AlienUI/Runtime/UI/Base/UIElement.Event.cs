using AlienUI.Events;
using AlienUI.Models;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : DependencyObject
    {
        public delegate void OnEventInvokeHandle(object sender, Event e, string eventName);
        public event OnEventInvokeHandle OnEventInvoke;


        /*  ����Unity���¼���������,PointerEnter��PointerExit�ܻ����ϴ���
        *   ����AlienUI���¼����ݻ���,���ǲ�����MouseEnter��MouseExit�¼�,���ǽ���Unity�Լ�����
        *   ͨ����Canceled�������Ƿ���false,����ֹAlienUI�¼������ϴ���
        *   ��Unity2021֮��İ汾,������ݻ��ƿ�����EventSystem��InputModule�ϱ��ر�,Ϊ�˱����߼�ͳһ
        *   Ӧ�����Ǵ����ϴ��ݹ���(Send Pointer Hover To Parent)
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
    }
}
