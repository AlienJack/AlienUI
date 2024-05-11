using AlienUI.Core.Commnands;
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
        protected override int OnDraw(int value)
        {
            return EditorGUILayout.IntField(value);
        }
    }

    public class StringDrawer : PropertyDrawer<string>
    {
        protected override string OnDraw(string value)
        {
            return EditorGUILayout.TextField(value);
        }
    }

    public class CommandDrawer : PropertyDrawer<CommandBase>
    {
        protected override CommandBase OnDraw(CommandBase value)
        {
            var type = value.GetType();
            var paramTypes = type.GenericTypeArguments;
            if (paramTypes == null || paramTypes.Length == 0)
                EditorGUILayout.LabelField("()");
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
                EditorGUILayout.LabelField(sb.ToString());
            }
            return value;
        }
    }

    public class ControlTemplateDrawer : PropertyDrawer<ControlTemplate>
    {
        protected override ControlTemplate OnDraw(ControlTemplate value)
        {
            var templates = Settings.Get().GetAllTemplateAssets();
            var templateNames = templates.Select(t => t.name).ToList();
            var currentSelect = templateNames.IndexOf(value.Name);
            currentSelect = EditorGUILayout.Popup(currentSelect, templateNames.ToArray());

            var selectTemplate = currentSelect == -1 ? value : new ControlTemplate(templateNames[currentSelect]);
            return selectTemplate;
        }
    }
}
