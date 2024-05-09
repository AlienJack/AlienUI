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

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(setting);
            EditorGUILayout.EndVertical();
        }

        private static void DrawTemplate(Settings setting)
        {
            if (m_templateList == null)
            {
                m_templateList = new ReorderableList(setting.m_templates, typeof(Template));
                m_templateList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "TemplateResouce");
                m_templateList.drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    EditorGUI.BeginChangeCheck();
                    var tempItem = m_templateList.list[index] as Template;
                    tempItem.Xml = EditorGUI.ObjectField(rect, tempItem.Xml, typeof(AmlAsset), false) as AmlAsset;
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
}
#endif