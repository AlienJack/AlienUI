using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core
{
    public class Document : IDependencyObjectResolver
    {
        private HashSet<DependencyObject> m_dpObjects = new HashSet<DependencyObject>();
        private Dictionary<DependencyObject, XmlNode> m_dpObjectsXmlMap = new Dictionary<DependencyObject, XmlNode>();
        private Dictionary<string, DependencyObject> m_dpObjectsNameMap = new Dictionary<string, DependencyObject>();
        public void RecordDependencyObject(DependencyObject dependencyObject, XmlNode xnode)
        {
            m_dpObjects.Add(dependencyObject);
            m_dpObjectsXmlMap[dependencyObject] = xnode;

            if (!string.IsNullOrWhiteSpace(dependencyObject.Name))
                m_dpObjectsNameMap[dependencyObject.Name] = dependencyObject;
        }

        public DependencyObject Resolve(string resolveKey)
        {
            m_dpObjectsNameMap.TryGetValue(resolveKey, out DependencyObject dpObject);
            return dpObject;
        }

        public void ResolveAttributes(XmlAttributeParser xmlParser)
        {
            foreach (var item in m_dpObjectsXmlMap)
            {
                ResolveAttributes(item.Key, item.Value, xmlParser);
            }
        }

        void ResolveAttributes(DependencyObject node, XmlNode xNode, XmlAttributeParser xmlParser)
        {
            foreach (XmlAttribute att in xNode.Attributes)
            {
                if (att.Name.StartsWith("xmlns")) continue; //xml保留字符跳过

                if (BindUtility.IsBindingString(att.Value))
                {
                    BindUtility.ParseBindParam(att.Value, out string propName, out string converterName, out string modeName);
                    node.DataContext.BeginBind()
                        .SetSourceProperty(propName)
                        .SetTarget(node)
                        .SetTargetProperty(att.Name)
                        .Apply(converterName, modeName);
                }
                else
                {
                    xmlParser.Begin(node, xNode, att);
                    if (!xmlParser.ParseType()) continue;
                    if (!xmlParser.ParseValue()) continue;

                    node.SetValue(att.Name, xmlParser.ResultValue, false);
                }

            }
        }
    }
}