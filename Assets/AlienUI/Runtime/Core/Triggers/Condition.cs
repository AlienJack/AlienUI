using AlienUI.Models;
using System;

namespace AlienUI.Core.Resources
{
    public class Condition : Resource
    {
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(Condition), string.Empty);



        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }

        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(Condition), string.Empty);


        public EnumCompareType CompareType
        {
            get { return (EnumCompareType)GetValue(CompareTypeProperty); }
            set { SetValue(CompareTypeProperty, value); }
        }
        public static readonly DependencyProperty CompareTypeProperty =
            DependencyProperty.Register("CompareType", typeof(EnumCompareType), typeof(Condition), default(EnumCompareType));

        private object m_rawValue;
        private DependencyObject m_target;
        public void ResolveCompareValue(DependencyObject target)
        {
            m_target = target;
            var valueType = m_target.GetDependencyPropertyType(PropertyName);
            var t = Engine.GetAttributeResolver(valueType);
            m_rawValue = t?.Resolve(Value, valueType);
        }
        public bool Test()
        {
            if (m_rawValue == null) return false;
            var value = m_target.GetValue(PropertyName);

            var result = CompareType switch
            {
                EnumCompareType.Equal => object.Equals(value, m_rawValue),
                EnumCompareType.GreaterThan when value is IComparable comValue => comValue.CompareTo(m_rawValue) > 0,
                EnumCompareType.GreaterThanOrEqual when value is IComparable comValue => comValue.CompareTo(m_rawValue) >= 0,
                EnumCompareType.LessThan when value is IComparable comValue => comValue.CompareTo(m_rawValue) < 0,
                EnumCompareType.LessTranOrEqual when value is IComparable comValue => comValue.CompareTo(m_rawValue) <= 0,
                _ => false
            };

            return result;
        }
    }

    public enum EnumCompareType
    {
        Equal, GreaterThan, GreaterThanOrEqual, LessThan, LessTranOrEqual
    }
}
