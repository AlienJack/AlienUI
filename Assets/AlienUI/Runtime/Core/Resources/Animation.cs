using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public class Animation : Resource
    {
        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Target.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(Animation), default(DependencyObjectRef));


        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(Animation), string.Empty);

        public string From
        {
            get { return (string)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(string), typeof(Animation), string.Empty, OnFromValueChanged);

        private static void OnFromValueChanged(DependencyObject sender, object oldValue, object newValue)
        {

        }

        public string To
        {
            get { return (string)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(string), typeof(Animation), string.Empty, OnToChanged);

        private static void OnToChanged(DependencyObject sender, object oldValue, object newValue)
        {

        }

        public float Duration
        {
            get { return (float)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(float), typeof(Animation), 1f);

        public float Offset
        {
            get { return (float)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Offset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(float), typeof(Animation), 0f);


        public bool Evalution(float time, DependencyObject target, out object value)
        {
            value = null;
            if (time < Offset) return false;

            var type = target.GetDependencyPropertyType(PropertyName);
            if (type == null) return false;

            var resolver = Engine.GetAttributeResolver(type);
            if (resolver == null) return false;

            var progress = (time - Offset) / Duration;

            value = resolver.Lerp(From, To, progress);

            return true;
        }

        public override void AddChild(DependencyObject childObj)
        {
            return;
        }
    }
}
