using AlienUI.Core;
using AlienUI.Models;
using AlienUI.UIElements;
using AlienUI.UIElements.ToolsScript;
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
        internal List<AmlResouces> m_amlResources = new List<AmlResouces>();
        [NonSerialized]
        internal Dictionary<string, AmlResouces> m_templatesDict = new Dictionary<string, AmlResouces>();
        [NonSerialized]
        internal Dictionary<Type, HashSet<AmlResouces>> m_userControl2TemplatesDict = new Dictionary<Type, HashSet<AmlResouces>>();
        [NonSerialized]
        internal Dictionary<string, AmlResouces> m_uiDict = new Dictionary<string, AmlResouces>();
        [NonSerialized]
        internal Dictionary<string, AmlResouces> m_windowDict = new Dictionary<string, AmlResouces>();
        [NonSerialized]
        bool m_optimized;
        [NonSerialized]
        internal XmlTypeCollector m_collector = new XmlTypeCollector();

        [SerializeField]
        internal UnityReference m_reference;

        public static Func<Settings> SettingGetter;

        public T GetUnityAsset<T>(string group, string assetName) where T : UnityEngine.Object
        {
            return m_reference.GetAsset<T>(group, assetName);
        }

        public bool GetUnityAssetPath(UnityEngine.Object obj, out string group, out string assetName)
        {
            group = string.Empty;
            assetName = string.Empty;
#if UNITY_EDITOR
            return m_reference.GetUnityAssetPath(obj, out group, out assetName);
#else
            return true;
#endif
        }

        internal void OptimizeData()
        {
            m_templatesDict.Clear();
            m_userControl2TemplatesDict.Clear();
            m_uiDict.Clear();
            m_collector.Collect();

            for (int i = 0; i < m_amlResources.Count; i++)
            {
                var item = m_amlResources[i];
                if (item.Aml == null)
                {
                    m_amlResources.RemoveAt(i);
                    i--;
                    continue;
                }
                item.CalcResourcesType(m_collector);

                if (item.IsTemplateAsset)
                {
                    m_templatesDict[item.Name] = item;
                    var type = Type.GetType(item.TemplateTarget);
                    if (type == null) continue;
                    if (!m_userControl2TemplatesDict.ContainsKey(type)) m_userControl2TemplatesDict[type] = new HashSet<AmlResouces>();
                    m_userControl2TemplatesDict[type].Add(item);
                }
                else
                {
                    if (typeof(Window).IsAssignableFrom(item.AssetType))
                    {
                        m_windowDict[item.Name] = item;
                    }
                    else
                    {
                        m_uiDict[item.Name] = item;
                    }
                }
            }

            m_reference.OptimizeData();
            m_optimized = true;
        }

        public AmlAsset GetTemplateAsset(string templateName)
        {
#if UNITY_EDITOR
            var template = m_amlResources.FirstOrDefault(a => a.Name == templateName);
#else
            m_templatesDict.TryGetValue(templateName, out AmlResouces template);
#endif
            if (template == null) return null;

            return template.Aml;
        }

        public IEnumerable<AmlAsset> GetAllTemplateAssetsByTargetType(Type targetType)
        {
            m_userControl2TemplatesDict.TryGetValue(targetType, out var value);
            if (value != null)
                return value.Select(r => r.Aml);
            else return Enumerable.Empty<AmlAsset>();
        }

        public IEnumerable<AmlAsset> GetUIAssets()
        {
            return m_uiDict.Values.Select(t => t.Aml);
        }

        public AmlResouces GetWindowAsset(string windowName)
        {
            m_windowDict.TryGetValue(windowName, out var value);
            return value;
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
            public bool IsTemplateAsset => AssetType == typeof(Template);

            public Type AssetType { get; private set; }

            internal void CalcResourcesType(XmlTypeCollector collector)
            {
                TemplateTarget = null;
                AssetType = null;
                if (Aml == null) return;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Aml.Text);
                XmlNode rootNode = xmlDoc.DocumentElement;
                AssetType = collector.GetDependencyObjectType(rootNode);

                TemplateTarget = rootNode.Attributes["Type"]?.Value;
            }
        }
    }
}
