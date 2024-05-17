using AlienUI.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public class Storyboard : Resource
    {
        private float m_currentTime;
        private float m_totalDuration;
        private Coroutine m_playCoroutine;

        internal delegate void OnPlayHanlde(Storyboard sender);
        internal event OnPlayHanlde OnPlay;

        public void Play()
        {
            Stop();

            m_totalDuration = GetChildren<Animation>().Max(ani => ani.Offset + ani.Duration);
            m_playCoroutine = Document.StartCoroutine(PlayFlow());

            OnPlay?.Invoke(this);
        }

        private IEnumerator PlayFlow()
        {
            m_currentTime = 0;
            var animations = GetChildren<Animation>();
            foreach (var anim in animations)
            {
                anim.StageDefaultValue();
            }

            do
            {
                m_currentTime += Time.deltaTime;
                m_currentTime = Mathf.Clamp(m_currentTime, 0, m_totalDuration);

                foreach (var anim in animations)
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
