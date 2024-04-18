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

        public Node CreateUI(string xmlTxt, Transform parent, DependencyObject dataContext)
        {
            List<(Node, XmlNode)> attResolveTask = new List<(Node, XmlNode)>();

            var node = Parse(xmlTxt, dataContext, ref attResolveTask);
            var uiGameObj = node.Initialize();

            foreach (var item in attResolveTask)
            {
                ResolveAttributes(item.Item1, item.Item2);
            }
            node.RefreshPropertyNotify();

            uiGameObj.transform.SetParent(parent, false);

            (node as UIElement).BeginLayout();

            return node;
        }

        private Node Parse(string xmlTxt, DependencyObject dataContext, ref List<(Node, XmlNode)> attResolveTask)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTxt);
            XmlNode rootNode = xmlDoc.DocumentElement;

            Node root = null;
            CreateNodeByXml(rootNode, ref root, dataContext, ref attResolveTask);

            return root;
        }



        private void CreateNodeByXml(XmlNode xnode, ref Node parentNode, DependencyObject dataContext, ref List<(Node, XmlNode)> attResolveTask)
        {
            var newNodeIns = AttParser.CreateNode(xnode);
            if (newNodeIns == null) return;

            newNodeIns.Engine = this;
            newNodeIns.DataContext = dataContext;

            attResolveTask.Add(new(newNodeIns, xnode));

            if (parentNode != null) newNodeIns.SetParent(parentNode);
            else parentNode = newNodeIns;

            foreach (XmlNode xchild in xnode.ChildNodes)
            {
                CreateNodeByXml(xchild, ref newNodeIns, dataContext, ref attResolveTask);
            }
        }

        public void ResolveAttributes(Node node, XmlNode xNode)
        {
            foreach (XmlAttribute att in xNode.Attributes)
            {
                if (att.Name.StartsWith("xmlns")) continue; //xml±£Áô×Ö·ûÌø¹ý

                AttParser.Begin(node, xNode, att);
                if (!AttParser.ParseType()) continue;
                if (!AttParser.ParseValue()) continue;

                node.SetValue(att.Name, AttParser.ResultValue, false);                
            }
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
