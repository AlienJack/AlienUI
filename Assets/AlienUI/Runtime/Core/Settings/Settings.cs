using AlienUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI
{
    public partial class Settings : ScriptableObject
    {
        [SerializeField]
        private Font m_defaultLabelFont;
        [SerializeField]
        private List<Template> m_templates = new List<Template>();

        [NonSerialized]
        private Dictionary<string, Template> m_templatesDict = new Dictionary<string, Template>();

        public static Func<Settings> SettingGetter;
        [NonSerialized]
        bool m_optimized;
        public void OptimizeData()
        {
            m_templates.ForEach(t => m_templatesDict[t.Name] = t);
            m_optimized = true;
        }

        public Font DefaultLabelFont => m_defaultLabelFont;

        public AmlAsset GetTemplateAsset(string templateName)
        {
            m_templatesDict.TryGetValue(templateName, out Template template);
            return template.Xml;
        }

        public IEnumerable<AmlAsset> GetAllTemplateAssets()
        {
            return m_templatesDict.Values.Select(v => v.Xml);
        }

#if UNITY_EDITOR
        private static Settings m_cacheInstance;
#endif

        public static Settings Get()
        {
#if UNITY_EDITOR
            if (m_cacheInstance == null)
            {
                m_cacheInstance = UnityEditor.AssetDatabase.LoadAssetAtPath<Settings>(PATH);
            }

            if (m_cacheInstance != null && !m_cacheInstance.m_optimized) m_cacheInstance.OptimizeData();
            return m_cacheInstance;
#else
            var settingObj = SettingGetter?.Invoke();
            if(settingObj!=null && !settingObj.m_optimized) settingObj.OptimizeData();

            return settingObj;
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
