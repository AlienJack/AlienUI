using AlienUI.Models;
using AlienUI.PropertyResolvers;
using System;

namespace AlienUI.Core.Resources
{
    public class AnimationKey : Resource, IComparable<AnimationKey>
    {
        public float Time
        {
            get { return (float)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(float), typeof(AnimationKey), new PropertyMetadata(0f));


        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(AnimationKey), new PropertyMetadata(string.Empty), OnValueChanged);

        private static void OnValueChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as AnimationKey;
            self.m_valueDirty = true;
        }

        private object m_actualValue;
        private bool m_valueDirty;

        public object GetActualValue(PropertyResolver resolver)
        {
            if (m_actualValue == null || m_valueDirty)
            {
                m_actualValue = resolver.Resolve(Value, resolver.GetResolveType());
                m_valueDirty = false;
            }

            return m_actualValue;
        }

        public int CompareTo(AnimationKey other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}
