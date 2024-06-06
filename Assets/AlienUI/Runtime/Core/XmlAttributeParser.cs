using AlienUI.Models;
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

        public void ParseValue(AmlNodeElement target, Type propType, string propName, string valueStr)
        {
            if (BindUtility.IsBindingString(valueStr, out Match match))
            {
                var bindType = BindUtility.ParseBindParam(match, out string srcPropName, out string converterName, out string modeName);
                object source = null;
                switch (bindType)
                {
                    case EnumBindingType.Binding: source = target.Document.m_dataContext; break;
                    case EnumBindingType.TemplateBinding: source = target.Document.m_templateHost; break;
                    default:
                        Engine.LogError("BindType Invalid");
                        break;
                }

                source.BeginBind(valueStr)
                    .SetSourceProperty(srcPropName)
                    .SetTarget(target)
                    .SetTargetProperty(propName)
                    .Apply(converterName, modeName);
            }
            else
            {
                var propertyResolver = GetAttributeResolver(propType);
                if (propertyResolver == null)
                {
                    Engine.LogError($"依赖属性<color=yellow>{propName}</color>的类型<color=white>{propType}</color>没有实现Resolver");
                    return;
                }

                ResultValue = propertyResolver.Resolve(valueStr, propType);

                target.FillDependencyValue(propName, ResultValue);
            }
        }
    }
}
