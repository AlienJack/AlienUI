using AlienUI.Core;
using AlienUI.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AlienUI.Editors
{
    public class AmlGenerator : MonoBehaviour
    {
        public static string Gen(AmlNodeElement elementRoot)
        {
            XmlDocument document = new XmlDocument();
            Gen(document, elementRoot, null);

            return document.OuterXml;
        }

        private static void Gen(XmlDocument doc, AmlNodeElement element, XmlElement parent)
        {
            var xmlEle = GenElement(doc, element);
            if (parent == null) doc.AppendChild(xmlEle);
            else parent.AppendChild(xmlEle);

            foreach (AmlNodeElement child in element.Children)
            {
                if (child.TemplateHost == null)
                    Gen(doc, child, xmlEle);
            }
        }

        private static XmlElement GenElement(XmlDocument doc, AmlNodeElement element)
        {
            var xmlEle = doc.CreateElement(element.xmlNodeName);
            foreach (var xmlns in element.xmlnsList)
            {
                xmlEle.SetAttribute(xmlns.Item1, xmlns.Item2);
            }

            foreach (var dp in element.GetAllDependencyProperties())
            {
                var value = element.GetValue(dp);
                if (value == dp.Meta.DefaultValue) continue;

                if (element.GetBinding(dp) is Binding binding)
                    xmlEle.SetAttribute(dp.PropName, binding.SourceCode);
                else
                {
                    var resolver = element.Engine.GetAttributeResolver(dp.PropType);
                    if (resolver == null) continue;

                    xmlEle.SetAttribute(dp.PropName, resolver.ToOriString(value));
                }

            }

            return xmlEle;
        }
    }
}
