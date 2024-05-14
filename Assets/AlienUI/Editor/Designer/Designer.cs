using AlienUI.Editors.PropertyDrawer;
using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
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

        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
            SceneView.duringSceneGui += SceneView_duringSceneGui;
            DesignerTool.OnSelected += DesignerTool_OnSelected;
        }



        private void OnUpdate()
        {
            if (m_target != null && m_target.Engine != null) m_target.Engine.ForceHanldeDirty();
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnUpdate;
            SceneView.duringSceneGui -= SceneView_duringSceneGui;
            DesignerTool.OnSelected -= DesignerTool_OnSelected;
        }

        private void DesignerTool_OnSelected(UIElement obj)
        {
            m_logicTree.SelectWithoutNotify(obj);
            m_selection = obj;
            SceneView.RepaintAll();
            Repaint();
        }

        private void SceneView_duringSceneGui(SceneView obj)
        {
            if (m_selection == null) return;
            if (m_selection.NodeProxy == null) return;

            List<IGrouping<string, DependencyProperty>> groups = GetDependencyGroups();

            foreach (var group in groups)
            {
                var groupName = group.Key;

                foreach (var property in group)
                {
                    var drawer = FindDrawer(property.PropType);
                    if (drawer == null) continue;

                    if (!property.Meta.IsReadOnly)
                    {
                        var value = drawer.OnSceneGUI(m_selection, property.PropName, m_selection.GetValue(property));
                        m_selection.SetValue(property, value);
                    }
                }
            }
        }

        private void OnGUI()
        {
            var rect = new Rect();
            rect.position += new Vector2(5, 5);
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage != null)
            {
                ToolManager.SetActiveTool<DesignerTool>();

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
            SceneView.RepaintAll();
        }

        private void DrawInspector(Rect rect)
        {
            rect.width = position.width * 0.7f - 15;
            rect.height = position.height - 45;
            rect.position = new Vector2(position.width * 0.3f + 10, rect.position.y);
            GUI.Box(rect, string.Empty, EditorStyles.helpBox);
            if (m_selection == null) return;
            if (m_selection.NodeProxy == null) return;

            List<IGrouping<string, DependencyProperty>> groups = GetDependencyGroups();
            GUILayout.BeginArea(rect, new GUIStyle { padding = new RectOffset(10, 10, 10, 10) });

            EditorGUILayout.LabelField(m_selection.GetType().Name, new GUIStyle(EditorStyles.label) { fontSize = 30 }, GUILayout.Height(30));

            foreach (var group in groups)
            {
                var groupName = group.Key;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (EditorGUILayout.Foldout(true, groupName))
                {
                    foreach (var property in group)
                    {
                        EditorGUILayout.BeginHorizontal();

                        DrawDirtyMark(property);

                        var drawer = FindDrawer(property.PropType);
                        if (drawer == null)
                        {
                            var color = GUI.color;
                            GUI.color = Color.yellow;
                            EditorGUILayout.LabelField(property.PropName, $"[{property.PropType}]");
                            GUI.color = color;
                        }
                        else
                        {

                            using (new EditorGUI.DisabledGroupScope(property.Meta.IsReadOnly))
                            {
                                var value = drawer.Draw(m_selection, property.PropName, m_selection.GetValue(property));
                                if (!property.Meta.IsReadOnly)
                                    m_selection.SetValue(property, value);
                            }
                        }

                        EditorGUILayout.EndHorizontal();

                        {
                            var rightClickRect = GUILayoutUtility.GetLastRect();
                            rightClickRect.width -= 10;
                            var color = GUI.color;
                            GUI.color = new Color(0, 0, 0, 0);
                            GUI.Box(rightClickRect, string.Empty);
                            GUI.color = color;
                            HandleContextMenu(m_selection, property);
                        }
                    }
                }

                EditorGUILayout.EndVertical();
            }

            GUILayout.EndArea();
        }

        private void DrawDirtyMark(DependencyProperty property)
        {
            var currentDPValue = m_selection.GetValue(property);
            var defaultDPValue = property.Meta.DefaultValue;
            var color = GUI.color;
            ColorUtility.TryParseHtmlString(currentDPValue != defaultDPValue ? "#0f80be" : "00000000", out var dirtyColor);
            GUI.color = dirtyColor;
            GUILayout.Box(string.Empty, EditorStyles.selectionRect, GUILayout.Width(2));
            GUI.color = color;
        }

        private List<IGrouping<string, DependencyProperty>> GetDependencyGroups()
        {
            var propties = m_selection.GetAllDependencyProperties().Where(p => !p.Meta.NotAllowEdit && !p.IsAttachedProerty);
            if (m_selection.Parent != null)
            {
                var parentAttProps = m_selection.Parent.GetAllDependencyProperties().Where(p => !p.Meta.NotAllowEdit && p.IsAttachedProerty);
                propties = propties.Concat(parentAttProps);
            }
            var groups = propties.GroupBy(p => p.IsAttachedProerty ? $"AttachedProperty From {p.AttachHostType}" : p.Meta.Group).ToList();
            return groups;
        }

        private void HandleContextMenu(UIElement m_selection, DependencyProperty property)
        {
            if (Event.current.type != EventType.ContextClick) return;
            Vector2 mousePos = Event.current.mousePosition;
            if (!GUILayoutUtility.GetLastRect().Contains(mousePos)) return;

            GenericMenu menu = new GenericMenu();

            // Use MenuItem as Title
            menu.AddDisabledItem(new GUIContent($"---{m_selection}.{property.PropName}---"));

            var currentDPValue = m_selection.GetValue(property);
            var defaultDPValue = property.Meta.DefaultValue;
            if (currentDPValue != defaultDPValue)
            {
                // Add Set DefaultValue Menu
                if (property.Meta.IsReadOnly)
                    menu.AddDisabledItem(new GUIContent("set default value (ReadOnly)"));
                else
                {
                    menu.AddItem(new GUIContent("set default value"), false, () =>
                    {
                        m_selection.SetValue(property, property.Meta.DefaultValue);
                    });
                }
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
            {
                var treeRect = rect;
                treeRect.height = Mathf.Max(rect.height, m_logicTree.totalHeight);

                GUI.BeginScrollView(rect, default, treeRect);
                m_logicTree.OnGUI(treeRect);
                GUI.EndScrollView();
            }
        }
    }
}
