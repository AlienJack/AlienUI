using AlienUI.Models;
using AlienUI.UIElements;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;

namespace AlienUI
{
    public struct ControlTemplate
    {
        private string m_name;
        public bool Valid => m_name != null;

        public ControlTemplate(string name)
        {
            m_name = name;
        }


        public UIElement Instantiate(Engine engine, RectTransform parent, DependencyObject dataContext, XmlNodeElement templateHost)
        {
            var templateAsset = engine.Settings.GetTemplateAsset(m_name);
            if (templateAsset == null) return null;

            var uiInstance = engine.CreateUI(templateAsset.text, parent, dataContext, templateHost);

            return uiInstance;
        }
    }
}
