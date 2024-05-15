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
        private static event Action<UIElement> OnRequestSelect;

        public override void OnActivated()
        {
            currentPick = null;
            targetUI = null;

            OnRequestSelect += DesignerTool_OnRequestSelect;
        }

        private void DesignerTool_OnRequestSelect(UIElement obj)
        {
            targetUI = obj;

            SceneView.RepaintAll();
        }

        public override void OnWillBeDeactivated()
        {
            currentPick = null;
            targetUI = null;

            OnRequestSelect -= DesignerTool_OnRequestSelect;
        }

        public override void OnToolGUI(EditorWindow window)
        {
            var evt = Event.current;
            if (evt.type == EventType.MouseDown && evt.button == 0)
            {
                evt.Use();

                if (Designer.Instance == null) return;

                var uiElements = Designer.Instance.GetTreeItems();

                var filter = uiElements.Select(u => u.Rect.gameObject).ToArray();

                currentPick = HandleUtility.PickGameObject(
                    evt.mousePosition, false,
                    currentPick == null ? null : new GameObject[] { currentPick },
                    filter);
                if (currentPick == null) return;

                targetUI = currentPick.GetComponent<NodeProxy>().TargetObject;
                OnSelected?.Invoke(targetUI);
            }

            if (targetUI != null)
            {
                if (targetUI.Rect != null)
                    AlienEditorUtility.DrawSceneBorder(targetUI.Rect, Vector2.zero, new Vector2(targetUI.ActualWidth, targetUI.ActualHeight), Color.cyan);
                else
                    targetUI = null;
            }
        }

        internal static void RaiseSelect(UIElement m_selection)
        {
            OnRequestSelect?.Invoke(m_selection);
        }
    }
}
