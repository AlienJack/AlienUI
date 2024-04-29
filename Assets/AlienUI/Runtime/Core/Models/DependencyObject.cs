using AlienUI.Core;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI.Models
{
    public abstract class DependencyObject : IDependencyObjectResolver
    {
        private Dictionary<DependencyProperty, object> m_dpPropValues = new Dictionary<DependencyProperty, object>();
        private Type m_selfType;
        private List<DependencyObject> m_childrens = new List<DependencyObject>();

        public Engine Engine { get; set; }
        public DependencyObject DataContext { get; set; }
        public Document Document { get; set; }
        protected List<DependencyObject> Children => m_childrens;

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

        public void AddChild(DependencyObject childObj)
        {
            m_childrens.Add(childObj);

            OnAddChild(childObj);
        }

        protected virtual void OnAddChild(DependencyObject childObj) { }

        public void RefreshPropertyNotify()
        {
            foreach (var dp in m_dpPropValues.Keys.ToArray())
            {
                var value = m_dpPropValues[dp];
                dp.RaiseChangeEvent(this, dp.DefaultValue, value);
            }
            foreach (var child in m_childrens)
                child.RefreshPropertyNotify();
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

        public DependencyObject Resolve(string resolveKey)
        {
            return this.Document.Resolve(resolveKey);
        }

        internal void Prepare()
        {
            OnPrepared();
        }

        protected virtual void OnPrepared() { }
    }
}
