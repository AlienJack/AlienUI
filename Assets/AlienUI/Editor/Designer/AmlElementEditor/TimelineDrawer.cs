using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Editors.TimelineDrawer
{
    public abstract class TimelineDrawer<T>
    {
        private T m_dataContext;

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

        private void DrawKeyRegion(Rect keyRegionDrawRect)
        {
            GUI.Box(keyRegionDrawRect, string.Empty);
        }

        protected abstract void OnDrawInfo(Rect infoDrawRect, T target);
    }
}
