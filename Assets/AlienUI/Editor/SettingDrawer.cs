using AlienUI.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using static AlienUI.Settings;

namespace AlienUI.Editors
{
    public static class SettingDrawer
    {
        private static Settings s_settingObj;
        [InitializeOnLoadMethod]
        public static void Init()
        {
            s_settingObj = Settings.Get();
            s_settingObj.OptimizeData();
            Settings.SettingGUIDraw = OnGUI;
        }

        private static int tableSelect;
        private static string[] tabTitles = new string[] { "Designer", "AmlResources", "Unity Ref" };

        private static void OnGUI(string searchContext)
        {
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(20, 20, 20, 20) });
            EditorGUI.BeginChangeCheck();

            tableSelect = GUILayout.Toolbar(tableSelect, tabTitles);
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            switch (tableSelect)
            {
                case 0:
                    DrawDesignerSettings(s_settingObj);
                    break;
                case 1:
                    if (GUILayout.Button("Recollect"))
                    {
                        s_settingObj.CollectAsset();
                    }
                    DrawDefaultFont(s_settingObj);
                    DrawUI(s_settingObj);
                    DrawTemplate(s_settingObj);
                    break;
                case 2:
                    DrawRefMap(s_settingObj);
                    break;
            }
            EditorGUILayout.EndVertical();

            if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(s_settingObj);
            EditorGUILayout.EndVertical();
        }

        private static void DrawDesignerSettings(Settings setting)
        {
            EditorGUILayout.BeginVertical(new GUIStyle { padding = new RectOffset(5, 5, 5, 5) });
            setting.EditPrefab = EditorGUILayout.ObjectField("EditPrefab", setting.EditPrefab, typeof(GameObject), false) as GameObject;
            setting.PreviewPrefab = EditorGUILayout.ObjectField("PreviewPrefab", setting.PreviewPrefab, typeof(GameObject), false) as GameObject;
            setting.DesignSize = EditorGUILayout.Vector2Field("DesignSize", setting.DesignSize);
            setting.DesignerLayout = EditorGUILayout.ObjectField("DesignerLayoutFile", setting.DesignerLayout, typeof(DefaultAsset), false) as DefaultAsset;
            setting.BackLayout = EditorGUILayout.ObjectField("QuitDesignerLayoutFile", setting.BackLayout, typeof(DefaultAsset), false) as DefaultAsset;
            setting.OpenAmlFileWhenOpenDesigner = EditorGUILayout.Toggle("Auto Open Aml FIle", setting.OpenAmlFileWhenOpenDesigner);
            EditorGUILayout.EndVertical();
        }

        private static void DrawDefaultFont(Settings setting)
        {
            setting.m_defaultLabelFont = (Font)EditorGUILayout.ObjectField("DefaultFont", setting.m_defaultLabelFont, typeof(Font), false);
        }

        private static ReorderableList m_uiListDrawer;
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

        private static ReorderableList m_templateListDrawer;
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

        private static Vector2 refMapScrollPos;
        private static string newGroup;
        private static string newName;
        private static Type newType;
        private static UnityEngine.Object newRef;

        private static bool GetGroupOpen(string groupName)
        {
            int result = EditorPrefs.GetInt($"{nameof(SettingDrawer)}.Is{groupName}", 1);
            return result == 1;
        }

        private static void SetGroupOpen(string groupName, bool isOpen)
        {
            EditorPrefs.SetInt($"{nameof(SettingDrawer)}.Is{groupName}", isOpen ? 1 : 0);
        }

        private static void DrawRefMap(Settings setting)
        {
            refMapScrollPos = EditorGUILayout.BeginScrollView(refMapScrollPos);
            foreach (var item in setting.m_reference.m_refMap)
            {
                var open = EditorGUILayout.BeginFoldoutHeaderGroup(GetGroupOpen(item.Key), item.Key);
                SetGroupOpen(item.Key, open);
                if (open)
                {
                    foreach (var groupItem in item.Value)
                    {
                        var manifestItem = groupItem.Value;
                        EditorGUILayout.BeginHorizontal();
                        var tempColor = GUI.color;
                        GUI.color = Color.red;
                        if (GUILayout.Button("DEL", GUILayout.Width(35)))
                        {
                            manifestItem.RefObject = null;
                            continue;
                        }
                        GUI.color = tempColor;
                        manifestItem.Name = EditorGUILayout.TextField(manifestItem.Name);
                        AlienEditorUtility.GetUnityAssetsTypesNameMap().TryGetValue(manifestItem.TypeName, out Type assetType);
                        if (assetType == null) assetType = typeof(UnityEngine.Object);
                        manifestItem.RefObject = EditorGUILayout.ObjectField(string.Empty, manifestItem.RefObject, assetType, false);
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add New UnityAsset Reference"))
            {
                var searchWindow = UnityAssetTypeSearchWindow.CreateInstance<UnityAssetTypeSearchWindow>();
                searchWindow.Options = AlienEditorUtility.GetUnityAssetsTypes();
                var pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                pos.x += 130;
                searchWindow.OnSelectType = OnSelect;
                SearchWindow.Open(new SearchWindowContext(pos), searchWindow);
            }
            var temp = EditorGUIUtility.labelWidth;
            if (newType != null)
            {
                EditorGUIUtility.labelWidth = 40;
                newGroup = EditorGUILayout.TextField("Group", newGroup);
                EditorGUIUtility.labelWidth = 40;
                newName = EditorGUILayout.TextField("Name", newName);
                EditorGUIUtility.labelWidth = 60;
                newRef = EditorGUILayout.ObjectField("Reference", newRef, newType, false);
            }


            var AddParamInvalid = string.IsNullOrWhiteSpace(newGroup) || string.IsNullOrWhiteSpace(newName) || newType == null || newRef == null;

            if (!AddParamInvalid)
            {
                if (GUILayout.Button("Create"))
                {
                    setting.m_reference.AddAsset(newGroup, newName, newType, newRef);
                    newGroup = string.Empty;
                    newName = string.Empty;
                    newType = null;
                    newRef = null;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth = temp;

            if (GUI.changed)
            {
                setting.m_reference.OptimizeData();
                EditorUtility.SetDirty(setting);
            }
        }


        private static void OnSelect(Type type)
        {
            newType = type;
        }

        public static AmlAsset CreateTemplateAml(this Settings settings, AmlAsset from = null, string defaultName = null)
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

            settings.CollectAsset();

            string newAssetPath = Path.GetRelativePath("Assets/..", path);
            return AssetDatabase.LoadAssetAtPath<AmlAsset>(newAssetPath);
        }

        public static void CollectAsset(this Settings settings, string ignorePath = null)
        {
            settings.m_amlResources.Clear();
            foreach (var guid in AssetDatabase.FindAssets("t:amlasset", new string[] { "Assets" }))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path == ignorePath) continue;
                var amlAsset = AssetDatabase.LoadAssetAtPath<AmlAsset>(path);
                settings.m_amlResources.Add(new AmlResouces { Aml = amlAsset });
            }

            settings.OptimizeData();

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);

            m_uiListDrawer = null;
            m_templateListDrawer = null;
        }
    }
}