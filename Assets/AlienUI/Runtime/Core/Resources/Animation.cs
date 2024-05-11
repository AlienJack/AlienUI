using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlienUI.Core.Resources
{
    public class Animation : Resource
    {
        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(Animation), new PropertyMeta(default(DependencyObjectRef)));

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(Animation), new PropertyMeta(string.Empty));

        public float Offset
        {
            get { return (float)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public float Duration => m_duration;

        
        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(float), typeof(Animation), new PropertyMeta(0f));

        private PropertyResolver m_resolver;
        private Type m_resolverType;
        private DependencyObject m_target;
        private float m_duration;
        private List<AnimationKey> m_keys = new List<AnimationKey>();
        private object m_defaultValue;

        protected override void OnPrepared()
        {
            cacheTargetAndResolver();
            cacheKeyframes();
        }

        private void cacheKeyframes()
        {
            m_duration = Offset;
            m_keys = m_keys.OrderBy(key => key.Time).ToList();
            m_duration = m_keys[m_keys.Count - 1].Time;

            m_duration += Offset;

            foreach (var key in m_keys)
            {
                key.ActualValue = m_resolver.Resolve(key.Value, m_resolverType);
            }
        }

        private void cacheTargetAndResolver()
        {
            m_target = Target.Get(this);
            if (m_target == null) return;

            m_resolverType = m_target.GetDependencyPropertyType(PropertyName);
            if (m_resolverType == null) return;

            m_resolver = Engine.GetAttributeResolver(m_resolverType);
        }

        public void StageDefaultValue()
        {
            if (m_target != null)
                m_defaultValue = m_target.GetValue(PropertyName);
        }

        public bool Evalution(float time, out object value)
        {
            value = null;

            if (m_target == null) return false;
            if (m_resolver == null) return false;

            if (time < Offset) return false;

            time -= Offset;

            peekEdgeKeys(time, out float progress, out var left, out var right);
            object from = left != null ? left.ActualValue : m_defaultValue;
            object to = right != null ? right.ActualValue : from;

            value = m_resolver.Lerp(from, to, progress);

            return true;
        }

        private void peekEdgeKeys(float time, out float progress, out AnimationKey left, out AnimationKey right)
        {
            left = null;
            right = null;
            progress = 1f;

            foreach (var key in m_keys)
            {
                if (key.Time < time)
                {
                    if (left == null || key.Time > left.Time)
                    {
                        left = key;
                    }
                }
                else if (key.Time >= time)
                {
                    if (right == null || key.Time < right.Time)
                    {
                        right = key;
                    }
                }
            }

            // 计算进度值
            if (left != null && right != null)
            {
                progress = (time - left.Time) / (right.Time - left.Time);
            }
            else if (left == null)
            {
                progress = time / right.Time;
            }
        }

        public void ApplyValue(object value)
        {
            if (m_target == null) return;

            m_target.SetValue(PropertyName, value);
        }

        protected override void OnAddChild(XmlNodeElement childObj)
        {
            switch (childObj)
            {
                case AnimationKey keyData:
                    m_keys.Add(keyData);
                    break;
            }
        }
    }
}
