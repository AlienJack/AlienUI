using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AlienUI.Editors.PropertyDrawer
{
    public class IntDrawer : PropertyDrawer<int>
    {
        protected override int OnDraw(Rect rect, int value)
        {
            return EditorGUI.IntField(rect, value);
        }
    }

    public class StringDrawer : PropertyDrawer<string>
    {
        protected override string OnDraw(Rect rect, string value)
        {
            return EditorGUI.TextField(rect, value);
        }
    }
}
