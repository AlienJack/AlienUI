using AlienUI.Core;
using AlienUI.Core.Resources;
using AlienUI.Core.Triggers;
using AlienUI.Models.Attributes;
using AlienUI.UIElements.ToolsScript;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.UIElements
{
    [AllowChild(typeof(Trigger), typeof(Resource))]
    public abstract partial class UIElement : AmlNodeElement
    {
        static DrivenRectTransformTracker tracker = new DrivenRectTransformTracker();

        private NodeProxy m_proxy = null;
        internal RectTransform m_rectTransform;
        internal RectTransform m_childRoot;

        internal IReadOnlyList<UIElement> UIChildren => GetChildren<UIElement>();

        internal RectTransform Rect => m_rectTransform;

        public UIElement UIParent => Parent as UIElement;

        internal UIElement TopParent
        {
            get
            {
                var parent = Parent;
                if (parent == null) return null;

                while (true)
                {
                    if (parent.Parent == null) return parent as UIElement;
                    parent = parent.Parent;
                }
            }
        }

        internal NodeProxy NodeProxy => m_proxy;

        protected override void OnAddChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case UIElement uiEle:
                    if (uiEle.Rect != null && Rect != null)
                    {
                        uiEle.Rect.SetParent(m_childRoot);
                        SetLayoutDirty();
                    }
                    break;
            }
        }

        protected override void OnRemoveChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case UIElement uiEle:
                    if (uiEle.Rect != null && uiEle.Rect.parent == m_childRoot)
                    {
                        uiEle.Rect.SetParent(null);
                        SetLayoutDirty();
                    }
                    break;
            }
        }

        protected override void OnMoveChild(AmlNodeElement childObj, int newIndex)
        {
            switch (childObj)
            {
                case UIElement uiEle:
                    if (uiEle.Rect != null && uiEle.Rect.parent == m_childRoot)
                    {
                        uiEle.Rect.SetSiblingIndex(Mathf.Max(0, newIndex - 1));
                        SetLayoutDirty();
                    }
                    break;
            }
        }

        internal GameObject Initialize()
        {
            if (m_rectTransform == null)
            {
                var go = AlienUtility.CreateEmptyUIGameObject(string.IsNullOrEmpty(Name) ? GetType().Name : Name);
                m_rectTransform = go.transform as RectTransform;
                tracker.Add(go, m_rectTransform, DrivenTransformProperties.All);

                OnInitialized();

                m_proxy = m_rectTransform.gameObject.AddComponent<NodeProxy>();
                m_proxy.TargetObject = this;
                m_proxy.hideFlags = HideFlags.HideInInspector;

                CreateChildRoot();
            }

            foreach (var child in UIChildren)
            {
                var childGo = child.Initialize();
                if (childGo != null)
                    childGo.transform.SetParent(m_childRoot, false);
            }

            return m_rectTransform.gameObject;
        }

        private void CreateChildRoot()
        {
            if (m_childRoot != null) return;

            var go = AlienUtility.CreateEmptyUIGameObject("[CHILD]");
            m_childRoot = go.transform as RectTransform;
            m_childRoot.SetParent(m_rectTransform, false);
            m_childRoot.anchorMin = new Vector2(0, 0);
            m_childRoot.anchorMax = new Vector2(1, 1);
            m_childRoot.pivot = new Vector2(0.5f, 0.5f);
            m_childRoot.sizeDelta = Vector2.zero;
            m_childRoot.anchoredPosition = Vector2.zero;
            tracker.Add(m_childRoot.gameObject, m_childRoot, DrivenTransformProperties.All);
        }

        protected abstract void OnInitialized();

        public void Close()
        {
            RaiseClose();
            Document.Dispose(this);
        }
    }
}
