using AlienUI.Core;
using AlienUI.Models;
using System.Collections.Generic;
using System.Linq;

namespace AlienUI.UIElements
{
    public abstract class AmlNodeElement : DependencyObject, IDependencyObjectResolver
    {
        private List<AmlNodeElement> m_childrens = new List<AmlNodeElement>();
        public List<AmlNodeElement> Children => m_childrens;


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

        public virtual void AddChild(AmlNodeElement childObj)
        {
            m_childrens.Add(childObj);

            OnAddChild(childObj);
        }

        protected virtual void OnAddChild(AmlNodeElement childObj) { }

        internal Dictionary<DependencyProperty, Binding> m_bindings = new Dictionary<DependencyProperty, Binding>();
        public Binding GetBinding(DependencyProperty dp)
        {
            m_bindings.TryGetValue(dp, out var binding);
            return binding;
        }

        public void RefreshPropertyNotify()
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
