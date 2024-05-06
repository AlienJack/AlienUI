using AlienUI.Core;
using AlienUI.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract class XmlNodeElement : DependencyObject, IDependencyObjectResolver
    {
        private List<XmlNodeElement> m_childrens = new List<XmlNodeElement>();
        protected List<XmlNodeElement> Children => m_childrens;


        public Engine Engine { get; set; }
        public DependencyObject DataContext { get; set; }
        public DependencyObject TemplateHost { get; internal set; }
        public Document Document { get; set; }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(XmlNodeElement), null);

        public virtual void AddChild(XmlNodeElement childObj)
        {
            m_childrens.Add(childObj);

            OnAddChild(childObj);
        }

        protected virtual void OnAddChild(XmlNodeElement childObj) { }



        public void RefreshPropertyNotify()
        {
            foreach (var dp in m_dpPropValues.Keys.ToArray())
            {
                var value = m_dpPropValues[dp];
                dp.RaiseChangeEvent(this, dp.DefaultValue, value);
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
    }
}
