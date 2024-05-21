using AlienUI.UIElements;
using AlienUI.UIElements.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AlienUI.Editors
{
    public class AssetInventory : EditorWindow
    {
        private static Dictionary<string, List<Asset>> s_assetsList = new Dictionary<string, List<Asset>>();
        [InitializeOnLoadMethod]
        static void LoadAssets()
        {
            var assets = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && t != typeof(Template) && t.IsSubclassOf(typeof(UIElement)))
                .Select(t => new Asset(t))
                .ToList();

            foreach (var asset in assets)
            {
                if (!s_assetsList.ContainsKey(asset.Group)) s_assetsList[asset.Group] = new List<Asset>();
                s_assetsList[asset.Group].Add(asset);
            }
        }

        internal static Dictionary<string, List<Asset>> GetAssets()
        {
            return s_assetsList;
        }

        internal static AssetSearchProvider GetSearchProvider(UIElement target)
        {
            var provider = AssetSearchProvider.CreateInstance<AssetSearchProvider>();
            provider.Target = target;
            return provider;
        }

        private void OnGUI()
        {
            var rect = position;
            DrawAsset(rect);
        }

        private static Vector2 leftScroll;
        private static Vector2 rightScroll;
        public static void DrawAsset(Rect rect)
        {
            var leftRect = new Rect(rect);
            leftRect.position = new Vector2(5, 5);
            leftRect.width = rect.width * 0.4f - 5;
            leftRect.height = rect.height - 10;

            GUI.Box(leftRect, string.Empty, EditorStyles.helpBox);
            GUILayout.BeginArea(leftRect);
            leftScroll = GUILayout.BeginScrollView(leftScroll);
            DrawAsset();
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            var rightRect = new Rect(leftRect);
            rightRect.position += new Vector2(leftRect.width + 5, 0);
            rightRect.width = rect.width * 0.6f - 10;

            GUI.Box(rightRect, string.Empty, EditorStyles.helpBox);
            GUILayout.BeginArea(rightRect);
            rightScroll = GUILayout.BeginScrollView(rightScroll);
            DrawAssetDetail(Selection);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static void DrawAssetDetail(Asset selection)
        {
            if (selection == null) return;

            if (Designer.Instance && Designer.Instance.Selection is UIElement selectUI)
            {
                var selectName = selectUI.Name ?? selectUI.GetType().Name;
                if (GUILayout.Button($"Add To {selectName}"))
                {
                    Designer.AddChild(selection.AssetType, selectUI);
                }
            }
        }



        private static Asset Selection;
        private static void DrawAsset()
        {
            EditorGUILayout.BeginVertical();
            foreach (var item in s_assetsList)
            {
                EditorGUILayout.BeginFoldoutHeaderGroup(true, item.Key);
                foreach (var asset in item.Value)
                {
                    var style = new GUIStyle(EditorStyles.miniButton);
                    GUIContent text = new GUIContent(asset.AssetType.Name, asset.AssetType.GetIcon());
                    var textRect = GUILayoutUtility.GetRect(text, style);
                    bool select = EditorGUI.Toggle(textRect, Selection == asset, EditorStyles.miniButtonMid);
                    GUI.Label(textRect, text);
                    if (select)
                    {
                        Selection = asset;
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
            EditorGUILayout.EndVertical();
        }

        public class Asset
        {
            public Type AssetType { get; private set; }
            public string Group { get; private set; }

            public Asset(Type assetType)
            {
                this.AssetType = assetType;
                if (assetType.IsSubclassOf(typeof(UserControl))) Group = "Controls";
                else if (assetType.IsSubclassOf(typeof(Container))) Group = "Container";
                else Group = "Base";
            }
        }
    }
}
