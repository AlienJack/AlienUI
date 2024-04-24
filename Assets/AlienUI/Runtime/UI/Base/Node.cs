using AlienUI.Models;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract class Node : DependencyObject
    {
        static DrivenRectTransformTracker tracker = new DrivenRectTransformTracker();

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(Node), null);

        private List<Node> m_childrens = new List<Node>();
        private Node m_parent = null;
        protected RectTransform m_rectTransform;
        protected RectTransform m_childRoot;

        public RectTransform Rect => m_rectTransform;
        public Engine Engine { get; set; }
        public DependencyObject DataContext { get; set; }

        protected Node Parent => m_parent;
        protected List<Node> Children => m_childrens;

        public Node TopParent
        {
            get
            {
                var parent = Parent;
                if (parent == null) return this;

                while (true)
                {
                    if (parent.Parent == null) return parent;
                    parent = parent.Parent;
                }
            }
        }

        public float ActualWidth
        {
            get => m_rectTransform.rect.width;
            set
            {
                m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
            }
        }

        public float ActualHeight
        {
            get => m_rectTransform.rect.height;
            set
            {
                m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
            }
        }

        public void SetParent(Node parentNode)
        {
            m_parent = parentNode;
            parentNode.m_childrens.Add(this);
        }

        public GameObject Initialize()
        {
            var go = CreateEmptyUIGameObject(string.IsNullOrEmpty(Name) ? GetType().Name : Name);
            m_rectTransform = go.transform as RectTransform;
            tracker.Add(go, m_rectTransform, DrivenTransformProperties.All);

            var childRoot = CreateEmptyUIGameObject("[CHILD]");
            m_childRoot = childRoot.transform as RectTransform;
            m_childRoot.SetParent(go.transform, false);
            m_childRoot.anchorMin = new Vector2(0, 0);
            m_childRoot.anchorMax = new Vector2(1, 1);
            m_childRoot.pivot = new Vector2(0.5f, 0.5f);
            m_childRoot.sizeDelta = Vector2.zero;
            m_childRoot.anchoredPosition = Vector2.zero;

            tracker.Add(childRoot, m_childRoot, DrivenTransformProperties.All);

            foreach (var child in m_childrens)
            {
                var childGo = child.Initialize();
                childGo.transform.SetParent(childRoot.transform, false);
            }
            OnInitialized();

            return go;
        }


        public override void RefreshPropertyNotify()
        {
            base.RefreshPropertyNotify();
            foreach (var child in Children) child.RefreshPropertyNotify();
        }

        protected abstract void OnInitialized();

        private static GameObject CreateEmptyUIGameObject(string name)
        {
            var go = new GameObject(name);
            var rect = go.AddComponent<RectTransform>();
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;

            return go;
        }
    }
}
