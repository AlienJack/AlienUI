using System;
using System.Collections.Generic;
using UnityEngine;
using OBJ_PROP_EVENTDICT = System.Collections.Generic.Dictionary<AlienUI.Models.DependencyObject, System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<AlienUI.Models.DependencyProperty.OnPropertyChangedHandle>>>;

namespace AlienUI.Models
{
    public partial class DependencyProperty
    {
        public delegate void OnPropertyChangedHandle(DependencyObject sender, object oldValue, object newValue);

        private static OBJ_PROP_EVENTDICT m_eventRouterMap = new OBJ_PROP_EVENTDICT();

        public static void Subscribe(DependencyObject listenObj, string propertyName, OnPropertyChangedHandle callback)
        {
            prepareMapDict(listenObj, propertyName);

            m_eventRouterMap[listenObj][propertyName].Add(callback);
        }

        public static void Unsubscribe(DependencyObject listenObj, string propertyName, OnPropertyChangedHandle callback)
        {
            prepareMapDict(listenObj, propertyName);

            m_eventRouterMap[listenObj][propertyName].Remove(callback);
        }

        public static void BroadcastEvent(DependencyObject sender, string propertyName, object oldValue, object newValue)
        {
            prepareMapDict(sender, propertyName);

            foreach (var callback in m_eventRouterMap[sender][propertyName])
            {
                callback.Invoke(sender, oldValue, newValue);
            }
        }

        private static void prepareMapDict(DependencyObject listenObj, string propertyName)
        {
            if (!m_eventRouterMap.ContainsKey(listenObj)) m_eventRouterMap[listenObj] = new Dictionary<string, HashSet<OnPropertyChangedHandle>>();
            if (!m_eventRouterMap[listenObj].ContainsKey(propertyName)) m_eventRouterMap[listenObj][propertyName] = new HashSet<OnPropertyChangedHandle>();
        }
    }
}
