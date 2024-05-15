using AlienUI.Models;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract class UserControl : UIElement
    {
        protected abstract ControlTemplate DefaultTemplate { get; }

        public ControlTemplate Template
        {
            get { return (ControlTemplate)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(ControlTemplate), typeof(UserControl), new PropertyMetadata(default(ControlTemplate), "Data"));

        internal Template m_templateInstance;

        protected override Vector2 CalcDesireSize()
        {
            return m_templateInstance.GetDesireSize();
        }

        protected override void OnInitialized()
        {
            var targetTemplate = Template.Valid ? Template : DefaultTemplate;

            m_templateInstance = targetTemplate.Instantiate(Engine, m_rectTransform, DataContext, this) as Template;
            var templateRoot = m_templateInstance.m_rectTransform;
            templateRoot.anchorMin = new Vector2(0, 0);
            templateRoot.anchorMax = new Vector2(1, 1);
            templateRoot.pivot = new Vector2(0.5f, 0.5f);
            templateRoot.sizeDelta = Vector2.zero;
            templateRoot.anchoredPosition = Vector2.zero;
            templateRoot.SetAsFirstSibling();

            if (m_templateInstance.TemplateRoot.Get(m_templateInstance) is UIElement rootUI)
                m_childRoot = rootUI.m_childRoot;

            m_templateInstance.Horizontal = eHorizontalAlign.Stretch;
            m_templateInstance.Vertical = eVerticalAlign.Stretch;
        }

        protected sealed override void OnPrepared()
        {
            foreach (var uichild in UIChildren)
            {
                m_templateInstance.AddChild(uichild);
            }
            UIChildren.Clear();
            AddChild(m_templateInstance);
        }
    }
}
