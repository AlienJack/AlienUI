using AlienUI.PropertyResolvers;
using AlienUI.UIElements;
using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace AlienUI.Core
{
    public class XmlAttributeParser
    {
        private EnumResolver m_enumResolver = new EnumResolver();
        public object ResultValue { get; private set; }

        internal XmlTypeCollector Collector => Settings.Get().m_collector;

        public AmlNodeElement CreateNode(XmlNode xnode)
        {
            var newIns = Collector.CreateInstance(xnode);
            return newIns;
        }

        public PropertyResolver GetAttributeResolver(Type propType)
        {
            if (propType.IsEnum)
                return m_enumResolver;

            return Collector.GetAttributeResolver(propType);
        }

        public void ParseValue(AmlNodeElement target, AmlNodeElement parent, Type propType, string propName, string valueStr)
        {
            if (BindUtility.IsBindingString(valueStr, out Match match))
            {
                object source = BindUtility.ParseBindParam(match, target.Document, out string srcPropName, out string converterName, out string modeName);

                source.BeginBind(valueStr)
                    .SetSourceProperty(srcPropName)
                    .SetTarget(target)
                    .SetTargetProperty(propName)
                    .Apply(converterName, modeName, parent);
            }
            else
            {
                var propertyResolver = GetAttributeResolver(propType);
                if (propertyResolver == null)
                {
                    Engine.LogError($"��������<color=yellow>{propName}</color>������<color=white>{propType}</color>û��ʵ��Resolver");
                    return;
                }

                ResultValue = propertyResolver.Resolve(valueStr, propType);

                target.FillDependencyValue(propName, ResultValue);
            }
        }
    }
}
