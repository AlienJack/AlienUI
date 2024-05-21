using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace AlienUI
{
    public partial class Settings : ScriptableObject
    {
        [SerializeField]
        private Font m_defaultLabelFont;
        public Font DefaultLabelFont => m_defaultLabelFont;
        [SerializeField]
        private List<AmlResouces> m_amlResources = new List<AmlResouces>();

        [NonSerialized]
        private Dictionary<string, AmlResouces> m_templatesDict = new Dictionary<string, AmlResouces>();
        [NonSerialized]
        private Dictionary<Type, HashSet<AmlResouces>> m_userControl2TemplatesDict = new Dictionary<Type, HashSet<AmlResouces>>();

        [NonSerialized]
        bool m_optimized;

        public static Func<Settings> SettingGetter;


        void OptimizeData()
        {
            foreach (var item in m_amlResources)
            {
                item.CalcResourcesType();
                if (item.IsTemplateAsset)
                {
                    m_templatesDict[item.Name] = item;
                    var type = Type.GetType(item.TemplateTarget);
                    if (!m_userControl2TemplatesDict.ContainsKey(type)) m_userControl2TemplatesDict[type] = new HashSet<AmlResouces>();
                    m_userControl2TemplatesDict[type].Add(item);
                }
            }

            m_optimized = true;
        }


        public AmlAsset GetTemplateAsset(string templateName)
        {
#if UNITY_EDITOR
            var template = m_amlResources.FirstOrDefault(a => a.Name == templateName);
#else
            m_templatesDict.TryGetValue(templateName, out AmlResouces template);
#endif
            return template.Aml;
        }

        public IEnumerable<AmlAsset> GetAllTemplateAssetsByTargetType(Type targetType)
        {
            m_userControl2TemplatesDict.TryGetValue(targetType, out var value);
            return value.Select(r => r.Aml);
        }

#if UNITY_EDITOR
        private static Settings m_cacheInstance;
#endif

        public static Settings Get()
        {
#if UNITY_EDITOR
            if (m_cacheInstance == null)
            {
                m_cacheInstance = UnityEditor.AssetDatabase.LoadAssetAtPath<Settings>(SettingPath);
            }

            if (m_cacheInstance != null && !m_cacheInstance.m_optimized) m_cacheInstance.OptimizeData();
            return m_cacheInstance;
#else
            var settingObj = SettingGetter?.Invoke();
            if (settingObj != null && !settingObj.m_optimized) settingObj.OptimizeData();

            return settingObj;
#endif
        }

        [Serializable]
        public class AmlResouces
        {
            public string Name => Aml.name;
            public AmlAsset Aml;

            public string TemplateTarget { get; private set; }
            public bool IsTemplateAsset { get; private set; }

            public void CalcResourcesType()
            {
                if (Aml == null) return;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Aml.Text);
                XmlNode rootNode = xmlDoc.DocumentElement;
                if (rootNode.NamespaceURI + "." + rootNode.Name != typeof(Template).FullName) return;

                TemplateTarget = rootNode.Attributes["Type"]?.Value;
                if (TemplateTarget != null) IsTemplateAsset = true;
            }
        }
    }
}
