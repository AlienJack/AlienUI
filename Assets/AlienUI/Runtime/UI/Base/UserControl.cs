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
            DependencyProperty.Register("Template", typeof(ControlTemplate), typeof(UserControl), default(ControlTemplate));

        internal UIElement m_templateInstance;

        protected override Float2 CalcDesireSize()
        {
            return m_templateInstance.GetDesireSize();
        }

        protected override void OnInitialized()
        {
            var targetTemplate = Template.Valid ? Template : DefaultTemplate;

            var templateRoot = AlienUtility.CreateEmptyUIGameObject("[TEMPLATE]").transform as RectTransform;
            templateRoot.SetParent(m_rectTransform, false);
            templateRoot.anchorMin = new Vector2(0, 0);
            templateRoot.anchorMax = new Vector2(1, 1);
            templateRoot.pivot = new Vector2(0.5f, 0.5f);
            templateRoot.sizeDelta = Vector2.zero;
            templateRoot.anchoredPosition = Vector2.zero;
            templateRoot.SetAsFirstSibling();

            m_templateInstance = targetTemplate.Instantiate(Engine, templateRoot, this);

            if (m_templateInstance.Document.m_templateChildRoot != null)
            {
                m_childRoot = m_templateInstance.Document.m_templateChildRoot.m_childRoot;
            }
        }
    }
}
