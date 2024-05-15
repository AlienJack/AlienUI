using AlienUI.Core.Commnands;
using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AlienUI.Editors.PropertyDrawer
{
    public class IntDrawer : PropertyDrawer<int>
    {
        protected override int OnDraw(UIElement host, string label, int value)
        {
            return EditorGUILayout.IntField(label, value);
        }
    }

    public class StringDrawer : PropertyDrawer<string>
    {
        protected override string OnDraw(UIElement host, string label, string value)
        {
            return EditorGUILayout.TextField(label, value);
        }
    }

    public class CommandDrawer : PropertyDrawer<CommandBase>
    {
        protected override CommandBase OnDraw(UIElement host, string label, CommandBase value)
        {
            var type = value.GetType();
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
        protected override ControlTemplate OnDraw(UIElement host, string label, ControlTemplate value)
        {
            var templates = Settings.Get().GetAllTemplateAssetsByTargetType(host.GetType());
            var templateNames = templates.Select(t => t.name).ToList();
            var currentSelect = templateNames.IndexOf(value.Name);
            currentSelect = EditorGUILayout.Popup(label, currentSelect, templateNames.ToArray());

            var selectTemplate = currentSelect == -1 ? value : new ControlTemplate(templateNames[currentSelect]);
            return selectTemplate;
        }
    }

    public class NumberDrawer : PropertyDrawer<Number>
    {
        protected override Number OnDraw(UIElement host, string label, Number value)
        {
            if (!value.Auto) value.Value = EditorGUILayout.FloatField(label, value.Value);
            else using (new EditorGUI.DisabledGroupScope(true)) EditorGUILayout.TextField(label, "Auto");

            value.Auto = GUILayout.Toggle(value.Auto, "Auto", EditorStyles.miniButton);

            return value;
        }
    }

    public class EnumDrawer : PropertyDrawer<Enum>
    {
        protected override Enum OnDraw(UIElement host, string label, Enum value)
        {
            return EditorGUILayout.EnumPopup(label, value);
        }
    }

    public class BorderDataDrawer : PropertyDrawer<BorderData>
    {
        protected override BorderData OnDraw(UIElement host, string label, BorderData value)
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
        protected override Vector2 OnDraw(UIElement host, string label, Vector2 value)
        {
            value = EditorGUILayout.Vector2Field(label, value);
            return value;
        }
    }

    public class BooleanDrawer : PropertyDrawer<bool>
    {
        protected override bool OnDraw(UIElement host, string label, bool value)
        {
            return EditorGUILayout.Toggle(label, value);
        }
    }

    public class FloatDrawer : PropertyDrawer<float>
    {
        protected override float OnDraw(UIElement host, string label, float value)
        {
            return EditorGUILayout.FloatField(label, value);
        }
    }

    public class GridDefineDrawer : PropertyDrawer<GridDefine>
    {
        protected override GridDefine OnDrawSceneGUI(UIElement host, string label, GridDefine value)
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

        protected override GridDefine OnDraw(UIElement host, string label, GridDefine value)
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
        protected override Vector2Int OnDraw(UIElement host, string label, Vector2Int value)
        {
            return EditorGUILayout.Vector2IntField(label, value);
        }
    }

    public class ColorDrawer : PropertyDrawer<Color>
    {
        protected override Color OnDraw(UIElement host, string label, Color value)
        {
            return EditorGUILayout.ColorField(label, value);
        }
    }

    public class SpriteDrawer : PropertyDrawer<Sprite>
    {
        protected override Sprite OnDraw(UIElement host, string label, Sprite value)
        {
            return (Sprite)EditorGUILayout.ObjectField(label, value, typeof(Sprite), false);
        }
    }

    public class FontDrawer : PropertyDrawer<Font>
    {
        protected override Font OnDraw(UIElement host, string label, Font value)
        {
            return (Font)EditorGUILayout.ObjectField(label, value, typeof(Font), false);
        }
    }
}
