using AlienUI.Core;
using AlienUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract class AmlNodeElement : DependencyObject, IDependencyObjectResolver
    {
        private List<AmlNodeElement> m_childrens = new List<AmlNodeElement>();

        internal List<AmlNodeElement> Children => m_childrens;

        private AmlNodeElement m_parent = null;
        internal AmlNodeElement Parent => m_parent;

        public Engine Engine { get; set; }
        public object DataContext => Document.m_dataContext;
        public Document Document { get; set; }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(AmlNodeElement), new PropertyMetadata(null));

        public event Action OnChildrenChanged;


        public virtual void AddChild(AmlNodeElement childObj)
        {
            m_childrens.Add(childObj);
            var childType = childObj.GetType();

            OnAddChild(childObj);
            childObj.SetParent(this);
            OnChildrenChanged?.Invoke();
        }

        public void MoveChild(AmlNodeElement childObj, int newIndex)
        {
            var oldIndex = m_childrens.IndexOf(childObj);
            if (oldIndex == -1) return;

            newIndex = Mathf.Clamp(newIndex, 0, m_childrens.Count);
            m_childrens.Insert(newIndex, childObj);
            if (oldIndex >= newIndex) oldIndex++;
            m_childrens.RemoveAt(oldIndex);

            OnMoveChild(childObj, newIndex);

            OnChildrenChanged?.Invoke();
        }

        public virtual void RemoveChild(AmlNodeElement childObj)
        {
            m_childrens.Remove(childObj);

            OnRemoveChild(childObj);
            childObj.SetParent(null);
            OnChildrenChanged?.Invoke();
        }

        protected virtual void OnAddChild(AmlNodeElement childObj) { }
        protected virtual void OnMoveChild(AmlNodeElement childObj, int newIndex) { }
        protected virtual void OnRemoveChild(AmlNodeElement childObj) { }

        internal IReadOnlyList<T> GetChildren<T>() where T : AmlNodeElement
        {
            List<T> result = new List<T>();
            foreach (var childObj in m_childrens)
            {
                if (childObj is T target) result.Add(target);
            }

            return result;
        }


        internal Dictionary<DependencyProperty, Binding> m_bindings = new Dictionary<DependencyProperty, Binding>();
        internal Binding GetBinding(DependencyProperty dp)
        {
            m_bindings.TryGetValue(dp, out var binding);
            return binding;
        }

        protected void SetParent(AmlNodeElement parentNode)
        {
            m_parent = parentNode;
            if (m_parent != null)
                OnParentSet(m_parent);
            else
                OnParentRemoved();
        }

        protected virtual void OnParentSet(AmlNodeElement parent) { }

        protected virtual void OnParentRemoved() { }

        private bool refreshed;
        internal void RefreshPropertyNotify()
        {
            if (!refreshed)
            {
                foreach (var dp in m_dpPropValues.Keys.ToArray())
                {
                    var value = m_dpPropValues[dp];
                    dp.RaiseChangeEvent(this, dp.Meta.DefaultValue, value);
                }

                refreshed = true;
            }

            foreach (var child in m_childrens)
                child.RefreshPropertyNotify();
        }


        public DependencyObject Resolve(string resolveKey)
        {
            return this.Document.Resolve(resolveKey);
        }

        private bool m_performed;
        public void RaiseDocumentPerformed()
        {
            if (!m_performed) OnDocumentPerformed();
            m_performed = true;

            foreach (var child in Children)
                child.RaiseDocumentPerformed();
        }
        protected virtual void OnDocumentPerformed() { }

#if UNITY_EDITOR
        public List<(string, string)> xmlnsList;

        private List<(string, string)> GetXmlsnList()
        {
            List<(string, string)> result = xmlnsList;
            AmlNodeElement host = this;
            do
            {
                result = host.xmlnsList;
                host = host.Parent;
            }
            while (result == null && host != null);

            return result;
        }

        public void GetXmlNodeName(out string prefix, out string name, out string namespaceURI)
        {
            name = GetType().FullName;
            prefix = string.Empty;
            namespaceURI = string.Empty;

            var xmlnsList = GetXmlsnList();
            if (xmlnsList == null) return;

            foreach (var item in xmlnsList)
            {
                var xmlNsName = item.Item1;
                var path = item.Item2;

                if (name.StartsWith(path))
                {
                    name = name.Replace($"{path}.", string.Empty);
                    namespaceURI = path;
                    var temp = xmlNsName.Split(':');
                    if (temp.Length > 1) prefix = temp[1];
                }
            }
        }
#endif
    }
}
