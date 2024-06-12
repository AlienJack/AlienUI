#if UNITY_EDITOR
using System.IO;
using UnityEditor;
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
    }
}
#endif