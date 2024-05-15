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
    [ExecuteInEditMode]
    public class Engine : MonoBehaviour
    {
        internal XmlAttributeParser AttParser = new XmlAttributeParser();

        private void Awake()
        {
            if (Application.isPlaying) DontDestroyOnLoad(gameObject);
        }

        private UIElement CreateUIInternal(string xmlTxt, Transform parent, DependencyObject dataContext, AmlNodeElement templateHost, UnityEngine.Object xmlAsset)
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

                uiGameObj.hideFlags = HideFlags.DontSave;

                return uiIns;
            }
            finally
            {
                currentHandlingDoc.Pop();
            }
        }


        public UIElement CreateUI(string xmlTxt, Transform parent, DependencyObject dataContext)
        {
            return CreateUIInternal(xmlTxt, parent, dataContext, null, null);
        }

        public UIElement CreateUI(AmlAsset amlAsset, Transform parent, DependencyObject dataContext)
        {
            return CreateUIInternal(amlAsset.Text, parent, dataContext, null, amlAsset);
        }

        public UIElement CreateTemplate(AmlAsset amlAsset, Transform parent, DependencyObject dataContext, AmlNodeElement templateHost)
        {
            return CreateUIInternal(amlAsset.Text, parent, dataContext, templateHost, amlAsset);
        }

        public UIElement CreateTemplate(string amlText, Transform parent, DependencyObject dataContext, AmlNodeElement templateHost)
        {
            return CreateUIInternal(amlText, parent, dataContext, templateHost, null);
        }

        private DependencyObject Parse(string xmlTxt, Document doc)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlTxt);
            XmlNode rootNode = xmlDoc.DocumentElement;
            
            DependencyObject root = CreateNodeByXml(rootNode, null, doc);

            return root;
        }

        private AmlNodeElement CreateNodeByXml(XmlNode xnode, AmlNodeElement parentNode, Document doc)
        {
            if (xnode.NodeType == XmlNodeType.Comment) return null;

            var newNodeIns = AttParser.CreateNode(xnode);
            if (newNodeIns == null) return null;

            newNodeIns.Engine = this;
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
            if (layoutTask.Count > 0)
            {
                foreach (UIElement element in layoutTask)
                {
                    if (element.TopParent == null) element.BeginLayout();
                }
                layoutTask.Clear();
                Canvas.ForceUpdateCanvases();
            }
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
            currentHandlingDoc.TryPeek(out Document doc);
            if (doc != null && doc.m_xmlAsset != null)
                Debug.LogError(message, doc.m_xmlAsset);
            else Debug.LogError(message);
        }

#if UNITY_EDITOR
        public void ForceHanldeDirty()
        {
            LateUpdate();
        }
#endif
    }
}
