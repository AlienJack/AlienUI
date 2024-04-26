using AlienUI.Core;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Models
{
    public abstract class DependencyObject
    {
        private Dictionary<DependencyProperty, object> m_dpPropValues = new Dictionary<DependencyProperty, object>();
        private Type m_selfType;

        public Engine Engine { get; set; }
        public DependencyObject DataContext { get; set; }
        public Document Document { get; set; }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(DependencyObject), null);

        public DependencyObject()
        {
            m_selfType = GetType();

            List<DependencyProperty> allDp = new List<DependencyProperty>();
            //填充依赖属性默认值
            DependencyProperty.GetAllDP(m_selfType, ref allDp);
            allDp.ForEach(dp => SetValue(dp, dp.DefaultValue, false));
        }

        public abstract void AddChild(DependencyObject childObj);

        public virtual void RefreshPropertyNotify()
        {
            foreach (var item in m_dpPropValues)
            {
                var dp = item.Key;
                var value = item.Value;
                dp.RaiseChangeEvent(this, dp.DefaultValue, value);
            }
        }


        public void SetValue(string propName, object value, bool notify = true)
        {
            var dp = DependencyProperty.GetDependencyPropertyByName(m_selfType, propName);
            if (dp == null)
            {
                Debug.LogError($"类型<color=blue>{m_selfType}</color>中没有找到依赖属性<color=yellow>{propName}</color>");
                return;
            }

            SetValue(dp, value, notify);
        }

        public void SetValue(DependencyProperty dp, object value, bool notify = true)
        {
            if (dp == null) return;

            m_dpPropValues.TryGetValue(dp, out object oldValue);

            var newValue = value;
            m_dpPropValues[dp] = value;

            if (notify)
                dp.RaiseChangeEvent(this, oldValue, newValue);
        }

        public object GetValue(string properyName)
        {
            var dp = DependencyProperty.GetDependencyPropertyByName(m_selfType, properyName);
            if (dp == null) return null;

            return GetValue(dp);
        }

        public object GetValue(DependencyProperty dp)
        {
            m_dpPropValues.TryGetValue(dp, out var value);
            if (value != null) return value;

            return dp.DefaultValue;
        }

        public T GetValue<T>(DependencyProperty dp)
        {
            return (T)GetValue(dp);
        }

        public Type GetDependencyPropertyType(string propName)
        {
            var dp = DependencyProperty.GetDependencyPropertyByName(m_selfType, propName);
            if (dp == null) return null;

            return dp.PropType;
        }
    }
}
