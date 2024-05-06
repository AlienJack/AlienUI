using AlienUI.Models;
using AlienUI.UIElements;
using UnityEngine;

namespace AlienUI
{
    public readonly struct ControlTemplate
    {
        private readonly string m_name;
        public readonly bool Valid => m_name != null;

        public ControlTemplate(string name)
        {
            m_name = name;
        }


        public readonly UIElement Instantiate(Engine engine, RectTransform parent, DependencyObject dataContext, XmlNodeElement templateHost)
        {
            var templateAsset = engine.Settings.GetTemplateAsset(m_name);
            if (templateAsset == null) return null;

            var uiInstance = engine.CreateUI(templateAsset.text, parent, dataContext, templateHost);

            return uiInstance;
        }
    }
}
