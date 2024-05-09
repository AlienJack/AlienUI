using AlienUI.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

namespace AlienUI.Editors
{
    [CustomEditor(typeof(AmlAsset))]
    public class AmlInspector : Editor
    {
        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            var tar = target as AmlAsset;
            EditorGUILayout.TextArea(tar.Text, new GUIStyle(EditorStyles.textArea) { wordWrap = true });
            EditorGUILayout.EndVertical();
        }
    }
}
