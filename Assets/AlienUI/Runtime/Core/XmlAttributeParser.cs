using AlienUI.Core.Resources;
using AlienUI.Core.Triggers;
using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace AlienUI.Core
{
    public class XmlAttributeParser
    {
        private Dictionary<Type, PropertyResolver> m_propertyResolvers = new Dictionary<Type, PropertyResolver>();
        private Dictionary<string, Type> m_NodeTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_triggerTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> m_resourcesTypes = new Dictionary<string, Type>();

        private UIElement m_node;
        private XmlAttribute m_xAtt;
        private XmlNode m_xNode;
        private Type m_pType;

        public object ResultValue { get; private set; }

        public XmlAttributeParser()
        {
            List<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToList();

            LoadPropertyResolver(allTypes);
            LoadNodeTypes(allTypes);
            LoadTriggerTypes(allTypes);
            LoadResourcesTypes(allTypes);
        }

        private void LoadResourcesTypes(List<Type> allTypes)
        {
            var resourceBaseType = typeof(Resource);

            foreach (var type in allTypes)
            {
                if (type.IsAbstract) continue;
                if (!type.IsSubclassOf(resourceBaseType)) continue;
                m_resourcesTypes[type.Name] = type;
            }
        }

        public PropertyResolver GetAttributeResolver(Type propType)
        {
            m_propertyResolvers.TryGetValue(propType, out var propertyResolver);
            return propertyResolver;
        }

        private void LoadTriggerTypes(List<Type> allTypes)
        {
            var triggerBaseType = typeof(Trigger);

            foreach (var triggerType in allTypes)
            {
                if (triggerType.IsAbstract) continue;
                if (!triggerType.IsSubclassOf(triggerBaseType)) continue;
                m_triggerTypes[triggerType.Name] = triggerType;
            }
        }

        private void LoadPropertyResolver(List<Type> allTypes)
        {
            var resolverBaseType = typeof(PropertyResolver);

            foreach (var resolverType in allTypes)
            {
                if (resolverType.IsAbstract) continue;
                if (!resolverType.IsSubclassOf(resolverBaseType)) continue;
                var resolverInstance = Activator.CreateInstance(resolverType) as PropertyResolver;
                m_propertyResolvers[resolverInstance.GetResolveType()] = resolverInstance;
            }
        }

        private void LoadNodeTypes(List<Type> allTypes)
        {
            var uiElementType = typeof(UIElement);
            foreach (var uiType in allTypes)
            {
                if (uiType.IsAbstract) continue;
                if (!uiType.IsSubclassOf(uiElementType)) continue;
                m_NodeTypes[uiType.FullName] = uiType;
            }
        }

        public object CreateNode(XmlNode xnode)
        {
            if (m_triggerTypes.ContainsKey(xnode.LocalName))
            {
                return createTrigger(xnode);
            }
            else if (m_resourcesTypes.ContainsKey(xnode.LocalName))
            {
                return createResource(xnode);
            }
            else
            {
                return createUIElement(xnode);
            }
        }

        private Trigger createTrigger(XmlNode xnode)
        {
            m_triggerTypes.TryGetValue(xnode.LocalName, out Type triggerType);
            if (triggerType == null) return null;

            var trigger = Activator.CreateInstance(triggerType) as Trigger;
            trigger.ParseByXml(xnode);
            return trigger;
        }

        private Resource createResource(XmlNode xnode)
        {
            throw new NotImplementedException();
        }

        private UIElement createUIElement(XmlNode xnode)
        {
            var fullName = xnode.NamespaceURI + "." + xnode.LocalName;
            if (!m_NodeTypes.TryGetValue(fullName, out var nodeType))
            {
                Debug.LogError($"not found class named: <color=blue>{fullName}</color> in node <color=white>{xnode.Name}</color>");
                return null;
            }

            var newNodeIns = Activator.CreateInstance(nodeType) as UIElement;
            return newNodeIns;
        }


        public void Begin(UIElement node, XmlNode xnode, XmlAttribute xAtt)
        {
            m_node = node;
            m_xAtt = xAtt;
            m_xNode = xnode;
        }

        public bool ParseType()
        {
            var pType = m_node.GetDependencyPropertyType(m_xAtt.Name);
            if (pType == null)
            {
                Debug.LogError($"no such DependencyProperty named: <color=yellow>{m_xAtt.Name}</color> in node <color=white>{m_xNode.Name}</color>");
                return false;
            }

            m_pType = pType;
            return true;
        }

        public bool ParseValue()
        {
            var propertyResolver = GetAttributeResolver(m_pType);
            if (propertyResolver == null)
            {
                Debug.LogError($"依赖属性<color=yellow>{m_xAtt.Name}</color>的类型<color=white>{m_pType}</color>没有实现Resolver");
                return false;
            }

            ResultValue = propertyResolver.Resolve(m_xAtt.Value);
            return true;
        }
    }
}
