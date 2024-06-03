using AlienUI.Models;
using AlienUI.Models.Attributes;
using AlienUI.UIElements;

namespace AlienUI.Core.Triggers
{
    [Description(Icon = "trigger")]
    [AllowChild(typeof(TriggerAction))]
    public abstract class Trigger : AmlNodeElement
    {
        private UIElement m_host;

        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(Trigger), new PropertyMetadata(default(DependencyObjectRef)));

        protected DependencyObject m_targetObj { get; private set; }

        protected override void OnParentSet(AmlNodeElement parent)
        {
            if (parent is UIElement host)
            {
                m_host = host;
                m_targetObj = Target.Get(Document) ?? m_host;
                OnInit();
            }
        }
        protected override void OnParentRemoved()
        {
            OnDispose();
        }


        public void Execute()
        {
            var actions = GetChildren<TriggerAction>();
            if (actions == null || actions.Count == 0) return;

            foreach (var action in actions)
            {
                action.Excute();
            }
        }

        protected abstract void OnInit();
        protected abstract void OnDispose();

    }
}
