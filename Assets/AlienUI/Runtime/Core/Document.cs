using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core
{
    public class Document : IDependencyObjectResolver
    {
        private HashSet<XmlNodeElement> m_dpObjects = new HashSet<XmlNodeElement>();
        private Dictionary<XmlNodeElement, XmlNode> m_dpObjectsXmlMap = new Dictionary<XmlNodeElement, XmlNode>();
        private Dictionary<string, List<XmlNodeElement>> m_dpObjectsNameMap = new Dictionary<string, List<XmlNodeElement>>();
        private Dictionary<XmlNodeElement, HashSet<XmlNodeElement>> m_parentChildrenRecords = new Dictionary<XmlNodeElement, HashSet<XmlNodeElement>>();
        private UIElement m_rootElement;
        internal DependencyObject m_dataContext;
        internal XmlNodeElement m_templateHost;
        internal Object m_xmlAsset;

        public Document(DependencyObject dataContext, XmlNodeElement templateHost, Object xmlAsset)
        {
            m_dataContext = dataContext;
            m_templateHost = templateHost;
            m_xmlAsset = xmlAsset;
        }

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

        public void StopAllCoroutine()
        {
            m_rootElement.NodeProxy.StopAllCoroutines();
        }

        public void Dispose(UIElement element)
        {
            if (element == m_rootElement)
            {
                StopAllCoroutine();
                GameObject.Destroy(m_rootElement.m_rectTransform.gameObject);
            }
            else
            {
                GameObject.Destroy(element.m_rectTransform.gameObject);
            }
        }

        internal void RecordDependencyObject(XmlNodeElement dependencyObject, XmlNode xnode)
        {
            m_dpObjects.Add(dependencyObject);
            m_dpObjectsXmlMap[dependencyObject] = xnode;

            var nameProperty = xnode.Attributes["Name"];
            if (nameProperty == null) return;
            var nameValue = nameProperty.Value;
            if (!string.IsNullOrWhiteSpace(nameValue))
            {
                if (!m_dpObjectsNameMap.ContainsKey(nameValue)) m_dpObjectsNameMap[nameValue] = new List<XmlNodeElement>();
                m_dpObjectsNameMap[nameValue].Add(dependencyObject);
            }
        }

        internal void RecordAddChild(XmlNodeElement parentNode, XmlNodeElement newNodeIns)
        {
            if (!m_parentChildrenRecords.ContainsKey(parentNode))
                m_parentChildrenRecords[parentNode] = new HashSet<XmlNodeElement>();

            m_parentChildrenRecords[parentNode].Add(newNodeIns);
        }

        public T Query<T>(string name) where T : DependencyObject
        {
            if (name == "#TemplateHost") return m_templateHost as T;
            else if (name == "#DataContext") return m_templateHost as T;

            m_dpObjectsNameMap.TryGetValue(name, out var dpObjects);
            if (dpObjects == null || dpObjects.Count == 0) return null;
            return dpObjects[0] as T;
        }

        public List<T> QueryAll<T>(string name) where T : DependencyObject
        {
            List<T> list = new List<T>();

            m_dpObjectsNameMap.TryGetValue(name, out var dpObjects);
            if (dpObjects == null || dpObjects.Count == 0) return list;
            foreach (var dpObject in dpObjects)
            {
                var targetTypeObj = dpObject as T;
                if (targetTypeObj != null) list.Add(targetTypeObj);
            }

            return list;
        }

        public DependencyObject Resolve(string resolveKey)
        {
            return Query<DependencyObject>(resolveKey);
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

        void ResolveAttributes(XmlNodeElement node, XmlNode xNode, XmlAttributeParser xmlParser)
        {
            foreach (XmlAttribute att in xNode.Attributes)
            {
                if (att.Name.StartsWith("xmlns")) continue; //xml保留字符跳过

                if (BindUtility.IsBindingString(att.Value, out Match match))
                {
                    var bindType = BindUtility.ParseBindParam(match, out string propName, out string converterName, out string modeName);
                    DependencyObject source = null;
                    switch (bindType)
                    {
                        case EnumBindingType.Binding: source = m_dataContext; break;
                        case EnumBindingType.TemplateBinding: source = m_templateHost; break;
                        default:
                            Engine.LogError("BindType Invalid");
                            break;
                    }
                    if (source != null)
                    {
                        source.BeginBind()
                        .SetSourceProperty(propName)
                        .SetTarget(node)
                        .SetTargetProperty(att.Name)
                        .Apply(converterName, modeName);
                    }
                }
                else
                {
                    xmlParser.Begin(node, xNode, att);
                    if (!xmlParser.ParseType()) continue;
                    if (!xmlParser.ParseValue()) continue;

                    node.FillDependencyValue(att.Name, xmlParser.ResultValue);
#if UNITY_EDITOR
                    //this is only for Editor Designer to Instantiate a template aml
                    if (!Application.isPlaying && m_templateHost == null && node is Template temp)
                        m_templateHost = temp.Engine.AttParser.Collector.CreateInstance(temp.Type);
#endif
                }
            }
        }


    }
}