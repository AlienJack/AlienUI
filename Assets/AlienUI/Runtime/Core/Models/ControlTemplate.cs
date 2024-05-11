using AlienUI.Models;
using AlienUI.UIElements;
using UnityEngine;

namespace AlienUI
{
    public readonly struct ControlTemplate
    {
        private readonly string m_name;
        public readonly bool Valid => m_name != null;
        public string Name => m_name;

        public ControlTemplate(string name)
        {
            m_name = name;
        }

        public readonly UIElement Instantiate(Engine engine, RectTransform parent, DependencyObject dataContext, XmlNodeElement templateHost)
        {
            var templateAsset = Settings.Get().GetTemplateAsset(m_name);
            if (templateAsset == null) return null;

            var uiInstance = engine.CreateTemplate(templateAsset, parent, dataContext, templateHost);

            return uiInstance;
        }
    }
}
