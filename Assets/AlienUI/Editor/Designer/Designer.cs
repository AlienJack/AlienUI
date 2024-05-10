using AlienUI.Editors.PropertyDrawer;
using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AlienUI.Editors
{
    public class Designer : EditorWindow
    {
        private static Dictionary<Type, PropertyDrawerBase> m_defaultDrawers = new Dictionary<Type, PropertyDrawerBase>();
        [InitializeOnLoadMethod]
        public static void InitDrawers()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(PropertyDrawerBase)));
            foreach (var drawerType in types)
            {
                var drawer = Activator.CreateInstance(drawerType) as PropertyDrawerBase;
                m_defaultDrawers[drawer.ValueType] = drawer;
            }
        }

        private UIElement m_target;
        private AmlAsset m_amlFile;
        private LogicTree m_logicTree;
        private UIElement m_selection;

        public void SetTarget(UIElement ui, AmlAsset amlFile)
        {
            m_target = ui;
            m_amlFile = amlFile;
            m_logicTree = new LogicTree(m_target);
            m_logicTree.OnSelectItem += M_logicTree_OnSelectItem;
        }

        private void M_logicTree_OnSelectItem(UIElement obj)
        {
            m_selection = obj;
        }

        private void OnGUI()
        {
            var rect = new Rect();
            rect.position += new Vector2(5, 5);
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                rect.width = Mathf.Min(position.width, 100);
                rect.height = Mathf.Min(position.height, 30);
                if (GUI.Button(rect, "ExitEdit"))
                {
                    StageUtility.GoToMainStage();
                    if (Settings.Get().BackLayout)
                    {
                        var path = AssetDatabase.GetAssetPath(Settings.Get().BackLayout);
                        EditorUtility.LoadWindowLayout(path);
                    }
                }
                if (m_amlFile != null)
                {
                    rect.position += new Vector2(rect.width, 0);
                    if (GUI.Button(rect, "OpenAml"))
                    {
                        AmlImporter.OverrideAMLOpen = false;
                        AssetDatabase.OpenAsset(m_amlFile);
                        AmlImporter.OverrideAMLOpen = true;
                    }
                }
            }

            rect.position = new Vector2(5, rect.height + 10);

            DrawTree(rect);
            DrawInspector(rect);

            if (GUI.changed)
            {
                Canvas.ForceUpdateCanvases();
            }
        }

        private void DrawInspector(Rect rect)
        {
            rect.width = position.width * 0.7f - 15;
            rect.height = position.height - 45;
            rect.position = new Vector2(position.width * 0.3f + 10, rect.position.y);
            GUI.Box(rect, string.Empty, EditorStyles.helpBox);

            if (m_selection == null) return;

            var propties = m_selection.GetAllDependencyProperties();
            var groups = propties.GroupBy(p => p.Group).ToList();
            rect.width -= 10;
            rect.height = 30;

            foreach (var group in groups)
            {
                rect.position += new Vector2(5, 5);

                var groupName = group.Key ?? "MISC";
                if (EditorGUI.Foldout(rect, true, groupName))
                {
                    var drawerRect = new Rect(rect);
                    var totalWidth = rect.width;
                    drawerRect.position += new Vector2(5, 0);

                    foreach (var property in group)
                    {
                        drawerRect.position += new Vector2(0, 20);
                        drawerRect.width = totalWidth * 0.5f;
                        drawerRect.height = 20;

                        EditorGUI.LabelField(drawerRect, property.PropName);
                        var drawerContentRect = new Rect(drawerRect);
                        drawerContentRect.width = totalWidth * 0.5f - 10;
                        drawerContentRect.position += new Vector2(drawerContentRect.width, 0);

                        m_defaultDrawers.TryGetValue(property.PropType, out var drawer);
                        if (drawer == null)
                        {
                            var color = GUI.color;
                            GUI.color = Color.red;
                            EditorGUI.LabelField(drawerContentRect, $"Not find [{property.PropType}] drawer", new GUIStyle(EditorStyles.label));
                            GUI.color = color;
                        }
                        else
                        {
                            var value = drawer.Draw(drawerContentRect, m_selection.GetValue(property));
                            m_selection.SetValue(property, value, true);
                        }

                    }
                }

                //GUI.Box(rect, string.Empty, EditorStyles.helpBox);
            }
        }

        private void DrawTree(Rect rect)
        {
            rect.width = position.width * 0.3f;
            rect.height = position.height - 45;
            GUI.Box(rect, string.Empty, EditorStyles.helpBox);

            rect.position += new Vector2(5, 5);
            rect.width -= 10; rect.height -= 10;
            if (m_logicTree != null)
                m_logicTree.OnGUI(rect);
        }
    }
}
