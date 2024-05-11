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
        }

        private void DrawInspector(Rect rect)
        {
            rect.width = position.width * 0.7f - 15;
            rect.height = position.height - 45;
            rect.position = new Vector2(position.width * 0.3f + 10, rect.position.y);
            GUI.Box(rect, string.Empty, EditorStyles.helpBox);
            if (m_selection == null) return;
            if (m_selection.NodeProxy == null) return;

            Selection.activeObject = m_selection.NodeProxy.gameObject;
            var propties = m_selection.GetAllDependencyProperties();
            var groups = propties.GroupBy(p => p.Meta.Group).ToList();
            rect.position += new Vector2(10, 10);
            rect.height -= 10;
            rect.width -= 10;
            GUILayout.BeginArea(rect);

            foreach (var group in groups)
            {
                var groupName = group.Key;
                if (EditorGUILayout.Foldout(true, groupName))
                {
                    foreach (var property in group)
                    {
                        EditorGUILayout.BeginHorizontal(new GUIStyle { padding = new RectOffset(10, 10, 0, 0) });
                        EditorGUILayout.LabelField(property.PropName);

                        var drawer = FindDrawer(property.PropType);
                        if (drawer == null)
                        {
                            var color = GUI.color;
                            GUI.color = Color.red;
                            EditorGUILayout.LabelField($"Not find [{property.PropType}] drawer", new GUIStyle(EditorStyles.label));
                            GUI.color = color;
                        }
                        else
                        {
                            using (new EditorGUI.DisabledGroupScope(property.Meta.IsReadOnly))
                            {
                                var value = drawer.Draw(m_selection.GetValue(property));
                                if (!property.Meta.IsReadOnly)
                                    m_selection.SetValue(property, value);

                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        GUI.Box(GUILayoutUtility.GetLastRect(), string.Empty);
                        HandleContextMenu(m_selection, property);
                    }
                }
            }

            GUILayout.EndArea();
        }

        private void HandleContextMenu(UIElement m_selection, DependencyProperty property)
        {
            if (Event.current.type != EventType.ContextClick) return;
            Vector2 mousePos = Event.current.mousePosition;
            if (!GUILayoutUtility.GetLastRect().Contains(mousePos)) return;

            // 创建一个新的通用菜单
            GenericMenu menu = new GenericMenu();            

            // Use MenuItem as Title
            menu.AddDisabledItem(new GUIContent($"---{m_selection}.{property.PropName}---"));

            // Add Set DefaultValue Menu
            if (property.Meta.IsReadOnly)
                menu.AddDisabledItem(new GUIContent("Set Default Value (ReadOnly)"));
            else
            {
                menu.AddItem(new GUIContent("Set Default Value"), false, () =>
                {
                    m_selection.SetValue(property, property.Meta.DefaultValue);
                });
            }

            menu.ShowAsContext();

            Event.current.Use();
        }

        private PropertyDrawerBase FindDrawer(Type propertyType)
        {
            if (m_defaultDrawers.TryGetValue(propertyType, out var drawer))
                return drawer;
            else if (propertyType.BaseType != null)
                return FindDrawer(propertyType.BaseType);
            else
                return null;
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
