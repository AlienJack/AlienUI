using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using AlienUI.Models;
using AlienUI.UIElements;
using AlienUI.Core.Triggers;
using AlienUI.Core.Resources;
using System.Xml;

namespace AlienUI.Core
{
    internal class XmlTypeCollector
    {
        private Dictionary<Type, PropertyResolver> m_propertyResolvers = new Dictionary<Type, PropertyResolver>();
        private Dictionary<string, Type> m_uiTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_triggerTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_resourcesTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_triggerActionTypes = new Dictionary<string, Type>();

        public void Collect()
        {
            Type propertyResolverBaseType = typeof(PropertyResolver);
            Type uibaseType = typeof(UIElement);
            Type triggerBaseType = typeof(Trigger);
            Type resourceBaseType = typeof(Resource);
            Type triggerActionBaseType = typeof(TriggerAction);

            List<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToList();

            foreach (var type in allTypes)
            {
                if (type.IsAbstract) continue;

                if (type.IsSubclassOf(propertyResolverBaseType))
                {
                    var resolverInstance = Activator.CreateInstance(type) as PropertyResolver;
                    m_propertyResolvers[resolverInstance.GetResolveType()] = resolverInstance;
                }
                else if (type.IsSubclassOf(uibaseType))
                {
                    m_uiTypes[type.FullName] = type;
                }
                else if (type.IsSubclassOf(triggerBaseType))
                {
                    m_triggerTypes[type.Name] = type;
                }
                else if (type.IsSubclassOf(resourceBaseType))
                {
                    m_resourcesTypes[type.Name] = type;
                }
                else if (type.IsSubclassOf(triggerActionBaseType))
                {
                    m_triggerActionTypes[type.Name] = type;
                }
            }
        }

        internal DependencyObject CreateInstance(XmlNode xnode)
        {
            var type = GetDependencyObjectType(xnode);
            if (type == null) return null;

            return Activator.CreateInstance(type) as DependencyObject;
        }

        private Type GetDependencyObjectType(XmlNode xnode)
        {
            var shortName = xnode.LocalName;
            Type instanceType;
            if (m_triggerTypes.TryGetValue(shortName, out instanceType)) return instanceType;
            else if (m_resourcesTypes.TryGetValue(shortName, out instanceType)) return instanceType;
            else if (m_triggerActionTypes.TryGetValue(shortName, out instanceType)) return instanceType;
            else
            {
                var fullName = xnode.NamespaceURI + "." + xnode.LocalName;
                m_uiTypes.TryGetValue(fullName, out instanceType);

                if (instanceType == null)
                {
                    Debug.LogError($"not found class named: <color=blue>{fullName}</color> in node <color=white>{xnode.Name}</color>");
                }

                return instanceType;
            }
        }

        private Trigger createTrigger(XmlNode xnode)
        {
            m_triggerTypes.TryGetValue(xnode.LocalName, out Type triggerType);
            if (triggerType == null) return null;

            var trigger = Activator.CreateInstance(triggerType) as Trigger;
            return trigger;
        }

        internal PropertyResolver GetAttributeResolver(Type propType)
        {
            m_propertyResolvers.TryGetValue(propType, out var propertyResolver);
            return propertyResolver;
        }
    }
}
