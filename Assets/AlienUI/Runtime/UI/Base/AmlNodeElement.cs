using AlienUI.Core;
using AlienUI.Models;
using System.Collections.Generic;
using System.Linq;

namespace AlienUI.UIElements
{
    public abstract class AmlNodeElement : DependencyObject, IDependencyObjectResolver
    {
        private List<AmlNodeElement> m_childrens = new List<AmlNodeElement>();
        protected List<AmlNodeElement> Children => m_childrens;


        public Engine Engine { get; set; }
        public DependencyObject DataContext { get; set; }
        public Document Document { get; set; }

        public DependencyObject TemplateHost
        {
            get { return (DependencyObject)GetValue(TemplateHostProperty); }
            set { SetValue(TemplateHostProperty, value); }
        }

        public static readonly DependencyProperty TemplateHostProperty =
            DependencyProperty.Register("TemplateHost", typeof(DependencyObject), typeof(AmlNodeElement), new PropertyMetadata(null).DisableEdit());



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
    }
}
