using AlienUI.UIElements;
using AlienUI.UIElements.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        private static Rect leftView;
        public static void DrawAsset(Rect rect)
        {
            var leftRect = new Rect(rect);
            leftRect.position = new Vector2(5, 5);
            leftRect.width = rect.width - 10;
            leftRect.height = rect.height - 10;

            GUI.Box(leftRect, string.Empty, EditorStyles.helpBox);
            GUILayout.BeginArea(leftRect);
            leftScroll = GUI.BeginScrollView(new Rect(leftRect) { width = leftRect.width - 10, height = leftRect.height - 10 }, leftScroll, new Rect(leftView) { width = leftRect.width - 30 }, false, true);
            leftView = DrawAssetItems(new Rect(leftRect) { width = leftRect.width - 20, position = new Vector2(leftRect.position.x - 5, leftRect.position.y) });
            GUI.EndScrollView();
            GUILayout.EndArea();
        }


        private static Asset Selection;
        private static Rect DrawAssetItems(Rect rect)
        {
            float totalWidth = rect.width - 4;
            Vector2 position = default;
            position.x += 2;
            position.y += 2;

            float totalHeight = 2;

            foreach (var item in s_assetsList)
            {
                EditorGUI.BeginFoldoutHeaderGroup(new Rect(position, new Vector2(totalWidth, 20)), true, item.Key);
                position.y += 20;
                totalHeight += 20;

                var itemPosition = position;
                var itemSize = new Vector2(70, 70);
                var lineWidth = totalWidth;
                foreach (var asset in item.Value)
                {
                    var itemRect = new Rect(itemPosition, itemSize);

                    var evt = Event.current;
                    switch (evt.type)
                    {
                        case EventType.MouseDrag:
                            if (itemRect.Contains(evt.mousePosition))
                            {
                                DragAndDrop.PrepareStartDrag();
                                DragAndDrop.StartDrag("AssetDrag");
                                DragAndDrop.SetGenericData("AssetDrag", asset.AssetType);
                                Event.current.Use();
                            }
                            break;
                    }

                    if (GUI.Toggle(itemRect, Selection == asset, string.Empty, GUI.skin.button))
                        Selection = asset;

                    GUI.BeginGroup(itemRect);
                    var textRect = new Rect(0, 0, itemRect.width * 0.5f, itemRect.height * 0.5f);
                    textRect.x += textRect.width * 0.5f;
                    textRect.y += textRect.height * 0.5f - 8;
                    GUI.DrawTexture(textRect, asset.AssetType.GetIcon());
                    var labelRect = new Rect(4, textRect.height * 0.5f + 2, itemRect.width - 8, itemRect.height);
                    var tip = $"<b><size=12>{asset.AssetType.FullName}</size></b>";
                    if (asset.AssetType.GetDescrib() is string des && !string.IsNullOrEmpty(des))
                        tip += $"\n{des}";
                    GUI.Label(labelRect, new GUIContent(asset.AssetType.Name, tip), new GUIStyle(EditorStyles.label) { fontSize = 10, alignment = TextAnchor.MiddleCenter, clipping = TextClipping.Clip });
                    GUI.EndGroup();

                    itemPosition.x += itemSize.x;
                    lineWidth -= itemSize.x;
                    if (item.Value[^1] != asset)
                    {
                        if (lineWidth < itemSize.x)
                        {
                            lineWidth = totalWidth;
                            itemPosition.y += itemSize.y;
                            itemPosition.x = position.x;
                            position.y += itemSize.y;
                            totalHeight += itemSize.y;
                        }
                    }


                }
                position.y += itemSize.y;
                totalHeight += itemSize.y;
                EditorGUI.EndFoldoutHeaderGroup();
            }

            rect.height = Mathf.Max(rect.height, totalHeight);

            return rect;
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
