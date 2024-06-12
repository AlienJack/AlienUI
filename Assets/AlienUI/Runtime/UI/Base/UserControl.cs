using AlienUI.Models;
using AlienUI.Models.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.UIElements
{
    [Description(Icon = "controller")]
    public abstract class UserControl : VisualElement
    {
        public abstract ControlTemplate DefaultTemplate { get; }

        public ControlTemplate Template
        {
            get { return (ControlTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(ControlTemplate), typeof(UserControl), new PropertyMetadata(default(ControlTemplate)), OnTemplatePropertyChanged);

        internal Template m_templateInstance;
        private UIElement m_templateRoot;

        private static void OnTemplatePropertyChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as UserControl;
            List<UIElement> childrenNeedReParent = new List<UIElement>();
            foreach (var child in self.UIChildren)
            {
                if (child.Document == self.Document)
                {
                    childrenNeedReParent.Add(child);
                }
            }

            if (self.m_templateInstance != null)
            {
                if (self.m_templateRoot != null)
                {
                    foreach (var child in self.m_templateRoot.UIChildren)
                    {
                        if (child.Document == self.Document)
                        {
                            self.m_templateRoot.RemoveChild(child);
                            childrenNeedReParent.Add(child);
                        }
                    }
                }
                self.RemoveChild(self.m_templateInstance);

                self.m_templateInstance.Close();
                self.m_templateInstance = null;
            }

            foreach (var child in childrenNeedReParent)
                self.RemoveChild(child);

            self.InstantiateTemplate();

            self.AddChild(self.m_templateInstance);
            foreach (var child in childrenNeedReParent)
                self.AddChild(child);

            self.OnTemplateApply();

            self.SetLayoutDirty();
        }

        protected virtual void OnTemplateApply() { }

        protected override Vector2 CalcDesireSize()
        {
            return m_templateInstance.GetDesireSize();
        }

        private void InstantiateTemplate()
        {
            var targetTemplate = Template.Valid ? Template : DefaultTemplate;

            m_templateInstance = targetTemplate.Instantiate(Engine, m_rectTransform, DataContext, this) as Template;
            if (m_templateInstance == null)
            {
                //try instantiate with DefaultTemplate
                Engine.LogError($"Template Resource [{Template.Name}] not found");
                m_templateInstance = DefaultTemplate.Instantiate(Engine, m_rectTransform, DataContext, this) as Template;
                if (m_templateInstance == null)
                {
                    Engine.LogError($"Template Resource [{DefaultTemplate.Name}] not found");
                    return;
                }
            }
            var templateRoot = m_templateInstance.m_rectTransform;
            templateRoot.anchorMin = new Vector2(0, 0);
            templateRoot.anchorMax = new Vector2(1, 1);
            templateRoot.pivot = new Vector2(0.5f, 0.5f);
            templateRoot.sizeDelta = Vector2.zero;
            templateRoot.anchoredPosition = Vector2.zero;
            templateRoot.SetAsFirstSibling();

            if (m_templateInstance.TemplateRoot.Get(m_templateInstance) is UIElement rootUI)
            {
                if (m_childRoot != null)
                {
                    List<Transform> unityChildren = new List<Transform>();

                    for (int i = 0; i < m_childRoot.childCount; i++)
                    {
                        unityChildren.Add(m_childRoot.GetChild(i));
                    }

                    foreach (var child in unityChildren)
                        child.SetParent(rootUI.m_childRoot, false);
                }
                m_childRoot = rootUI.m_childRoot;
                m_templateRoot = rootUI;
            }

            m_templateInstance.Horizontal = eHorizontalAlign.Stretch;
            m_templateInstance.Vertical = eVerticalAlign.Stretch;
        }

        public sealed override void AddChild(AmlNodeElement childObj)
        {
            if (childObj is UIElement && childObj != m_templateInstance)
            {
                if (m_templateRoot == null)
                    base.AddChild(childObj);
                else
                    m_templateRoot.AddChild(childObj);
            }
            else
            {
                base.AddChild(childObj);
            }
        }

        public sealed override void RemoveChild(AmlNodeElement childObj)
        {
            base.RemoveChild(childObj);
            m_templateInstance?.RemoveChild(childObj);
        }
    }
}
