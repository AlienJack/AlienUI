using System;
using System.Collections.Generic;

namespace AlienUI.Models
{
    public partial class DependencyProperty
    {
        public string PropName { get; private set; }
        public Type PropType { get; private set; }
        public object DefaultValue { get; private set; }
        public delegate void OnValueChangedHandle(DependencyObject sender, object oldValue, object newValue);
        public event OnValueChangedHandle OnValueChanged;

        private int m_hash;

        private DependencyProperty(string propName, Type propType, object defaultValue, OnValueChangedHandle onValueChanged)
        {
            PropName = propName;
            PropType = propType;
            DefaultValue = defaultValue;
            m_hash = PropName.GetHashCode();
            OnValueChanged += onValueChanged;
        }

        public void RaiseChangeEvent(DependencyObject sender, object oldValue, object newValue)
        {
            OnValueChanged?.Invoke(sender, oldValue, newValue);
            BroadcastEvent(sender, PropName, oldValue, newValue);
        }

        public override int GetHashCode()
        {
            return m_hash;
        }

        private static Type bottomType = typeof(DependencyObject);
        private static Dictionary<Type, Dictionary<string, DependencyProperty>> m_dependencyProperties = new Dictionary<Type, Dictionary<string, DependencyProperty>>();
        public static DependencyProperty Register(string propName, Type propType, Type owenClassType, object defaultValue, OnValueChangedHandle onValueChanged = null)
        {
            if (!m_dependencyProperties.ContainsKey(owenClassType))
                m_dependencyProperties[owenClassType] = new Dictionary<string, DependencyProperty>();

            var newDP = new DependencyProperty(propName, propType, defaultValue, onValueChanged);
            m_dependencyProperties[owenClassType][newDP.PropName] = newDP;
            return newDP;
        }

        public static DependencyProperty GetDependencyPropertyByName(Type owenClassType, string propName)
        {
            m_dependencyProperties.TryGetValue(owenClassType, out var dps);
            DependencyProperty targetDp = null;
            if (dps != null) dps.TryGetValue(propName, out targetDp);

            if (targetDp == null && (owenClassType.BaseType == bottomType || owenClassType.IsSubclassOf(bottomType)))
                return GetDependencyPropertyByName(owenClassType.BaseType, propName);
            else
                return targetDp;
        }

        public static void GetAllDP(Type owenClassType, ref List<DependencyProperty> result)
        {
            if (owenClassType.IsGenericType) owenClassType = owenClassType.GetGenericTypeDefinition();
            m_dependencyProperties.TryGetValue(owenClassType, out var dps);
            if (dps != null) result.AddRange(dps.Values);

            if (owenClassType.BaseType == bottomType || owenClassType.IsSubclassOf(bottomType))
                GetAllDP(owenClassType.BaseType, ref result);
        }
    }
    public class PropertyMeta
    {
        public PropertyMeta(object defaultValue) { }
    }
}
