using AlienUI.Core.Commnands;
using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace AlienUI.Editors.PropertyDrawer
{
    public class IntDrawer : PropertyDrawer<int>
    {
        protected override int OnDraw(AmlNodeElement host, string label, int value)
        {
            return EditorGUILayout.IntField(label, value);
        }
    }

    public class StringDrawer : PropertyDrawer<string>
    {
        protected override string OnDraw(AmlNodeElement host, string label, string value)
        {
            var result = EditorGUILayout.TextField(label, value);
            return result;
        }
    }

    public class CommandDrawer : PropertyDrawer<CommandBase>
    {
        protected override CommandBase OnDraw(AmlNodeElement host, string label, CommandBase value)
        {
            var dp = host.GetDependencyProperty(label);
            var type = dp.PropType;

            var paramTypes = type.GenericTypeArguments;
            if (paramTypes == null || paramTypes.Length == 0)
                EditorGUILayout.LabelField(label, "()");
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("(");
                for (int i = 0; i < paramTypes.Length; i++)
                {
                    var paramType = paramTypes[i];
                    sb.Append(paramType.Name);
                    if (i < paramTypes.Length - 1) sb.Append(",");
                }
                sb.Append(")");
                EditorGUILayout.LabelField(label, sb.ToString());
            }
            return value;
        }
    }

    public class ControlTemplateDrawer : PropertyDrawer<ControlTemplate>
    {
        protected override ControlTemplate OnDraw(AmlNodeElement host, string label, ControlTemplate value)
        {
            var templates = Settings.Get().GetAllTemplateAssetsByTargetType(host.GetType());
            var templateNames = templates.Select(t => t.name).ToList();
            var currentSelect = templateNames.IndexOf(value.Name);
            currentSelect = EditorGUILayout.Popup(label, currentSelect, templateNames.ToArray());

            var selectTemplate = currentSelect == -1 ? value : new ControlTemplate(templateNames[currentSelect]);
            return selectTemplate;
        }
    }

    public class ItemTemplateDrawer : PropertyDrawer<ItemTemplate>
    {
        protected override ItemTemplate OnDraw(AmlNodeElement host, string label, ItemTemplate value)
        {
            var templates = Settings.Get().GetUIAssets();
            var templateNames = templates.Select(t => t.name).ToList();
            var currentSelect = templateNames.IndexOf(value.Name);
            currentSelect = EditorGUILayout.Popup(label, currentSelect, templateNames.ToArray());

            var selectTemplate = currentSelect == -1 ? value : new ItemTemplate(templateNames[currentSelect]);
            return selectTemplate;
        }
    }

    public class NumberDrawer : PropertyDrawer<Number>
    {
        protected override Number OnDraw(AmlNodeElement host, string label, Number value)
        {
            if (!value.Auto) value.Value = EditorGUILayout.FloatField(label, value.Value);
            else using (new EditorGUI.DisabledGroupScope(true)) EditorGUILayout.TextField(label, "Auto");

            value.Auto = GUILayout.Toggle(value.Auto, "Auto", EditorStyles.miniButton);

            return value;
        }
    }

    public class EnumDrawer : PropertyDrawer<Enum>
    {
        protected override Enum OnDraw(AmlNodeElement host, string label, Enum value)
        {
            return EditorGUILayout.EnumPopup(label, value);
        }
    }

    public class BorderDataDrawer : PropertyDrawer<BorderData>
    {
        protected override BorderData OnDraw(AmlNodeElement host, string label, BorderData value)
        {
            EditorGUILayout.LabelField(label);
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.Space(80, true);
                var rect = GUILayoutUtility.GetLastRect();

                GUI.Box(rect, string.Empty, EditorStyles.helpBox);
                GUIStyle style = new GUIStyle(EditorStyles.textField) { alignment = TextAnchor.MiddleCenter, fontSize = 10 };


                float size = 30;
                rect.position += new Vector2(rect.width * 0.5f - size * 0.5f, 5);
                rect.width = size; rect.height = size;
                value.top = EditorGUI.FloatField(rect, value.top, style); //top
                rect.position += new Vector2(0, size + 5);
                value.bottom = EditorGUI.FloatField(rect, value.bottom, style);//down
                rect.position += new Vector2(-size - 5, -size * 0.5f - 2.5f);
                value.left = EditorGUI.FloatField(rect, value.left, style);//left
                rect.position += new Vector2(size * 2 + 5 * 2, 0);
                value.right = EditorGUI.FloatField(rect, value.right, style);//right
                return value;
            }
        }
    }

    public class Vector2Drawer : PropertyDrawer<Vector2>
    {
        protected override Vector2 OnDraw(AmlNodeElement host, string label, Vector2 value)
        {
            value = EditorGUILayout.Vector2Field(label, value);
            return value;
        }
    }

    public class BooleanDrawer : PropertyDrawer<bool>
    {
        protected override bool OnDraw(AmlNodeElement host, string label, bool value)
        {
            return EditorGUILayout.Toggle(label, value);
        }
    }

    public class FloatDrawer : PropertyDrawer<float>
    {
        protected override float OnDraw(AmlNodeElement host, string label, float value)
        {
            return EditorGUILayout.FloatField(label, value);
        }
    }

    public class GridDefineDrawer : PropertyDrawer<GridDefine>
    {
        protected override GridDefine OnDrawSceneGUI(AmlNodeElement host, string label, GridDefine value)
        {
            if (host is UIElements.Grid grid)
            {
                int col = grid.GridDefine.Column;
                int row = grid.GridDefine.Row;
                grid.GridDefine.GetDefines(out var colDefs, out var rowDefs);
                for (int i = 0; i < col * row; i++)
                {
                    int x = i % col;
                    int y = i / col;
                    var size = grid.GridDefine.GetCellSize(x, y);
                    var pos = grid.GridDefine.GetCellOffset(x, y);

                    var labelStyle = new GUIStyle(EditorStyles.label);
                    labelStyle.normal.background = AlienEditorUtility.MakeTex(2, 2, Color.gray);
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    grid.Rect.DrawSceneBorder(pos, size, Color.yellow, (rect) =>
                    {
                        GUILayout.FlexibleSpace();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField($"{x},{y}", labelStyle, GUILayout.Width(30));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();

                        if (y == 0 && colDefs.Count > 1)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("Col:", labelStyle, GUILayout.Width(30));
                            var colDef = colDefs[x];
                            colDef.DefineType = (GridDefine.EnumDefineType)EditorGUILayout.EnumPopup(colDef.DefineType, GUILayout.Width(60));
                            colDef.Value = EditorGUILayout.FloatField(colDef.Value, GUILayout.Width(30));
                            colDefs[x] = colDef;
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                        }

                        if (x == 0 && rowDefs.Count > 1)
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.LabelField("Row:", labelStyle, GUILayout.Width(30));
                            var rowDef = rowDefs[y];
                            rowDef.DefineType = (GridDefine.EnumDefineType)EditorGUILayout.EnumPopup(rowDef.DefineType, GUILayout.Width(60));
                            rowDef.Value = EditorGUILayout.FloatField(rowDef.Value, GUILayout.Width(30));
                            rowDefs[y] = rowDef;
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndHorizontal();
                        }

                        GUILayout.FlexibleSpace();
                    });
                }

                return new GridDefine(colDefs.ToArray(), rowDefs.ToArray());
            }
            else
            {
                return value;
            }
        }

        protected override GridDefine OnDraw(AmlNodeElement host, string label, GridDefine value)
        {
            EditorGUILayout.LabelField(label, new GUIStyle(EditorStyles.label), GUILayout.Height(25));
            var rect = GUILayoutUtility.GetLastRect();
            rect.position += new Vector2(80, 0);
            rect.width -= 80;
            ////GUILayout.BeginArea(rect);
            //GUILayout.Button("!!!!!");
            ////GUILayout.EndArea();

            GUI.BeginGroup(rect);
            EditorGUI.LabelField(new Rect(0, 0, 30, 20), "Col");
            int newColumn = EditorGUI.IntField(new Rect(35, 0, 30, 20), value.Column);
            EditorGUI.LabelField(new Rect(80, 0, 30, 20), "Row");
            int newRow = EditorGUI.IntField(new Rect(115, 0, 30, 20), value.Row);
            GUI.EndGroup();

            value.GetDefines(out var colDefs, out var rowDefs);

            if (newColumn != value.Column || newRow != value.Row)
            {
                while (newColumn != colDefs.Count || newRow != rowDefs.Count)
                {
                    if (newColumn > colDefs.Count)
                        colDefs.Add(new GridDefine.Define { DefineType = GridDefine.EnumDefineType.Weight, Value = 1 });
                    else if (newColumn < colDefs.Count)
                        colDefs.RemoveAt(colDefs.Count - 1);

                    if (newRow > rowDefs.Count)
                        rowDefs.Add(new GridDefine.Define { DefineType = GridDefine.EnumDefineType.Weight, Value = 1 });
                    else if (newRow < rowDefs.Count)
                        rowDefs.RemoveAt(newRow - 1);
                }

                value = new GridDefine(colDefs.ToArray(), rowDefs.ToArray());
            }

            return value;
        }
    }

    public class Vector2IntDrawer : PropertyDrawer<Vector2Int>
    {
        protected override Vector2Int OnDraw(AmlNodeElement host, string label, Vector2Int value)
        {
            return EditorGUILayout.Vector2IntField(label, value);
        }
    }

    public class ColorDrawer : PropertyDrawer<Color>
    {
        protected override Color OnDraw(AmlNodeElement host, string label, Color value)
        {
            return EditorGUILayout.ColorField(label, value);
        }
    }

    public class SpriteDrawer : PropertyDrawer<Sprite>
    {
        protected override Sprite OnDraw(AmlNodeElement host, string label, Sprite value)
        {
            EditorGUILayout.BeginVertical();
            value = (Sprite)EditorGUILayout.ObjectField(label, value, typeof(Sprite), false);
            if (value != null)
            {
                if (Settings.Get().GetUnityAssetPath(value, out string group, out string assetName))
                {
                    EditorGUILayout.HelpBox($"Sprite From {group}.{assetName}", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Not Found In UnityAssetRef", MessageType.Error);
                }
            }
            EditorGUILayout.EndVertical();
            return value;
        }
    }

    public class FontDrawer : PropertyDrawer<Font>
    {
        protected override Font OnDraw(AmlNodeElement host, string label, Font value)
        {
            return (Font)EditorGUILayout.ObjectField(label, value, typeof(Font), false);
        }
    }

    public class DependencyObjectRefDrawer : PropertyDrawer<DependencyObjectRef>
    {
        protected override DependencyObjectRef OnDraw(AmlNodeElement host, string label, DependencyObjectRef value)
        {
            var tag = EditorGUILayout.TextField(label, value.GetUniqueTag());
            return value.SetUniqueTag(tag);
        }
    }

    public class AnimationCurveDrawer : PropertyDrawer<AnimationCurve>
    {
        protected override AnimationCurve OnDraw(AmlNodeElement host, string label, AnimationCurve value)
        {
            return EditorGUILayout.CurveField(label, new AnimationCurve(value.keys));
        }
    }

    public class HorizontalDrawer : PropertyDrawer<eHorizontalAlign>
    {
        // Horizontal Alignment Icons
        private readonly GUIContent left = new GUIContent(AlienEditorUtility.GetIcon("Left"));
        private readonly GUIContent Middle = new GUIContent(AlienEditorUtility.GetIcon("Center"));
        private readonly GUIContent Right = new GUIContent(AlienEditorUtility.GetIcon("Right"));
        private readonly GUIContent Stretch = new GUIContent(AlienEditorUtility.GetIcon("Stretch_H"));


        protected override eHorizontalAlign OnDraw(AmlNodeElement host, string label, eHorizontalAlign value)
        {
            EditorGUILayout.LabelField(label);
            GUILayout.FlexibleSpace();

            if (GUILayout.Toggle(value == eHorizontalAlign.Left, left, EditorStyles.toolbarButton))
                value = eHorizontalAlign.Left;
            if (GUILayout.Toggle(value == eHorizontalAlign.Middle, Middle, EditorStyles.toolbarButton))
                value = eHorizontalAlign.Middle;
            if (GUILayout.Toggle(value == eHorizontalAlign.Right, Right, EditorStyles.toolbarButton))
                value = eHorizontalAlign.Right;
            if (GUILayout.Toggle(value == eHorizontalAlign.Stretch, Stretch, EditorStyles.toolbarButton))
                value = eHorizontalAlign.Stretch;

            return value;
        }
    }

    public class VerticalDrawer : PropertyDrawer<eVerticalAlign>
    {
        // Horizontal Alignment Icons
        private readonly GUIContent Top = new GUIContent(AlienEditorUtility.GetIcon("Upper"));
        private readonly GUIContent Middle = new GUIContent(AlienEditorUtility.GetIcon("Middle"));
        private readonly GUIContent Bottom = new GUIContent(AlienEditorUtility.GetIcon("Lower"));
        private readonly GUIContent Stretch = new GUIContent(AlienEditorUtility.GetIcon("Stretch_V"));


        protected override eVerticalAlign OnDraw(AmlNodeElement host, string label, eVerticalAlign value)
        {
            EditorGUILayout.LabelField(label);
            GUILayout.FlexibleSpace();

            if (GUILayout.Toggle(value == eVerticalAlign.Top, Top, EditorStyles.toolbarButton))
                value = eVerticalAlign.Top;
            if (GUILayout.Toggle(value == eVerticalAlign.Middle, Middle, EditorStyles.toolbarButton))
                value = eVerticalAlign.Middle;
            if (GUILayout.Toggle(value == eVerticalAlign.Bottom, Bottom, EditorStyles.toolbarButton))
                value = eVerticalAlign.Bottom;
            if (GUILayout.Toggle(value == eVerticalAlign.Stretch, Stretch, EditorStyles.toolbarButton))
                value = eVerticalAlign.Stretch;

            return value;
        }
    }
}
