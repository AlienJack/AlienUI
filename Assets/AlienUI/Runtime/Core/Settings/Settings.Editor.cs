#if UNITY_EDITOR
using AlienUI.Models;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AlienUI
{
    public partial class Settings : ScriptableObject
    {
        public static string RootPATH => "Assets/AlienUI";
        public static string SettingPath => Path.Combine(RootPATH, "Runtime/Settings.asset");

        [SettingsProvider]
        public static SettingsProvider CreateToProjectSetting()
        {
            var provider = new SettingsProvider("Project/AlienUI", SettingsScope.Project);
            provider.label = "AlienUI";
            provider.guiHandler = OnDrawSettingGUI;

            return provider;
        }


        private static ReorderableList m_templateListDrawer;
        private static ReorderableList m_uiListDrawer;

        private static void OnDrawSettingGUI(string searchContext)
        {
            var setting = prepareSettingObject();

            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(20, 20, 20, 20) });
            EditorGUI.BeginChangeCheck();

            if (GUILayout.Button("Recollect"))
            {
                setting.CollectAsset();
            }

            DrawDefaultFont(setting);
            DrawUI(setting);
            DrawTemplate(setting);

            EditorGUILayout.Space(20);

            DrawDesignerSettings(setting);

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(setting);
            EditorGUILayout.EndVertical();
        }



        private static void DrawDesignerSettings(Settings setting)
        {
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(5, 5, 5, 5) });
            EditorGUILayout.LabelField("Design Mode Settings", new GUIStyle(EditorStyles.label) { fontSize = 16 });
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(5, 5, 5, 5) });
            setting.EditPrefab = EditorGUILayout.ObjectField("EditPrefab", setting.EditPrefab, typeof(GameObject), false) as GameObject;
            setting.PreviewPrefab = EditorGUILayout.ObjectField("PreviewPrefab", setting.PreviewPrefab, typeof(GameObject), false) as GameObject;
            setting.DesignSize = EditorGUILayout.Vector2Field("DesignSize", setting.DesignSize);
            setting.DesignerLayout = EditorGUILayout.ObjectField("DesignerLayoutFile", setting.DesignerLayout, typeof(DefaultAsset), false) as DefaultAsset;
            setting.BackLayout = EditorGUILayout.ObjectField("QuitDesignerLayoutFile", setting.BackLayout, typeof(DefaultAsset), false) as DefaultAsset;
            setting.OpenAmlFileWhenOpenDesigner = EditorGUILayout.Toggle("Auto Open Aml FIle", setting.OpenAmlFileWhenOpenDesigner);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private static void DrawUI(Settings setting)
        {
            if (m_uiListDrawer == null)
            {
                m_uiListDrawer = new ReorderableList(setting.m_uiDict.Values.ToList(), typeof(AmlResouces));
                m_uiListDrawer.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "UIResources");
                m_uiListDrawer.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.BeginChangeCheck();
                    var tempItem = m_uiListDrawer.list[index] as AmlResouces;
                    tempItem.Aml = EditorGUI.ObjectField(rect, tempItem.Aml, typeof(AmlAsset), false) as AmlAsset;
                    if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(setting);
                };
            }

            m_uiListDrawer.DoLayoutList();
        }

        private static void DrawTemplate(Settings setting)
        {
            if (m_templateListDrawer == null)
            {
                m_templateListDrawer = new ReorderableList(setting.m_templatesDict.Values.ToList(), typeof(AmlResouces));
                m_templateListDrawer.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "TemplateResource");
                m_templateListDrawer.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.BeginChangeCheck();
                    var tempItem = m_templateListDrawer.list[index] as AmlResouces;
                    tempItem.Aml = EditorGUI.ObjectField(rect, tempItem.Aml, typeof(AmlAsset), false) as AmlAsset;
                    if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(setting);
                };
            }

            m_templateListDrawer.DoLayoutList();
        }

        private static void DrawDefaultFont(Settings setting)
        {
            setting.m_defaultLabelFont = (Font)EditorGUILayout.ObjectField("DefaultFont", setting.m_defaultLabelFont, typeof(Font), false);
        }

        private static Settings prepareSettingObject()
        {
            var set = AssetDatabase.LoadAssetAtPath<Settings>(SettingPath);
            if (set == null)
            {
                AssetDatabase.CreateAsset(CreateInstance<Settings>(), SettingPath);
                set = AssetDatabase.LoadAssetAtPath<Settings>(SettingPath);
            }

            return set;
        }

        public AmlAsset CreateTemplateAml(AmlAsset from = null, string defaultName = null)
        {
            var title = from != null ? $"Create The Copy From \"{from.name}\"" : "CreateTemplateAml";
            var defaultFileName = defaultName ?? $"{from.name}_Clone";
            var path = EditorUtility.SaveFilePanel(
                                    title,
                                    "Assets",
                                    defaultFileName, "aml");

            if (string.IsNullOrWhiteSpace(path)) return null;

            bool isInProject = Path.GetFullPath(path).StartsWith(Path.GetFullPath("Assets"));
            if (!isInProject)
            {
                EditorUtility.DisplayDialog("Error", "The Aml file must be created within the Unity project directory", "ok");
                return null;
            }
            var sourcePath = AssetDatabase.GetAssetPath(from);
            AssetDatabase.CopyAsset(sourcePath, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            CollectAsset();

            string newAssetPath = Path.GetRelativePath("Assets/..", path);
            return AssetDatabase.LoadAssetAtPath<AmlAsset>(newAssetPath);
        }

        public void CollectAsset()
        {
            m_amlResources.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t:amlasset", new string[] { "Assets" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var amlAsset = AssetDatabase.LoadAssetAtPath<AmlAsset>(path);
                m_amlResources.Add(new AmlResouces { Aml = amlAsset });
            }

            OptimizeData();

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);

            m_uiListDrawer = null;
            m_templateListDrawer = null;
        }
    }

    public partial class Settings : ScriptableObject
    {
        public GameObject EditPrefab;
        public GameObject PreviewPrefab;
        public Vector2 DesignSize = new Vector2(1920, 1080);
        public DefaultAsset DesignerLayout;
        public DefaultAsset BackLayout;
        public bool OpenAmlFileWhenOpenDesigner;


        internal Texture2D GetIcon(string iconName)
        {
            if (iconName == null) return null;

            var guids = AssetDatabase.FindAssets("t:texture2d", new string[] { Path.Combine(RootPATH, "Editor/Icons") });
            foreach (var id in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(id);
                if (Path.GetFileNameWithoutExtension(path).ToLower() == iconName.ToLower())
                {
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }
            return null;
        }
    }
}
#endif