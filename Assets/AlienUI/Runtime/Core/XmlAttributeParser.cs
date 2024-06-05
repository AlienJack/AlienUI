using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Xml;

namespace AlienUI.Core
{
    public class XmlAttributeParser
    {
        private XmlAttribute m_xAtt;
        private Type m_pType;
        private EnumResolver m_enumResolver = new EnumResolver();
        public object ResultValue { get; private set; }

        internal XmlTypeCollector Collector => Settings.Get().m_collector;

        public AmlNodeElement CreateNode(XmlNode xnode)
        {
            var newIns = Collector.CreateInstance(xnode);
            return newIns;
        }

        public void Begin(XmlAttribute xAtt)
        {
            m_xAtt = xAtt;
        }

        public void SetPropertyType(Type propType)
        {
            m_pType = propType;
        }

        public PropertyResolver GetAttributeResolver(Type propType)
        {
            if (propType.IsEnum)
                return m_enumResolver;

            return Collector.GetAttributeResolver(propType);
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
