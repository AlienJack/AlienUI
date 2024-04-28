using AlienUI.Core.Resources;
using AlienUI.Core.Triggers;
using AlienUI.Events;
using AlienUI.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AlienUI.UIElements
{
    public abstract partial class UIElement : DependencyObject
    {
        static DrivenRectTransformTracker tracker = new DrivenRectTransformTracker();

        private UIElement m_parent = null;
        private NodeProxy m_proxy = null;
        protected RectTransform m_rectTransform;
        protected RectTransform m_childRoot;

        public List<UIElement> UIChildren { get; private set; } = new List<UIElement>();

        public RectTransform Rect => m_rectTransform;

        protected UIElement Parent => m_parent;

        public UIElement TopParent
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

        internal NodeProxy NodeProxy => m_proxy;

        protected override void OnAddChild(DependencyObject childObj)
        {
            switch (childObj)
            {
                case UIElement uiEle:
                    uiEle.SetParent(this);
                    UIChildren.Add(uiEle);
                    break;
                case Trigger trigger: AddTrigger(trigger); break;
            }
        }

        void SetParent(UIElement parentNode)
        {
            m_parent = parentNode;
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

            foreach (var child in UIChildren)
            {
                var childGo = child.Initialize();
                childGo.transform.SetParent(childRoot.transform, false);
            }
            OnInitialized();
            m_proxy = m_rectTransform.gameObject.AddComponent<NodeProxy>();
            m_proxy.TargetObject = this;

            return go;
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
