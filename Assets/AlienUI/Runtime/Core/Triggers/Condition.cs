using AlienUI.Core.Triggers;
using AlienUI.Models;
using System;

namespace AlienUI.Core.Resources
{
    public class Condition : TriggerAction
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(Condition), new PropertyMetadata(string.Empty));

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(Condition), new PropertyMetadata(string.Empty));


        public EnumCompareType CompareType
        {
            get { return (EnumCompareType)GetValue(CompareTypeProperty); }
            set { SetValue(CompareTypeProperty, value); }
        }
        public static readonly DependencyProperty CompareTypeProperty =
            DependencyProperty.Register("CompareType", typeof(EnumCompareType), typeof(Condition), new PropertyMetadata(EnumCompareType.Equal));

        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(Condition), new PropertyMetadata(default(DependencyObjectRef)));

        private object m_rawValue;
        private DependencyObject m_target;
        public override void OnInit(Trigger trigger)
        {
            m_target = Target.Get(this);

            if (m_target == null)
                m_target = trigger.Target.Get(this);

            var valueType = m_target.GetDependencyPropertyType(PropertyName);
            var t = Engine.GetAttributeResolver(valueType);
            m_rawValue = t?.Resolve(Value, valueType);

            base.OnInit(trigger);
        }

        public override bool Excute()
        {
            var value = m_target.GetValue(PropertyName);

            var result = CompareType switch
            {
                EnumCompareType.Equal => object.Equals(value, m_rawValue),
                EnumCompareType.GreaterThan when value is IComparable comValue => comValue.CompareTo(m_rawValue) > 0,
                EnumCompareType.GreaterThanOrEqual when value is IComparable comValue => comValue.CompareTo(m_rawValue) >= 0,
                EnumCompareType.LessThan when value is IComparable comValue => comValue.CompareTo(m_rawValue) < 0,
                EnumCompareType.LessTranOrEqual when value is IComparable comValue => comValue.CompareTo(m_rawValue) <= 0,
                EnumCompareType.NotEqual => !object.Equals(value, m_rawValue),
                _ => false
            };

            return result;
        }
    }

    public enum EnumCompareType
    {
        Equal, GreaterThan, GreaterThanOrEqual, LessThan, LessTranOrEqual, NotEqual
    }
}
