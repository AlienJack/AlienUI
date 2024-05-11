#if UNITY_EDITOR
using AlienUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AlienUI
{
    public partial class Settings : ScriptableObject
    {
        public const string PATH = "Assets/AlienUI/Runtime/UI/SettingAsset/Settings.asset";

        [SettingsProvider]
        public static SettingsProvider CreateToProjectSetting()
        {
            var provider = new SettingsProvider("Project/AlienUI", SettingsScope.Project);
            provider.label = "AlienUI";
            provider.guiHandler = OnDrawSettingGUI;

            return provider;
        }


        private static ReorderableList m_templateList;

        private static void OnDrawSettingGUI(string searchContext)
        {
            var setting = prepareSettingObject();

            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(20, 20, 20, 20) });
            EditorGUI.BeginChangeCheck();

            DrawDefaultFont(setting);
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
            setting.DesignSize = EditorGUILayout.Vector2Field("DesignSize", setting.DesignSize);
            setting.DesignerLayout = EditorGUILayout.ObjectField("DesignerLayoutFile", setting.DesignerLayout, typeof(DefaultAsset), false) as DefaultAsset;
            setting.BackLayout = EditorGUILayout.ObjectField("QuitDesignerLayoutFile", setting.BackLayout, typeof(DefaultAsset), false) as DefaultAsset;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private static void DrawTemplate(Settings setting)
        {
            if (m_templateList == null)
            {
                m_templateList = new ReorderableList(setting.m_amlResources, typeof(AmlResouces));
                m_templateList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "TemplateResouce");
                m_templateList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.BeginChangeCheck();
                    var tempItem = m_templateList.list[index] as AmlResouces;
                    tempItem.Aml = EditorGUI.ObjectField(rect, tempItem.Aml, typeof(AmlAsset), false) as AmlAsset;
                    if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(setting);
                };
            }

            m_templateList.DoLayoutList();
        }

        private static void DrawDefaultFont(Settings setting)
        {
            setting.m_defaultLabelFont = (Font)EditorGUILayout.ObjectField("DefaultFont", setting.m_defaultLabelFont, typeof(Font), false);
        }

        private static Settings prepareSettingObject()
        {
            var set = AssetDatabase.LoadAssetAtPath<Settings>(PATH);
            if (set == null)
            {
                AssetDatabase.CreateAsset(CreateInstance<Settings>(), PATH);
                set = AssetDatabase.LoadAssetAtPath<Settings>(PATH);
            }

            return set;
        }
    }

    public partial class Settings : ScriptableObject
    {
        public GameObject EditPrefab;
        public Vector2 DesignSize = new Vector2(1920, 1080);
        public DefaultAsset DesignerLayout;
        public DefaultAsset BackLayout;
    }
}
#endif