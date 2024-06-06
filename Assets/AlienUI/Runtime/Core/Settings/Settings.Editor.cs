#if UNITY_EDITOR
using AlienUI.Editors;
using AlienUI.Models;
using AlienUI.UIElements.ToolsScript;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

        public delegate void SettingGUIDrawHandle(string searchContext);
        public static SettingGUIDrawHandle SettingGUIDraw;


        private static void OnDrawSettingGUI(string searchContext)
        {
            SettingGUIDraw?.Invoke(searchContext);
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