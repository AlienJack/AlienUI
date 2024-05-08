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

        internal UIElement CreateUI(string xmlTxt, Transform parent, DependencyObject dataContext, XmlNodeElement templateHost, UnityEngine.Object xmlAsset)
        {
            try
            {
                Document document = new Document(dataContext, templateHost, xmlAsset);
                currentHandlingDoc.Push(document);
                var uiIns = Parse(xmlTxt, document) as UIElement;
                Debug.Assert(uiIns != null);
                document.SetDocumentHost(uiIns);

                document.PrepareStruct(AttParser);

                var uiGameObj = uiIns.Initialize();
                uiGameObj.transform.SetParent(parent, false);

                document.PrepareNotify(uiIns);

                uiIns.RaiseShow();

                SetDirty(uiIns);

                return uiIns;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw ex;
            }
            finally
            {
                currentHandlingDoc.Pop();
            }

        }

        public UIElement CreateUI(string xmlTxt, Transform parent, DependencyObject dataContext)
        {
            return CreateUI(xmlTxt, parent, dataContext, null, null);
        }

        public UIElement CreateUI(TextAsset xmlAsset, Transform parent, DependencyObject dataContext)
        {
            return CreateUI(xmlAsset.text, parent, dataContext, null, xmlAsset);
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

        private static Stack<Document> currentHandlingDoc = new Stack<Document>();
        public static void Log(object message)
        {
            if (currentHandlingDoc.Peek() is Document doc && doc.m_xmlAsset != null)
                Debug.Log(message, doc.m_xmlAsset);
            else Debug.Log(message);
        }

        public static void LogError(object message)
        {
            if (currentHandlingDoc.Peek() is Document doc && doc.m_xmlAsset != null)
                Debug.LogError(message, doc.m_xmlAsset);
            else Debug.LogError(message);
        }
    }
}
