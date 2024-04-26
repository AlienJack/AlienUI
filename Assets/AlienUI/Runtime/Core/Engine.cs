using AlienUI.Core;
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
            List<(UIElement, XmlNode)> attResolveTask = new List<(UIElement, XmlNode)>();

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

        private UIElement Parse(string xmlTxt, DependencyObject dataContext, ref List<(UIElement, XmlNode)> attResolveTask)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTxt);
            XmlNode rootNode = xmlDoc.DocumentElement;

            UIElement root = null;
            CreateNodeByXml(rootNode, ref root, dataContext, ref attResolveTask);

            return root;
        }

        private void CreateNodeByXml(XmlNode xnode, ref UIElement parentNode, DependencyObject dataContext, ref List<(UIElement, XmlNode)> attResolveTask)
        {
            var newNodeIns = AttParser.CreateNode(xnode);
            if (newNodeIns == null) return;

            if (newNodeIns is Trigger triggerIns)
            {
                parentNode.AddTrigger(triggerIns);
            }
            else if (newNodeIns is UIElement uiIns)
            {
                uiIns.Engine = this;
                uiIns.DataContext = dataContext;

                attResolveTask.Add(new(uiIns, xnode));

                if (parentNode != null) uiIns.SetParent(parentNode);
                else parentNode = uiIns;

                foreach (XmlNode xchild in xnode.ChildNodes)
                {
                    CreateNodeByXml(xchild, ref uiIns, dataContext, ref attResolveTask);
                }
            }
        }

        public PropertyResolver GetAttributeResolver(Type propType)
        {
            return AttParser.GetAttributeResolver(propType);
        }

        public void ResolveAttributes(UIElement node, XmlNode xNode)
        {
            foreach (XmlAttribute att in xNode.Attributes)
            {
                if (att.Name.StartsWith("xmlns")) continue; //xml±£Áô×Ö·ûÌø¹ý

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
                    AttParser.Begin(node, xNode, att);
                    if (!AttParser.ParseType()) continue;
                    if (!AttParser.ParseValue()) continue;

                    node.SetValue(att.Name, AttParser.ResultValue, false);
                }

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
