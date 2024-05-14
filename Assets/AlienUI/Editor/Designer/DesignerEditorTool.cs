using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AlienUI.Editors
{
    [EditorTool("AlienUI Designer")]
    public class DesignerTool : EditorTool
    {
        private GameObject currentPick;
        private UIElement targetUI;

        public static event Action<UIElement> OnSelected;

        public override void OnToolGUI(EditorWindow window)
        {
            var evt = Event.current;
            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                evt.Use();

                var go = HandleUtility.PickGameObject(evt.mousePosition, false);
                if (go == null) return;

                GameObject topGo = go;
                while (topGo.transform.parent != null)
                {
                    topGo = topGo.transform.parent.gameObject;
                }

                var proxys = topGo.GetComponentsInChildren<NodeProxy>();

                currentPick = HandleUtility.PickGameObject(evt.mousePosition, false, currentPick == null ? null : new GameObject[] { currentPick }, proxys.Where(p => p.TargetObject.TemplateHost == null).Select(p => p.gameObject).ToArray());
                if (currentPick == null) return;

                targetUI = currentPick.GetComponent<NodeProxy>().TargetObject;
                OnSelected?.Invoke(targetUI);
            }

            if (targetUI != null)
            {
                AlienEditorUtility.DrawSceneBorder(targetUI.Rect, Vector2.zero, new Vector2(targetUI.ActualWidth, targetUI.ActualHeight), Color.cyan);
            }
        }
    }
}
