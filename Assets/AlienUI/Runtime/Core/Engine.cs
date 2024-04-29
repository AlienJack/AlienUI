using AlienUI.Core;
using AlienUI.Core.Resources;
using AlienUI.Core.Triggers;
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
    public class Engine : MonoBehaviour
    {
        private XmlAttributeParser AttParser = new XmlAttributeParser();
        [SerializeField]
        private Settings m_setting;
        public Settings Settings => m_setting;

        private void Awake()
        {
            Settings.OptimizeData();
            DontDestroyOnLoad(gameObject);
        }

        public UIElement CreateUI(string xmlTxt, Transform parent, DependencyObject dataContext)
        {
            Document document = new Document();
            var uiIns = Parse(xmlTxt, dataContext, document) as UIElement;
            Debug.Assert(uiIns != null);
            document.SetDocumentHost(uiIns);

            document.PrepareStruct(AttParser);

            var uiGameObj = uiIns.Initialize();
            uiGameObj.transform.SetParent(parent, false);

            document.PrepareNotify(uiIns);

            SetDirty(uiIns);

            return uiIns;
        }

        private DependencyObject Parse(string xmlTxt, DependencyObject dataContext, Document doc)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTxt);
            XmlNode rootNode = xmlDoc.DocumentElement;

            DependencyObject root = CreateNodeByXml(rootNode, null, dataContext, doc);

            return root;
        }

        private DependencyObject CreateNodeByXml(XmlNode xnode, DependencyObject parentNode, DependencyObject dataContext, Document doc)
        {
            var newNodeIns = AttParser.CreateNode(xnode);
            if (newNodeIns == null) return null;

            newNodeIns.Engine = this;
            newNodeIns.DataContext = dataContext;
            newNodeIns.Document = doc;
            newNodeIns.Document.RecordDependencyObject(newNodeIns, xnode);
            if (parentNode != null)
                newNodeIns.Document.RecordAddChild(parentNode, newNodeIns);


            foreach (XmlNode xchild in xnode.ChildNodes)
            {
                CreateNodeByXml(xchild, newNodeIns, dataContext, doc);
            }

            return newNodeIns;
        }

        public PropertyResolver GetAttributeResolver(Type propType)
        {
            return AttParser.GetAttributeResolver(propType);
        }

        private HashSet<UIElement> layoutTask = new HashSet<UIElement>();

        public void SetDirty(UIElement element)
        {
            UIElement handleTarget = element.TopParent == null ? element : element.TopParent as UIElement;
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
