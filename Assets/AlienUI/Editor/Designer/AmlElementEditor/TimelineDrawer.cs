using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AlienUI.Editors.TimelineDrawer
{
    public abstract class TimelineDrawer<T>
    {
        protected T m_dataContext;

        public float Scale = 30f;

        public TimelineDrawer(T timelineObj)
        {
            m_dataContext = timelineObj;
        }

        public void Draw(Rect rect)
        {
            var infoDrawRect = new Rect(rect);
            infoDrawRect.width = rect.width * 0.4f;
            var keyRegionDrawRect = new Rect(rect);
            keyRegionDrawRect.width = rect.width - infoDrawRect.width;
            keyRegionDrawRect.x += infoDrawRect.width;

            DrawInfo(infoDrawRect);
            DrawKeyRegion(keyRegionDrawRect);
        }

        private void DrawInfo(Rect infoDrawRect)
        {
            OnDrawInfo(infoDrawRect, m_dataContext);
        }

        private int hoverKeyIndex = -1;
        private Rect hoverKeyRect = default;

        private Vector2 keyRegionScoll;
        private void DrawKeyRegion(Rect keyRegionDrawRect)
        {
            GUI.Box(keyRegionDrawRect, string.Empty);
            var keys = GetKeyTime(m_dataContext);

            float totalTime = keys.Max();

            keyRegionScoll = GUI.BeginScrollView(keyRegionDrawRect, keyRegionScoll,
                new Rect(keyRegionDrawRect)
                {
                    width = totalTime * Scale + 10 * Scale,
                    height = keyRegionDrawRect.height - 20
                });

            //画刻度
            for (int i = 0; i <= (int)(totalTime + 30) * 10; i++)
            {
                var time = i * 0.1f;

                bool atSecond = i % 10 == 0;
                var rect = new Rect(keyRegionDrawRect);
                rect.width = atSecond ? 2f : 1f;
                rect.height = atSecond ? 30f : 15f;
                rect.y = keyRegionDrawRect.y;
                rect.x = keyRegionDrawRect.x + time * Scale + 5f - rect.width * 0.5f;

                GUI.DrawTexture(rect, Texture2D.linearGrayTexture, ScaleMode.StretchToFill);

                var timelabelRect = new Rect(rect);
                timelabelRect.y += 30f + 5f;
                timelabelRect.width = 30f;
                timelabelRect.height = 30f;
                timelabelRect.x -= timelabelRect.width * 0.5f;
                if (atSecond)
                    EditorGUI.LabelField(timelabelRect, time.ToString(), new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });
                else
                {
                    var fadeAlpha = 1f;
                    if (Scale > 250) fadeAlpha = 1f;
                    else if (Scale <= 250 && Scale >= 240) fadeAlpha = 1 - ((250 - Scale) / 10f);
                    else fadeAlpha = 0f;

                    var temp = GUI.color;
                    GUI.color = new Color(1, 1, 1, fadeAlpha);
                    EditorGUI.LabelField(timelabelRect, time.ToString(), new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter });
                    GUI.color = temp;
                }
            }

            hoverKeyIndex = -1;
            //画关键帧
            for (int i = 0; i < keys.Count; i++)
            {
                var rect = new Rect(keyRegionDrawRect);
                rect.width = 11f;
                rect.height = 11f;
                rect.y = keyRegionDrawRect.y;
                rect.x = keyRegionDrawRect.x + keys[i] * Scale;

                GUI.DrawTexture(rect, AlienEditorUtility.GetIcon("key"), ScaleMode.StretchToFill);
                GUIUtility.GetControlID("timelinekey".GetHashCode(), FocusType.Passive);
                if (rect.Contains(Event.current.mousePosition))
                {
                    hoverKeyIndex = i;
                    hoverKeyRect = rect;
                }
            }

            GUI.EndScrollView();

            if (hoverKeyIndex != -1)
            {
                var startPos = hoverKeyRect.position;
                startPos.x += 20;
                startPos.y += 5;
                OnKeyTipDraw(startPos, hoverKeyIndex);
            }
            Designer.Instance.Repaint();
        }

        protected abstract void OnKeyTipDraw(Vector2 position, int keyIndex);
        protected abstract List<float> GetKeyTime(T target);
        protected abstract void OnDrawInfo(Rect infoDrawRect, T target);
    }
}
