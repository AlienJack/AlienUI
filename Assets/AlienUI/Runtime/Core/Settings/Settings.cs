using AlienUI.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI
{
    public partial class Settings : ScriptableObject
    {
        [SerializeField]
        private Font m_defaultLabelFont;
        [SerializeField]
        private List<Template> m_templates = new List<Template>();

        private Dictionary<string, Template> m_templatesDict = new Dictionary<string, Template>();

        public static Func<Settings> SettingGetter;

        public void OptimizeData()
        {
            m_templates.ForEach(t => m_templatesDict[t.Name] = t);
        }

        public Font DefaultLabelFont => m_defaultLabelFont;

        public AmlAsset GetTemplateAsset(string templateName)
        {
            m_templatesDict.TryGetValue(templateName, out Template template);
            return template.Xml;
        }

#if UNITY_EDITOR
        private static Settings m_cacheInstance;
#endif

        public static Settings Get()
        {
            if (m_cacheInstance != null) return m_cacheInstance;
#if UNITY_EDITOR
            m_cacheInstance = UnityEditor.AssetDatabase.LoadAssetAtPath<Settings>(PATH);
            return m_cacheInstance;
#else
            return SettingGetter?.Invoke();
#endif
        }

        [Serializable]
        public class Template
        {
            public string Name => Xml.name;
            public AmlAsset Xml;
        }
    }
}
