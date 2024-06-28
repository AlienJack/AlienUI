using AlienUI.Models;
using AlienUI.Models.Attributes;
using AlienUI.PropertyResolvers;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    [AllowChild(typeof(AnimationKey))]
    public class Animation : Resource
    {
        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(Animation), new PropertyMetadata(default(DependencyObjectRef)), OnTargetChanged);

        private static void OnTargetChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Animation;
            self.m_needPrepareData = true;
        }

        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(Animation), new PropertyMetadata(string.Empty));

        public float Offset
        {
            get { return (float)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }


        public static readonly DependencyProperty OffsetProperty =
            DependencyProperty.Register("Offset", typeof(float), typeof(Animation), new PropertyMetadata(0f), OnOffsetChanged);



        public AnimationCurve Curve
        {
            get { return (AnimationCurve)GetValue(CurveProperty); }
            set { SetValue(CurveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurveProperty =
            DependencyProperty.Register("Curve", typeof(AnimationCurve), typeof(Animation), new PropertyMetadata(AnimationCurve.Linear(0, 0, 1, 1)));


        private static void OnOffsetChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Animation;
            self.m_needPrepareData = true;
        }

        internal PropertyResolver m_resolver;
        private Type m_resolverType;
        private DependencyObject m_target;

        private SortedSet<AnimationKey> m_keys = new SortedSet<AnimationKey>();
        internal object m_defaultValue;

        public float Duration => Offset + m_keys.Max.Time;


        private bool m_needPrepareData;
        internal void PrepareDatas()
        {
            if (m_needPrepareData)
            {
                cacheTargetAndResolver();

                m_needPrepareData = false;
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

        internal void StageDefaultValue()
        {
            if (m_target != null)
                m_defaultValue = m_target.GetValue(PropertyName);
        }

        public bool Evalution(float time, out object value)
        {
            PrepareDatas();

            value = null;

            if (m_target == null) return false;
            if (m_resolver == null) return false;

            if (time < Offset) return false;

            time -= Offset;

            peekEdgeKeys(time, out float progress, out var left, out var right);
            object from = left != null ? left.GetActualValue(m_resolver) : m_defaultValue;
            object to = right != null ? right.GetActualValue(m_resolver) : from;

            progress = Curve.Evaluate(Curve.length * progress);
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
                if (right.Time == 0)
                    progress = 1f;
                else
                    progress = time / right.Time;
            }
        }

        public void ApplyValue(object value)
        {
            if (m_target == null) return;

            m_target.SetValue(PropertyName, value);
        }

        protected override void OnAddChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case AnimationKey keyData:
                    m_keys.Add(keyData);
                    m_needPrepareData = true;
                    break;
            }

        }

        protected override void OnRemoveChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case AnimationKey keyData:
                    m_keys.Remove(keyData);
                    m_needPrepareData = true;
                    break;
            }
        }
    }
}
