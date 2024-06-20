using AlienUI.Core;
using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace AlienUI.Editors
{
    public class AmlGenerator : MonoBehaviour
    {
        public static string Gen(AmlNodeElement elementRoot)
        {
            XmlDocument document = new();
            Gen(document, null, elementRoot, elementRoot.Document);

            return XDocument.Parse(document.OuterXml).ToString();
        }

        private static void Gen(XmlDocument xDoc, XmlElement xparent, AmlNodeElement element, Document doc)
        {
            if (element.Document == doc)
            {
                var xmlEle = GenElement(xDoc, element);
                if (xparent == null) xDoc.AppendChild(xmlEle);
                else xparent.AppendChild(xmlEle);

                xparent = xmlEle;
            }

            foreach (AmlNodeElement child in element.Children.OrderBy(c=>c is UIElement?0:1))
            {
                Gen(xDoc, xparent, child, doc);
            }
        }

        private static XmlElement GenElement(XmlDocument doc, AmlNodeElement element)
        {
            element.GetXmlNodeName(out string prefix, out string name, out string namespaceURI);
            var xmlEle = doc.CreateElement(prefix, name, namespaceURI);
            if (element.xmlnsList != null)
            {
                foreach (var xmlns in element.xmlnsList)
                {
                    xmlEle.SetAttribute(xmlns.Item1, xmlns.Item2);
                }
            }

            var allProperties = new List<DependencyProperty>();
            allProperties.AddRange(element.GetAllDependencyProperties());
            var attachedDPs = element.GetAttachedProperties();
            allProperties.AddRange(attachedDPs);
            foreach (var dp in allProperties)
            {
                if (dp.Meta.IsAmlDisable) continue;

                var value = element.GetValue(dp);

                if (element.GetBinding(dp) is Binding binding)
                    xmlEle.SetAttribute(dp.PropName, binding.SourceCode);
                else
                {
                    if (value == null && dp.Meta.DefaultValue == null) continue;
                    if (value != null)
                    {
                        if (value.Equals(dp.Meta.DefaultValue)) continue;
                    }
                    var resolver = element.Engine.GetAttributeResolver(dp.PropType);
                    if (resolver == null) continue;

                    xmlEle.SetAttribute(dp.PropName, resolver.ToOriString(value));
                }
            }


            return xmlEle;
        }
    }
}
