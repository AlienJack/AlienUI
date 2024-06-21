using AlienUI.Core.Triggers;
using AlienUI.Models;
using AlienUI.PropertyResolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Core.Resources
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

        public bool Smooth
        {
            get { return (bool)GetValue(SmoothProperty); }
            set { SetValue(SmoothProperty, value); }
        }

        public static readonly DependencyProperty SmoothProperty =
            DependencyProperty.Register("Smooth", typeof(bool), typeof(Setter), new PropertyMetadata(false));

        public float SmoothTime
        {
            get { return (float)GetValue(SmoothTimeProperty); }
            set { SetValue(SmoothTimeProperty, value); }
        }
        public static readonly DependencyProperty SmoothTimeProperty =
            DependencyProperty.Register("SmoothTime", typeof(float), typeof(Setter), new PropertyMetadata(0.2f));

        private DependencyObject m_target;
        public override void OnInit(Trigger trigger)
        {
            m_target = Target.Get(this);

            if (m_target == null)
                m_target = trigger.Target.Get(this);

            base.OnInit(trigger);
        }

        Coroutine m_smoothingCor;
        public override bool Excute()
        {
            var target = Target.Get(Document);
            if (target == null) return true;

            var prop = target.GetDependencyProperty(PropertyName);
            if (prop == null) return true;

            var resolver = Engine.GetAttributeResolver(prop.PropType);
            if (resolver == null) return true;

            var newValue = resolver.Resolve(Value, prop.PropType);

            if (Smooth)
            {
                m_smoothingCor = Document.StartCoroutine(SmoothSet(resolver, prop, target, newValue));
            }
            else target.SetValue(prop, newValue);

            return true;
        }

        private void StopSmoothing()
        {
            if (m_smoothingCor != null)
            {
                Document.StopCoroutine(m_smoothingCor);
                m_smoothingCor = null;
            }
        }


        private static Dictionary<SmoothingID, Setter> s_smoothingSetters = new Dictionary<SmoothingID, Setter>();
        private IEnumerator SmoothSet(PropertyResolver resolver, DependencyProperty prop, DependencyObject target, object endValue)
        {
            var smoothingID = new SmoothingID { Property = prop, Target = target };
            s_smoothingSetters.TryGetValue(smoothingID, out var setter);
            if (setter != null)
                setter.StopSmoothing();

            s_smoothingSetters[smoothingID] = this;

            var oldValue = target.GetValue(prop);
            var targetValue = endValue;

            float currentTime = 0;
            while (currentTime <= SmoothTime)
            {
                var value = resolver.Lerp(oldValue, targetValue, currentTime / SmoothTime);
                target.SetValue(prop, value);

                currentTime += Time.deltaTime;
                yield return null;
            }

            if (m_smoothingCor != null)
            {
                s_smoothingSetters.Remove(smoothingID);
            }
        }



        private struct SmoothingID
        {
            public DependencyObject Target;
            public DependencyProperty Property;

            public override int GetHashCode()
            {
                return HashCode.Combine(Target, Property);
            }
        }
    }
}