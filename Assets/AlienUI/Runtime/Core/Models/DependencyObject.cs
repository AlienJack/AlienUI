using AlienUI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI.Models
{
    public abstract class DependencyObject 
    {
        protected Dictionary<DependencyProperty, object> m_dpPropValues = new Dictionary<DependencyProperty, object>();
        private Type m_selfType;

        public DependencyObject()
        {
            m_selfType = GetType();

            List<DependencyProperty> allDp = new List<DependencyProperty>();
            //�����������Ĭ��ֵ
            DependencyProperty.GetAllDP(m_selfType, ref allDp);
            allDp.ForEach(dp => SetValue(dp, dp.DefaultValue, false));
        }


        public void SetValue(string propName, object value, bool notify = true)
        {
            var dp = DependencyProperty.GetDependencyPropertyByName(m_selfType, propName);
            if (dp == null)
            {
                Debug.LogError($"����<color=blue>{m_selfType}</color>��û���ҵ���������<color=yellow>{propName}</color>");
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
