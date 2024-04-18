using AlienUI.Models;
using AlienUI.UIElements;
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

        // Using a DependencyProperty as the backing store for Template.  This enables animation, styling, binding, etc...
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

            m_templateInstance = (UIElement)Engine.CreateUI(targetTemplate.text, m_childRoot, this);
        }

    }
}
