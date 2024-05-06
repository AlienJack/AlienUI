using AlienUI.Core;
using AlienUI.Core.Converters;
using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AlienUI
{
    public class Engine : MonoBehaviour
    {
        private XmlAttributeParser AttParser = new XmlAttributeParser();
        [SerializeField]
        private Settings m_setting;
        public Settings Settings => m_setting;
        [SerializeField]
        private RectTransform UIRoot;

        private void Awake()
        {
            Settings.OptimizeData();
            DontDestroyOnLoad(gameObject);
        }

        internal UIElement CreateUI(string xmlTxt, Transform parent, DependencyObject dataContext, XmlNodeElement templateHost)
        {
            Document document = new Document();
            var uiIns = Parse(xmlTxt, document) as UIElement;
            Debug.Assert(uiIns != null);
            document.SetDocumentHost(uiIns, dataContext, templateHost);

            document.PrepareStruct(AttParser);

            var uiGameObj = uiIns.Initialize();
            uiGameObj.transform.SetParent(parent, false);

            document.PrepareNotify(uiIns);

            uiIns.RaiseShow();

            SetDirty(uiIns);

            return uiIns;
        }

        public UIElement CreateUI(string xmlTxt, Transform parent, DependencyObject dataContext)
        {
            return CreateUI(xmlTxt, parent, dataContext, null);
        }

        private DependencyObject Parse(string xmlTxt, Document doc)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTxt);
            XmlNode rootNode = xmlDoc.DocumentElement;

            DependencyObject root = CreateNodeByXml(rootNode, null, doc);

            return root;
        }

        private XmlNodeElement CreateNodeByXml(XmlNode xnode, XmlNodeElement parentNode, Document doc)
        {
            if (xnode.NodeType == XmlNodeType.Comment) return null;

            var newNodeIns = AttParser.CreateNode(xnode);
            if (newNodeIns == null) return null;

            newNodeIns.Engine = this;
            newNodeIns.DataContext = doc.m_dataContext;
            newNodeIns.TemplateHost = doc.m_templateHost;
            newNodeIns.Document = doc;
            newNodeIns.Document.RecordDependencyObject(newNodeIns, xnode);
            if (parentNode != null)
                newNodeIns.Document.RecordAddChild(parentNode, newNodeIns);


            foreach (XmlNode xchild in xnode.ChildNodes)
            {
                CreateNodeByXml(xchild, newNodeIns, doc);
            }

            return newNodeIns;
        }

        public PropertyResolver GetAttributeResolver(Type propType)
        {
            return AttParser.GetAttributeResolver(propType);
        }

        public ConverterBase GetConverter(Type srcType, Type dstType)
        {
            return AttParser.Collector.GetConverter(srcType, dstType);
        }

        public ConverterBase GetConverter(string converterName)
        {
            return AttParser.Collector.GetConverter(converterName);
        }

        private HashSet<UIElement> layoutTask = new HashSet<UIElement>();

        public void SetDirty(UIElement element)
        {
            UIElement handleTarget = element.TopParent == null ? element : element.TopParent;
            layoutTask.Add(handleTarget);
        }

        private void LateUpdate()
        {
            foreach (UIElement element in layoutTask)
            {
                if (element.TopParent == null) element.BeginLayout();
            }
            layoutTask.Clear();
        }
    }
}
