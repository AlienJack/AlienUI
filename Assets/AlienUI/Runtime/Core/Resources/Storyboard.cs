using AlienUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public class Storyboard : Resource
    {
        private List<Animation> m_animations = new List<Animation>();
        private float m_currentTime;
        private float m_totalDuration;
        private Coroutine m_playCoroutine;

        internal delegate void OnPlayHanlde(Storyboard sender);
        internal event OnPlayHanlde OnPlay;

        protected override void OnAddChild(DependencyObject childObj)
        {
            switch (childObj)
            {
                case Animation anim:
                    m_animations.Add(anim);
                    break;
            }
        }


        public void Play()
        {
            Stop();

            m_totalDuration = m_animations.Max(ani => ani.Offset + ani.Duration);
            m_playCoroutine = Document.StartCoroutine(PlayFlow());

            OnPlay?.Invoke(this);
        }

        private IEnumerator PlayFlow()
        {
            m_currentTime = 0;
            foreach (var anim in m_animations)
            {
                anim.StageDefaultValue();
            }

            do
            {
                m_currentTime += Time.deltaTime;
                m_currentTime = Mathf.Clamp(m_currentTime, 0, m_totalDuration);

                foreach (var anim in m_animations)
                {
                    if (!anim.Evalution(m_currentTime, out object newValue)) continue;
                    anim.ApplyValue(newValue);
                }

                yield return null;
            }
            while (m_currentTime < m_totalDuration);

            m_playCoroutine = null;
        }

        internal void Stop()
        {
            if (m_playCoroutine != null)
            {
                Document.StopCoroutine(m_playCoroutine);
                m_playCoroutine = null;
            }
        }
    }
}
