using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AlienUI.Editors
{
    public class Designer : EditorWindow
    {
        private UIElement m_target;
        private AmlAsset m_amlFile;
        private LogicTree m_logicTree;

        public void SetTarget(UIElement ui, AmlAsset amlFile)
        {
            m_target = ui;
            m_amlFile = amlFile;
            m_logicTree = new LogicTree(m_target);
        }

        private void OnGUI()
        {
            var rect = new Rect();
            rect.position += new Vector2(5, 5);
            if (PrefabStageUtility.GetCurrentPrefabStage() is PrefabStage stage)
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
                        //AssetDatabase.OpenAsset(m_amlFile);
                    }
                }
            }

            rect.position = new Vector2(5, rect.height + 10);

            DrawTree(rect);

        }

        private void DrawTree(Rect rect)
        {
            rect.width = position.width * 0.3f;
            rect.height = position.height - 45;
            //EditorGUI.DrawRect(rect, Color.red);
            GUI.Box(rect, string.Empty, EditorStyles.helpBox);

            rect.position += new Vector2(5, 5);
            rect.width -= 10; rect.height -= 10;
            if (m_logicTree != null)
                m_logicTree.OnGUI(rect);
        }
    }
}
