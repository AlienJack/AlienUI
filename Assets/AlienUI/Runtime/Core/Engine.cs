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
    public class Engine
    {
        private XmlAttributeParser AttParser = new XmlAttributeParser();
        public Settings Settings { get; private set; }
        public Engine(Settings setting)
        {
            Settings = setting;
            setting.OptimizeData();
        }

        public UIElement CreateUI(string xmlTxt, Transform parent, DependencyObject dataContext)
        {
            Document document = new Document();
            var uiIns = Parse(xmlTxt, dataContext, document) as UIElement;
            Debug.Assert(uiIns != null);

            var uiGameObj = uiIns.Initialize();
            document.ResolveAttributes(AttParser);
            uiIns.RefreshPropertyNotify();

            uiGameObj.transform.SetParent(parent, false);

            uiIns.BeginLayout();

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
                parentNode.AddChild(newNodeIns);

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
        public void Lateupdate()
        {
            layoutTask.Clear();

            foreach (UIElement element in layoutTask)
            {
                element.BeginLayout();
            }
        }
    }
}
