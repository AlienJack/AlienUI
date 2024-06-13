using AlienUI.Core;
using AlienUI.Editors.PropertyDrawer;
using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine;
using EG = UnityEditor.EditorGUI;
using EGL = UnityEditor.EditorGUILayout;
using G = UnityEngine.GUI;
using GL = UnityEngine.GUILayout;

namespace AlienUI.Editors
{
    public class Designer : EditorWindow
    {
        private static Dictionary<Type, PropertyDrawerBase> m_defaultDrawers = new Dictionary<Type, PropertyDrawerBase>();
        private static Dictionary<Type, ElementEditor> m_elementEditors = new Dictionary<Type, ElementEditor>();
        [InitializeOnLoadMethod]
        public static void InitDrawers()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => !t.IsAbstract);
            foreach (var drawerType in types)
            {
                if (drawerType.IsSubclassOf(typeof(PropertyDrawerBase)))
                {
                    var drawer = Activator.CreateInstance(drawerType) as PropertyDrawerBase;
                    m_defaultDrawers[drawer.ValueType] = drawer;
                }
                else if (drawerType.IsSubclassOf(typeof(ElementEditor)))
                {
                    var editor = Activator.CreateInstance(drawerType) as ElementEditor;
                    m_elementEditors[editor.AdapterType] = editor;
                }
            }
        }

        public static Designer Instance { get; private set; }

        private void OnEnable()
        {
            EditorApplication.update += OnUpdate;
            SceneView.duringSceneGui += SceneView_duringSceneGui;
            DesignerTool.OnSelected += DesignerTool_OnSelected;
            AmlAsset.OnAssetReimported += AmlAsset_OnAssetReimported;
            Instance = this;
        }

        private void AmlAsset_OnAssetReimported(string path)
        {
            if (m_amlFile != null && m_amlFile.Path == path)
            {
                if (m_amlFile.Text.GetHashCode() == savingHash) return;

                bool reload = EditorUtility.DisplayDialog(
                    "Designer Warning",
                    $"[{m_amlFile.name}] has been changed outside Designer.\nDo you want to load the changes?",
                    "Reload", "Overwrite by Designer"
                    );

                if (reload) Reboot();
                else SaveToAml();
            }
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
            AmlAsset.OnAssetReimported -= AmlAsset_OnAssetReimported;

            Instance = null;
        }

        private UIElement m_target;
        private AmlAsset m_amlFile;
        private LogicTree m_logicTree;

        private UIElement m_selection;
        public UIElement Selection
        {
            get => m_selection;
            set
            {
                m_selection = value;
                drawContext.Clear();
                drawContext.Add(m_selection);

                UpdateCurrentDraw();
            }
        }

        private AmlNodeElement m_currentDrawTarget;
        private void UpdateCurrentDraw()
        {
            if (m_currentDrawTarget != null)
            {
                m_currentDrawTarget.OnDependencyPropertyChanged -= Target_OnDependencyPropertyChanged;
                m_currentDrawTarget.OnChildrenChanged -= Target_OnChildrenChanged;
            }

            if (drawContext.Count > 0)
                m_currentDrawTarget = drawContext[^1];

            if (m_currentDrawTarget != null)
            {
                m_currentDrawTarget.OnDependencyPropertyChanged += Target_OnDependencyPropertyChanged;
                m_currentDrawTarget.OnChildrenChanged += Target_OnChildrenChanged;
            }
        }

        public void StartDesginer(UIElement ui, AmlAsset amlFile)
        {
            indentQueue.Clear();
            indentQueue.Add(amlFile);
            savingHash = 0;
            SetTarget(ui, amlFile);
        }

        void SetTarget(UIElement ui, AmlAsset amlFile)
        {
            m_target = ui;
            m_amlFile = amlFile;
            m_logicTree = new LogicTree(m_target, HasIndent());
            m_logicTree.OnSelectItem += TreeViewSelectItemChanged;
            Selection = null;
        }

        public void Refresh()
        {
            m_logicTree?.Reload();
        }

        private void TreeViewSelectItemChanged(UIElement obj)
        {
            Selection = obj;
            SceneView.RepaintAll();
            AssetInventory.GetWindow<AssetInventory>().Repaint();
            DesignerTool.RaiseSelect(Selection);
            Focus();
        }

        public IEnumerable<UIElement> GetTreeItems()
        {
            return m_logicTree.GetUIs();
        }

        private void Target_OnDependencyPropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            SaveToAml();
        }

        internal void Reboot()
        {
            initAml(m_amlFile);
        }

        private List<AmlAsset> indentQueue = new List<AmlAsset>();
        internal void Restart(AmlAsset aa)
        {
            if (aa == m_amlFile) Reboot();
            else
            {
                indentQueue.Add(aa);
                savingHash = 0;
                initAml(aa);
            }
        }

        private void initAml(AmlAsset aa)
        {
            if (m_target != null) m_target.Close();
            var stage = PrefabStageUtility.GetCurrentPrefabStage();
            if (stage == null) return;

            var engine = stage.prefabContentsRoot.GetComponentInChildren<Engine>();

            var ui = engine.CreateUI(aa, engine.transform, null);
            SetTarget(ui, aa);
        }

        internal AmlAsset HasIndent()
        {
            return indentQueue.Count >= 2 ? indentQueue[^2] : null;
        }

        internal void BackIndent()
        {
            var frontAml = HasIndent();

            if (frontAml != null)
            {
                indentQueue.RemoveAt(indentQueue.Count - 1);
                if (frontAml != null)
                {
                    initAml(frontAml);
                }
            }
        }


        private void Target_OnChildrenChanged()
        {
            SaveToAml();
        }

        private List<AmlNodeElement> drawContext = new List<AmlNodeElement>();
        private Vector2 inspectorScoll;
        private void DrawInspector(Rect rect)
        {
            rect.width = position.width * 0.7f - 15;
            rect.height = position.height - 45;
            rect.position = new Vector2(position.width * 0.3f + 10, rect.position.y);
            G.Box(rect, string.Empty, EditorStyles.helpBox);
            if (Selection == null) return;
            if (Selection.NodeProxy == null) return;

            GL.BeginArea(rect, new GUIStyle { padding = new RectOffset(10, 10, 10, 10) });
            inspectorScoll = EGL.BeginScrollView(inspectorScoll);
            DrawElement(this);
            EGL.EndScrollView();
            GL.EndArea();
        }

        private void DesignerTool_OnSelected(UIElement obj)
        {
            m_logicTree.SelectWithoutNotify(obj);
            Selection = obj;
            SceneView.RepaintAll();

            Repaint();
        }

        private void SceneView_duringSceneGui(SceneView obj)
        {
            if (Selection == null) return;
            if (Selection.NodeProxy == null) return;

            List<IGrouping<string, DependencyProperty>> groups = GetDependencyGroups(Selection);

            foreach (var group in groups)
            {
                var groupName = group.Key;

                foreach (var property in group)
                {
                    var drawer = FindDrawer(property.PropType);
                    if (drawer == null) continue;

                    if (!property.Meta.IsReadOnly)
                    {
                        var value = drawer.OnSceneGUI(Selection, property.PropName, Selection.GetValue(property));
                        Selection.SetValue(property, value);
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

                GL.BeginArea(new Rect(rect) { height = 30, width = position.width });
                EGL.BeginHorizontal();
                if (GL.Button("ExitEdit"))
                {
                    StageUtility.GoToMainStage();
                    if (Settings.Get().BackLayout)
                    {
                        var path = AssetDatabase.GetAssetPath(Settings.Get().BackLayout);
                        EditorUtility.LoadWindowLayout(path);
                    }
                }
                if (m_amlFile != null && GL.Button("OpenAml"))
                {
                    AmlImporter.OverrideAMLOpen = false;
                    AssetDatabase.OpenAsset(m_amlFile);
                    AmlImporter.OverrideAMLOpen = true;
                }
                if (m_amlFile != null && GL.Button("Save"))
                {
                    SaveToAml();
                }
                GL.FlexibleSpace();
                EGL.EndHorizontal();
                GL.EndArea();
            }
            else
            {
                GL.BeginArea(new Rect(rect) { height = 30, width = position.width });
                EGL.BeginHorizontal();
                if (GL.Button("ExitEdit"))
                {
                    if (Settings.Get().BackLayout)
                    {
                        var path = AssetDatabase.GetAssetPath(Settings.Get().BackLayout);
                        EditorUtility.LoadWindowLayout(path);
                    }
                }

                GL.FlexibleSpace();
                EGL.EndHorizontal();
                GL.EndArea();
            }

            rect.position = new Vector2(5, rect.height + 30);

            DrawTree(rect);
            DrawInspector(rect);
        }

        private Vector2 treeScoll;
        private void DrawTree(Rect rect)
        {
            rect.width = position.width * 0.3f;
            rect.height = position.height - 45;
            G.Box(rect, string.Empty, EditorStyles.helpBox);

            rect.position += new Vector2(5, 5);
            rect.width -= 10; rect.height -= 10;
            if (m_logicTree != null)
            {
                var treeRect = rect;
                treeRect.height = Mathf.Max(rect.height, m_logicTree.totalHeight);

                treeScoll = G.BeginScrollView(rect, treeScoll, treeRect);
                m_logicTree.OnGUI(treeRect);
                if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
                {
                    if (Selection != null && Selection.UIParent != null)
                    {
                        Selection.UIParent.RemoveChild(Selection);
                        Selection.Close();
                        SaveToAml();
                    }
                }
                G.EndScrollView();
            }
        }

        public static UIElement AddChild(Type uiElementType, UIElement target)
        {
            var newUI = Activator.CreateInstance(uiElementType) as UIElement;
            newUI.Document = target.Document;
            newUI.Engine = target.Engine;
            target.AddChild(newUI);
            newUI.Initialize();
            newUI.RefreshPropertyNotify();
            newUI.Rect.SetParent(target.m_childRoot);
            target.Engine.SetDirty(target);
            Designer.SaveToAml();

            return newUI;
        }


        private int savingHash;
        internal static void SaveToAml()
        {
            var designer = Instance;
            if (designer == null) return;

            var str = AmlGenerator.Gen(designer.m_target);
            designer.m_amlFile.Text = str;
            designer.m_amlFile.SaveToDisk();
            designer.savingHash = designer.m_amlFile.Text.GetHashCode();
            designer.Refresh();

            Debug.Log($"{designer.m_amlFile.name} saved");
        }


        private static void DrawElement(Designer designer)
        {
            if (designer.drawContext.Count == 0) return;
            var target = designer.drawContext[^1];

            EGL.BeginHorizontal();
            DrawTitle(designer, target);
            DrawProperties(target);

            DrawCustom(designer, target);

            if (DrawChildElements(designer, target, target.Children) is AmlNodeElement select)
            {
                designer.drawContext.Add(select);
                designer.UpdateCurrentDraw();
            }
        }

        private static void DrawProperties(AmlNodeElement target)
        {
            List<IGrouping<string, DependencyProperty>> groups = GetDependencyGroups(target);
            foreach (var group in groups)
            {
                var groupName = group.Key;

                EGL.BeginVertical(EditorStyles.helpBox);

                if (EGL.Foldout(true, groupName))
                {
                    foreach (var property in group)
                    {
                        EGL.BeginHorizontal();

                        DrawDirtyMark(target, property);

                        var drawer = FindDrawer(property.PropType);
                        if (drawer == null)
                        {
                            var color = G.color;
                            G.color = Color.yellow;
                            EGL.LabelField(property.PropName, $"[{property.PropType}]");
                            G.color = color;
                        }
                        else
                        {
                            var canEdit = !property.Meta.IsReadOnly && target.GetBinding(property) == null;
                            using (new EG.DisabledGroupScope(!canEdit))
                            {
                                var value = drawer.Draw(target, property.PropName, target.GetValue(property));
                                if (canEdit)
                                    target.SetValue(property, value);
                            }
                        }

                        if (target.GetBinding(property) != null)
                        {
                            var bindRect = GUILayoutUtility.GetLastRect();
                            bindRect.width += 2;
                            bindRect.height += 2;
                            bindRect.position -= new Vector2(1, 1);
                            AlienEditorUtility.DrawBorder(bindRect, Color.yellow);
                        }

                        EGL.EndHorizontal();

                        {
                            var rightClickRect = GUILayoutUtility.GetLastRect();
                            using (AlienEditorUtility.BeginGUIColorScope(new Color(0, 0, 0, 0)))
                            {
                                G.Box(rightClickRect, string.Empty);
                            }
                            HandleContextMenu(target, property);
                        }

                        GL.Space(2);

                    }
                }

                EGL.EndVertical();
            }
        }

        private static void DrawTitle(Designer designer, AmlNodeElement target)
        {
            if (designer.drawContext.Count > 1)
            {
                for (int i = 0; i < designer.drawContext.Count - 1; i++)
                {
                    var parent = designer.drawContext[i];
                    var name = parent.Name ?? parent.GetType().Name;
                    if (GL.Button(new GUIContent(name, parent.GetIcon()), GUILayout.Height(18)))
                    {
                        designer.drawContext = designer.drawContext.Take(i + 1).ToList();
                        designer.UpdateCurrentDraw();
                        break;
                    }
                    GL.Label("/");
                }
            }
            EGL.LabelField(new GUIContent(target.GetType().Name, target.GetIcon()), new GUIStyle(EditorStyles.label) { fontSize = 18 }, GL.Height(20));
            GL.FlexibleSpace();
            EGL.EndHorizontal();
        }

        private static void DrawCustom(Designer designer, AmlNodeElement target)
        {
            var type = target.GetType();
            while (type != typeof(AmlNodeElement))
            {
                if (m_elementEditors.TryGetValue(type, out var editor))
                {
                    editor.Draw(designer.Selection, target);
                    break;
                }
                else
                {
                    type = type.BaseType;
                }
            }
        }

        private static AmlNodeElement DrawChildElements(Designer designer, AmlNodeElement parent, IList<AmlNodeElement> children)
        {
            AmlNodeElement select = null;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                if (child is UIElement) continue;
                EGL.BeginHorizontal();
                {
                    if (GL.Button($"Edit {child.Name ?? child.GetType().Name}"))
                    {
                        select = child;
                    }
                    using (AlienEditorUtility.BeginGUIColorScope(Color.red))
                    {
                        if (GL.Button("DEL", new GUIStyle(G.skin.button) { alignment = TextAnchor.MiddleCenter }, GL.Width(40)))
                        {
                            parent.RemoveChild(child);
                            i--;
                        }
                    }
                }
                EGL.EndHorizontal();
            }

            using (AlienEditorUtility.BeginGUIColorScope(Color.green))
            {
                EGL.BeginHorizontal();
                {
                    var temp = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 80;
                    var collector = parent.Engine.AttParser.Collector;

                    var allowChildTypes = parent.GetAllowChildTypes().ToList();
                    var options = allowChildTypes.Select(t => t.Name).ToList();
                    options.Insert(0, "------");
                    int selectChild = EGL.Popup("Add Child", 0, options.ToArray());
                    if (selectChild > 0)
                    {
                        var childType = allowChildTypes[selectChild - 1];
                        var newChild = Activator.CreateInstance(childType) as AmlNodeElement;
                        newChild.Document = parent.Document;
                        newChild.Engine = parent.Engine;
                        parent.AddChild(newChild);
                    }
                    EditorGUIUtility.labelWidth = temp;
                }
                EGL.EndHorizontal();
            }

            return select;
        }

        private static void DrawDirtyMark(AmlNodeElement target, DependencyProperty property)
        {
            var currentDPValue = target.GetValue(property);
            var defaultDPValue = property.Meta.DefaultValue;
            var color = G.color;
            ColorUtility.TryParseHtmlString(currentDPValue != defaultDPValue ? "#0f80be" : "#00000000", out var dirtyColor);
            G.color = dirtyColor;
            GL.Box(string.Empty, EditorStyles.selectionRect, GL.Width(2));
            G.color = color;
        }

        private static List<IGrouping<string, DependencyProperty>> GetDependencyGroups(AmlNodeElement target)
        {
            var propties = target.GetAllDependencyProperties().Where(p => !p.Meta.IsAmlDisable && !p.IsAttachedProerty);
            if (target.Parent != null)
            {
                var parentAttProps = target.Parent.GetAllDependencyProperties().Where(p => !p.Meta.IsAmlDisable && p.IsAttachedProerty);
                propties = propties.Concat(parentAttProps);
            }
            var groups = propties.GroupBy(p => p.IsAttachedProerty ? $"AttachedProperty From {p.AttachHostType}" : p.Meta.Group).ToList();
            return groups;
        }

        private static void HandleContextMenu(AmlNodeElement selection, DependencyProperty property)
        {
            if (Event.current.type != EventType.ContextClick) return;
            Vector2 mousePos = Event.current.mousePosition;
            if (!GUILayoutUtility.GetLastRect().Contains(mousePos)) return;

            GenericMenu menu = new GenericMenu();

            // Use MenuItem as Title
            menu.AddDisabledItem(new GUIContent($"---{selection}.{property.PropName}---"));

            if (selection.GetBinding(property) == null)
            {
                var currentDPValue = selection.GetValue(property);
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
                            selection.SetValue(property, property.Meta.DefaultValue);
                        });
                    }
                }

                menu.AddItem(new GUIContent("Set Binding"), false, () =>
                {
                    TextInputWindow.ShowWindow("Set Binding", string.Empty, mousePos, (inputSorce) =>
                    {
                        SetBinding(selection, property, inputSorce);
                    });
                });
            }
            else
            {
                menu.AddItem(new GUIContent("Edit Binding"), false, () =>
                {
                    TextInputWindow.ShowWindow("Edit Binding", selection.GetBinding(property).SourceCode, mousePos, (inputSorce) =>
                    {
                        selection.GetBinding(property).Disconnect();
                        SetBinding(selection, property, inputSorce);
                    });
                });
                menu.AddItem(new GUIContent("Remove Binding"), false, () =>
                {
                    selection.GetBinding(property).Disconnect();
                });
            }

            menu.ShowAsContext();

            Event.current.Use();
        }

        private static void SetBinding(AmlNodeElement selection, DependencyProperty property, string inputSorce)
        {
            if (BindUtility.IsBindingString(inputSorce, out Match match))
            {
                var bindType = BindUtility.ParseBindParam(match, out string propName, out string converterName, out string modeName);
                object source = null;
                switch (bindType)
                {
                    case EnumBindingType.Binding: source = selection.DataContext; break;
                    case EnumBindingType.TemplateBinding: source = selection.Document.m_templateHost; break;
                    default:
                        Engine.LogError("BindType Invalid");
                        break;
                }

                source.BeginBind(inputSorce)
                    .SetSourceProperty(propName)
                    .SetTarget(selection)
                    .SetTargetProperty(property.PropName)
                    .Apply(converterName, modeName, selection.Parent);

                SaveToAml();
            }
        }

        private static PropertyDrawerBase FindDrawer(Type propertyType)
        {
            if (m_defaultDrawers.TryGetValue(propertyType, out var drawer))
                return drawer;
            else if (propertyType.BaseType != null)
                return FindDrawer(propertyType.BaseType);
            else
                return null;
        }


    }
}
