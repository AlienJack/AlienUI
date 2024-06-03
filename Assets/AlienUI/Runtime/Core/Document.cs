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
        private HashSet<AmlNodeElement> m_dpObjects = new HashSet<AmlNodeElement>();
        private Dictionary<AmlNodeElement, XmlNode> m_dpObjectsXmlMap = new Dictionary<AmlNodeElement, XmlNode>();
        private Dictionary<string, List<AmlNodeElement>> m_dpObjectsNameMap = new Dictionary<string, List<AmlNodeElement>>();
        private Dictionary<AmlNodeElement, HashSet<AmlNodeElement>> m_parentChildrenRecords = new Dictionary<AmlNodeElement, HashSet<AmlNodeElement>>();
        private Dictionary<AmlNodeElement, AmlNodeElement> m_childParentRecords = new Dictionary<AmlNodeElement, AmlNodeElement>();
        private UIElement m_rootElement;
        internal DependencyObject m_dataContext;
        internal AmlNodeElement m_templateHost;
        internal Object m_xmlAsset;

        public Document(DependencyObject dataContext, AmlNodeElement templateHost, Object xmlAsset)
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
            }
            if (Application.isPlaying)
                GameObject.Destroy(element.m_rectTransform.gameObject);
            else
                GameObject.DestroyImmediate(element.m_rectTransform.gameObject);
        }

        internal void RecordDependencyObject(AmlNodeElement dependencyObject, XmlNode xnode)
        {
            m_dpObjects.Add(dependencyObject);
            m_dpObjectsXmlMap[dependencyObject] = xnode;

            var nameProperty = xnode.Attributes["Name"];
            if (nameProperty == null) return;
            var nameValue = nameProperty.Value;
            if (!string.IsNullOrWhiteSpace(nameValue))
            {
                if (!m_dpObjectsNameMap.ContainsKey(nameValue)) m_dpObjectsNameMap[nameValue] = new List<AmlNodeElement>();
                m_dpObjectsNameMap[nameValue].Add(dependencyObject);
            }
        }

        internal void RecordAddChild(AmlNodeElement parentNode, AmlNodeElement newNodeIns)
        {
            if (!m_parentChildrenRecords.ContainsKey(parentNode))
                m_parentChildrenRecords[parentNode] = new HashSet<AmlNodeElement>();

            m_parentChildrenRecords[parentNode].Add(newNodeIns);
            m_childParentRecords[newNodeIns] = parentNode;
        }

        public T Query<T>(string name) where T : DependencyObject
        {
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
                m_childParentRecords.TryGetValue(item.Key, out var parent);
                ResolveAttributes(item.Key, parent, item.Value, xmlParser);
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
            uiRoot.RefreshPropertyNotify();
        }

        void ResolveAttributes(AmlNodeElement node, AmlNodeElement parent, XmlNode xNode, XmlAttributeParser xmlParser)
        {
            foreach (XmlAttribute att in xNode.Attributes)
            {
                if (att.Name.StartsWith("xmlns"))
                {
#if UNITY_EDITOR
                    if (node.xmlnsList == null) node.xmlnsList = new List<(string, string)>();
                    node.xmlnsList.Add(new(att.Name, att.Value));
#endif
                    continue;
                }

                var dp = node.GetDependencyProperty(att.Name);
                if (dp == null && parent != null)
                {
                    dp = parent.GetDependencyProperty(att.Name);
                }

                if (dp == null)
                {
                    Engine.LogError($"no such DependencyProperty named: <color=yellow>{att.Name}</color> in node <color=white>{xNode.Name}</color>");
                    if (parent != null)
                        Engine.LogError($"no such AttenchedProperty named: <color=yellow>{att.Name}</color> in node <color=white>{parent.GetType()}</color>");

                    continue;
                }

                if (dp.Meta.IsAmlDisable)
                {
                    Engine.LogError($"Not allow to set {dp.PropName} in AML file");
                    continue;
                }

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

                    source.BeginBind(att.Value)
                        .SetSourceProperty(propName)
                        .SetTarget(node)
                        .SetTargetProperty(att.Name)
                        .Apply(converterName, modeName);
                }
                else
                {
                    xmlParser.Begin(node, xNode, att);
                    xmlParser.SetPropertyType(dp.PropType);
                    if (!xmlParser.ParseValue()) continue;

                    node.FillDependencyValue(dp, xmlParser.ResultValue);
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