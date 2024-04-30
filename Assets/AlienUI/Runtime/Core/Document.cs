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
        private Dictionary<DependencyObject, HashSet<DependencyObject>> m_parentChildrenRecords = new Dictionary<DependencyObject, HashSet<DependencyObject>>();
        private UIElement m_rootElement;
        internal UIElement m_templateChildRoot;

        public void SetDocumentHost(UIElement root)
        {
            m_rootElement = root;
        }

        public Coroutine StartCoroutine(IEnumerator itor)
        {
            return m_rootElement.NodeProxy.StartCoroutine(itor);
        }

        public void StopCoroutine(Coroutine cor)
        {
            m_rootElement.NodeProxy.StopCoroutine(cor);
        }

        internal void RecordDependencyObject(DependencyObject dependencyObject, XmlNode xnode)
        {
            m_dpObjects.Add(dependencyObject);
            m_dpObjectsXmlMap[dependencyObject] = xnode;

            var nameProperty = xnode.Attributes["Name"];
            if (nameProperty == null) return;
            var nameValue = nameProperty.Value;
            if (!string.IsNullOrWhiteSpace(nameValue))
                m_dpObjectsNameMap[nameValue] = dependencyObject;
        }

        internal void RecordAddChild(DependencyObject parentNode, DependencyObject newNodeIns)
        {
            if (!m_parentChildrenRecords.ContainsKey(parentNode))
                m_parentChildrenRecords[parentNode] = new HashSet<DependencyObject>();

            m_parentChildrenRecords[parentNode].Add(newNodeIns);
        }

        public DependencyObject Resolve(string resolveKey)
        {
            m_dpObjectsNameMap.TryGetValue(resolveKey, out DependencyObject dpObject);
            return dpObject;
        }

        internal void PrepareStruct(XmlAttributeParser xmlParser)
        {
            foreach (var item in m_dpObjectsXmlMap)
            {
                ResolveAttributes(item.Key, item.Value, xmlParser);
            }
            foreach (var item in m_parentChildrenRecords)
            {
                var parent = item.Key;
                foreach (var child in item.Value)
                {
                    parent.AddChild(child);
                }
            }
        }

        internal void PrepareNotify(UIElement uiRoot)
        {
            foreach (var dpObj in m_dpObjects)
            {
                dpObj.Prepare();
            }

            uiRoot.RefreshPropertyNotify();
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

            if (node is UIElement uiEle && uiEle.TemplateRoot)
                m_templateChildRoot = uiEle;
        }


    }
}