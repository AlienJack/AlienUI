using AlienUI.Models;
using AlienUI.UIElements.ToolsScript;
using System.Collections;
using UnityEngine;

namespace AlienUI.Core.Triggers
{
    public class Setter : TriggerAction
    {
        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(Setter), new PropertyMetadata(default));


        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(Setter), new PropertyMetadata(null));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(Setter), new PropertyMetadata(null));

        private DependencyObject m_target;
        public override void OnInit(Trigger trigger)
        {
            m_target = Target.Get(this);

            if (m_target == null)
                m_target = trigger.Target.Get(this);

            base.OnInit(trigger);
        }

        public override bool Excute()
        {
            var target = Target.Get(Document);
            if (target == null) return true;

            var prop = target.GetDependencyProperty(PropertyName);
            if (prop == null) return true;

            var resolver = Engine.GetAttributeResolver(prop.PropType);
            if (resolver == null) return true;

            var newValue = resolver.Resolve(Value, prop.PropType);
            target.SetValue(prop, newValue);

            return true;
        }
    }
}