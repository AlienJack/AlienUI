using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AlienUI.Editors
{
    public static class AlienEditorUtility
    {
        public static void DrawBorder(Rect rect, Color color)
        {
            Handles.BeginGUI();
            // 设置线条颜色为黄色
            Handles.color = color;
            // 绘制矩形框
            Handles.DrawWireCube(new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0), new Vector3(rect.width, rect.height, 0));
            Handles.EndGUI();
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            Color32[] pix = new Color32[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }

            Texture2D result = new(width, height);
            result.SetPixels32(pix);
            result.Apply();
            return result;
        }

        public static void DrawSceneBorder(this RectTransform rectTrans, Vector2 leftTopPos, Vector2 size, Color drawColor, Action<Rect> OnGUI = null)
        {
            var gridRect = rectTrans;
            size *= gridRect.localScale;

            Vector2 offset = (Vector3)gridRect.rect.size * new Vector2(rectTrans.pivot.x, 1 - rectTrans.pivot.y);
            offset.Scale(new Vector3(1, -1, 1));
            leftTopPos -= offset;

            var posLT = gridRect.TransformPoint(leftTopPos);
            var posRT = posLT + new Vector3(size.x, 0, 0);
            var posRB = posRT + new Vector3(0, -size.y, 0);
            var posLB = posRB + new Vector3(-size.x, 0, 0);

            var temp = Handles.color;
            Handles.color = drawColor;
            Handles.DrawLine(posLT, posRT, 2);
            Handles.DrawLine(posRT, posRB, 2);
            Handles.DrawLine(posRB, posLB, 2);
            Handles.DrawLine(posLB, posLT, 2);
            Handles.color = temp;

            if (OnGUI != null)
            {
                Handles.BeginGUI();

                var screenPos = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(posLT);
                screenPos.y = SceneView.lastActiveSceneView.camera.pixelHeight - screenPos.y;

                var screenPosMax = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(posRB);
                screenPosMax.y = SceneView.lastActiveSceneView.camera.pixelHeight - screenPosMax.y;

                var rectSize = screenPosMax - screenPos;
                var rect = new Rect { position = screenPos, width = rectSize.x, height = rectSize.y };

                GUILayout.BeginArea(rect);
                OnGUI(rect);
                GUILayout.EndArea();

                Handles.EndGUI();
            }
        }
    }
}
