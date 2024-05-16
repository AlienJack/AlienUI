using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AlienUI.Core
{
    public class XmlAttributeParser
    {
        private DependencyObject m_node;
        private XmlAttribute m_xAtt;
        private XmlNode m_xNode;
        private Type m_pType;
        private XmlTypeCollector m_collector;
        private EnumResolver m_enumResolver = new EnumResolver();
        public object ResultValue { get; private set; }

        internal XmlTypeCollector Collector => m_collector;

        public XmlAttributeParser()
        {
            m_collector = new XmlTypeCollector();
            m_collector.Collect();
        }

        public AmlNodeElement CreateNode(XmlNode xnode)
        {
            var newIns = m_collector.CreateInstance(xnode);
#if UNITY_EDITOR
            newIns.xmlNodeName = xnode.LocalName;
#endif
            return newIns;
        }

        public void Begin(DependencyObject node, XmlNode xnode, XmlAttribute xAtt)
        {
            m_node = node;
            m_xAtt = xAtt;
            m_xNode = xnode;
        }

        public void SetPropertyType(Type propType)
        {
            m_pType = propType;
        }

        public PropertyResolver GetAttributeResolver(Type propType)
        {
            if (propType.IsEnum)
                return m_enumResolver;

            return m_collector.GetAttributeResolver(propType);
        }

        public bool ParseValue()
        {
            var propertyResolver = GetAttributeResolver(m_pType);
            if (propertyResolver == null)
            {
                Engine.LogError($"依赖属性<color=yellow>{m_xAtt.Name}</color>的类型<color=white>{m_pType}</color>没有实现Resolver");
                return false;
            }

            ResultValue = propertyResolver.Resolve(m_xAtt.Value, m_pType);
            return true;
        }
    }
}
