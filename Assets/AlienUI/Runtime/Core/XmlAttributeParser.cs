using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace AlienUI
{
    public class XmlAttributeParser
    {
        private Dictionary<Type, PropertyResolver> m_propertyResolvers = new Dictionary<Type, PropertyResolver>();
        private Dictionary<string, Type> m_NodeTypes = new Dictionary<string, Type>();

        private Node m_node;
        private XmlAttribute m_xAtt;
        private XmlNode m_xNode;
        private Type m_pType;

        public object ResultValue { get; private set; }

        public XmlAttributeParser()
        {
            List<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToList();

            LoadPropertyResolver(allTypes);
            LoadNodeTypes(allTypes);
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

        public Node CreateNode(XmlNode xnode)
        {
            var fullName = xnode.NamespaceURI + "." + xnode.LocalName;
            if (!m_NodeTypes.TryGetValue(fullName, out var nodeType))
            {
                Debug.LogError($"not found class named: <color=blue>{fullName}</color> in node <color=white>{xnode.Name}</color>");
                return null;
            }

            var newNodeIns = Activator.CreateInstance(nodeType) as Node;
            return newNodeIns;
        }


        public void Begin(Node node, XmlNode xnode, XmlAttribute xAtt)
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
            m_propertyResolvers.TryGetValue(m_pType, out var propertyResolver);
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
