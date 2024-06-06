using AlienUI.Core.Converters;
using AlienUI.Core.Resources;
using AlienUI.Core.Triggers;
using AlienUI.PropertyResolvers;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ConverterDict = System.Collections.Generic.Dictionary<System.Type, System.Collections.Generic.Dictionary<System.Type, AlienUI.Core.Converters.ConverterBase>>;

namespace AlienUI.Core
{
    internal class XmlTypeCollector
    {
        private Dictionary<Type, PropertyResolver> m_propertyResolvers = new Dictionary<Type, PropertyResolver>();
        private Dictionary<string, Type> m_uiTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_triggerTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_resourcesTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_triggerActionTypes = new Dictionary<string, Type>();
        private ConverterDict m_converterMaps = new ConverterDict();
        private Dictionary<string, ConverterBase> m_converterNameMap = new Dictionary<string, ConverterBase>();

        public void Collect()
        {
            Type propertyResolverBaseType = typeof(PropertyResolver);
            Type uibaseType = typeof(UIElement);
            Type triggerBaseType = typeof(Trigger);
            Type resourceBaseType = typeof(Resource);
            Type triggerActionBaseType = typeof(TriggerAction);
            Type converterBaseType = typeof(ConverterBase);

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
                else if (type.IsSubclassOf(converterBaseType))
                {
                    var converterIns = Activator.CreateInstance(type) as ConverterBase;
                    var srcType = type.BaseType.GenericTypeArguments[0];
                    var dstType = type.BaseType.GenericTypeArguments[1];

                    if (!m_converterMaps.ContainsKey(srcType))
                        m_converterMaps[srcType] = new Dictionary<Type, ConverterBase>();

                    m_converterMaps[srcType][dstType] = converterIns;

                    m_converterNameMap[type.Name] = converterIns;
                }
            }
        }

        internal ConverterBase GetConverter(Type srcType, Type dstType)
        {
            Dictionary<Type, ConverterBase> temp = null;
            while (temp == null && srcType != null)
            {
                m_converterMaps.TryGetValue(srcType, out temp);
                srcType = srcType.BaseType;
            }
            if (temp == null) return null;

            if (temp.TryGetValue(dstType, out var converter))
                return converter;

            return null;
        }

        internal ConverterBase GetConverter(string converterName)
        {
            if (m_converterNameMap.TryGetValue(converterName, out var converter))
                return converter;

            return null;
        }

        internal AmlNodeElement CreateInstance(XmlNode xnode)
        {
            var type = GetDependencyObjectType(xnode);
            if (type == null) return null;

            return Activator.CreateInstance(type) as AmlNodeElement;
        }

        internal AmlNodeElement CreateInstance(string nodeName)
        {
            m_uiTypes.TryGetValue(nodeName, out var instanceType);
            if (instanceType == null) return null;
            return Activator.CreateInstance(instanceType) as AmlNodeElement;
        }

        internal Type GetDependencyObjectType(XmlNode xnode)
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
                    Engine.LogError($"not found class named: <color=blue>{fullName}</color> in node <color=white>{xnode.Name}</color>");
                }

                return instanceType;
            }
        }

        internal PropertyResolver GetAttributeResolver(Type propType)
        {
            while (propType != null)
            {
                if (m_propertyResolvers.TryGetValue(propType, out var propertyResolver)) return propertyResolver;
                propType = propType.BaseType;
            }

            return null;
        }

        internal List<Type> GetTypesOfAssignFrom(Type type)
        {
            List<Type> result = new List<Type>();
            if (type == typeof(Trigger) || type.IsSubclassOf(typeof(Trigger)))
            {

                foreach (var triggerType in m_triggerTypes.Values)
                {
                    if (type.IsAssignableFrom(triggerType))
                        result.Add(triggerType);
                }
            }
            else if (type == typeof(Resource) || type.IsSubclassOf(typeof(Resource)))
            {
                foreach (var resType in m_resourcesTypes.Values)
                {
                    if (type.IsAssignableFrom(resType))
                        result.Add(resType);
                }
            }

            return result;
        }
    }
}
