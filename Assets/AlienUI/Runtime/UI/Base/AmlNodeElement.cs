using AlienUI.Core;
using AlienUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlienUI.UIElements
{
    public abstract class AmlNodeElement : DependencyObject, IDependencyObjectResolver
    {
        private List<AmlNodeElement> m_childrens = new List<AmlNodeElement>();
        private Dictionary<Type, List<AmlNodeElement>> m_typedChildrens = new Dictionary<Type, List<AmlNodeElement>>();

        public List<AmlNodeElement> Children => m_childrens;

        private AmlNodeElement m_parent = null;
        internal AmlNodeElement Parent => m_parent;

        public Engine Engine { get; set; }
        public DependencyObject DataContext => Document.m_dataContext;
        public DependencyObject TemplateHost => Document.m_templateHost;
        public Document Document { get; set; }


        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(AmlNodeElement), new PropertyMetadata(null));

        public event Action OnChildrenChanged;

        internal void AddChild(AmlNodeElement childObj)
        {
            m_childrens.Add(childObj);
            var childType = childObj.GetType();

            if (!m_typedChildrens.ContainsKey(childType)) m_typedChildrens[childType] = new List<AmlNodeElement>();
            m_typedChildrens[childType].Add(childObj);

            OnAddChild(childObj);

            OnChildrenChanged?.Invoke();
        }

        internal void RemoveChild(AmlNodeElement childObj)
        {
            m_childrens.Remove(childObj);

            OnRemoveChild(childObj);

            OnChildrenChanged?.Invoke();
        }

        protected virtual void OnAddChild(AmlNodeElement childObj) { }
        protected virtual void OnRemoveChild(AmlNodeElement childObj) { }

        internal Dictionary<DependencyProperty, Binding> m_bindings = new Dictionary<DependencyProperty, Binding>();
        internal Binding GetBinding(DependencyProperty dp)
        {
            m_bindings.TryGetValue(dp, out var binding);
            return binding;
        }

        protected void SetParent(AmlNodeElement parentNode)
        {
            m_parent = parentNode;
        }
        internal void RefreshPropertyNotify()
        {
            foreach (var dp in m_dpPropValues.Keys.ToArray())
            {
                var value = m_dpPropValues[dp];
                dp.RaiseChangeEvent(this, dp.Meta.DefaultValue, value);
            }
            foreach (var child in m_childrens)
                child.RefreshPropertyNotify();
        }


        public DependencyObject Resolve(string resolveKey)
        {
            return this.Document.Resolve(resolveKey);
        }

        internal void Prepare()
        {
            OnPrepared();
        }

        protected virtual void OnPrepared() { }

#if UNITY_EDITOR
        public List<(string, string)> xmlnsList = new List<(string, string)>();
        public string xmlNodeName;
#endif
    }
}
