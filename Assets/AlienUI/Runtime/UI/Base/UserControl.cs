using AlienUI.Models;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract class UserControl : UIElement
    {
        protected abstract TextAsset DefaultTemplate { get; }

        public TextAsset Template
        {
            get { return (TextAsset)GetValue(TemplateProperty); }
            set { SetValue(TemplateProperty, value); }
        }
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register("Template", typeof(TextAsset), typeof(UserControl), null);

        private UIElement m_templateInstance;

        protected override Float2 CalcDesireSize()
        {
            return m_templateInstance.GetDesireSize();
        }

        protected override void OnInitialized()
        {
            var targetTemplate = Template ?? DefaultTemplate;

            var templateRoot = CreateEmptyUIGameObject("[TEMPLATE]").transform as RectTransform;
            templateRoot.SetParent(m_childRoot, false);
            templateRoot.anchorMin = new Vector2(0, 0);
            templateRoot.anchorMax = new Vector2(1, 1);
            templateRoot.pivot = new Vector2(0.5f, 0.5f);
            templateRoot.sizeDelta = Vector2.zero;
            templateRoot.anchoredPosition = Vector2.zero;
            templateRoot.SetAsFirstSibling();

            m_templateInstance = Engine.CreateUI(targetTemplate.text, templateRoot, this);
            AddChild(m_templateInstance);
        }

    }
}
