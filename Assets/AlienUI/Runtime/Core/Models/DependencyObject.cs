using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI.Models
{
    public abstract class DependencyObject
    {
        private Type m_selfType;

        protected Dictionary<DependencyProperty, object> m_dpPropValues = new Dictionary<DependencyProperty, object>();

        public delegate void OnDependencyPropertyChangedHandle(DependencyProperty dp, object oldValue, object newValue);
        public event OnDependencyPropertyChangedHandle OnDependencyPropertyChanged;

        public DependencyObject()
        {
            m_selfType = GetType();

            List<DependencyProperty> allDp = new List<DependencyProperty>();
            //�����������Ĭ��ֵ
            DependencyProperty.GetAllDP(m_selfType, ref allDp);
            allDp.ForEach(dp => FillDependencyValue(dp, dp.Meta.DefaultValue));
        }

        internal void FillDependencyValue(DependencyProperty dp, object value)
        {
            m_dpPropValues[dp] = value;
        }
        internal void FillDependencyValue(string propertyName, object value)
        {
            var dp = GetDependencyProperty(propertyName);
            if (dp != null)
                FillDependencyValue(dp, value);
        }

        public DependencyProperty GetDependencyProperty(string propName)
        {
            var dp = DependencyProperty.GetDependencyPropertyByName(m_selfType, propName);
            return dp;
        }

        public List<DependencyProperty> GetAllDependencyProperties()
        {
            List<DependencyProperty> results = new List<DependencyProperty>();
            DependencyProperty.GetAllDP(m_selfType, ref results);
            return results;
        }

        public List<DependencyProperty> GetAttachedProperties()
        {
            List<DependencyProperty> result = new List<DependencyProperty>();
            foreach (var dp in m_dpPropValues.Keys)
            {
                if (dp.IsAttachedProerty) result.Add(dp);
            }

            return result;
        }

        public void SetValue(string propName, object value)
        {
            var dp = DependencyProperty.GetDependencyPropertyByName(m_selfType, propName);
            if (dp == null)
            {
                Engine.LogError($"����<color=blue>{m_selfType}</color>��û���ҵ���������<color=yellow>{propName}</color>");
                return;
            }

            SetValue(dp, value);
        }

        public void SetValue(DependencyProperty dp, object value)
        {
            if (dp == null) return;

            if (dp.Meta.IsReadOnly)
            {
                throw new Exception("Readonly Property Can not be Set");
            }

            m_dpPropValues.TryGetValue(dp, out object oldValue);

            if (oldValue == null && value == null) return;
            if (oldValue != null)
            {
                //dont use == operator cause unity's mono is suck
                if (oldValue.Equals(value)) return;
            }

            var newValue = value;
            m_dpPropValues[dp] = value;

            dp.RaiseChangeEvent(this, oldValue, newValue);

#if UNITY_EDITOR
            if (!Application.isPlaying) Canvas.ForceUpdateCanvases();
#endif
        }

        public void RaisePropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            OnDependencyPropertyChanged?.Invoke(dp, oldValue, newValue);
        }

        public object GetValue(string properyName)
        {
            if (properyName == "Self") return this;

            var dp = DependencyProperty.GetDependencyPropertyByName(m_selfType, properyName);
            if (dp == null) return null;

            return GetValue(dp);
        }

        public object GetValue(DependencyProperty dp)
        {
            if (m_dpPropValues.TryGetValue(dp, out var value)) return value;

            return dp.Meta.DefaultValue;
        }

        public T GetValue<T>(DependencyProperty dp)
        {
            return (T)GetValue(dp);
        }

        public Type GetDependencyPropertyType(string propName)
        {
            if (propName == "Self") return m_selfType;

            var dp = DependencyProperty.GetDependencyPropertyByName(m_selfType, propName);
            if (dp == null) return null;

            return dp.PropType;
        }

    }
}
