using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI
{
    [CreateAssetMenu(fileName = "Settings", menuName = "AlienUI/CreateSettings")]
    public class Settings : ScriptableObject
    {
        [SerializeField]
        private Font m_defaultLabelFont;
        [SerializeField]
        private List<Template> m_templates = new List<Template>();

        private Dictionary<string, Template> m_templatesDict = new Dictionary<string, Template>();

        public void OptimizeData()
        {
            m_templates.ForEach(t => m_templatesDict[t.Name] = t);
        }

        public Font DefaultLabelFont => m_defaultLabelFont;

        public TextAsset GetTemplate(string templateName)
        {
            m_templatesDict.TryGetValue(templateName, out Template template);
            return template.Xml;
        }


        [Serializable]
        public class Template
        {
            public string Name => Xml.name;
            public TextAsset Xml;
        }
    }
}
