using AlienUI.Core.Resources;
using AlienUI.UIElements;
using System.Collections.Generic;

namespace AlienUI
{
    public class StoryboardGroup : Resource
    {
        private Storyboard m_currentPlay;

        protected override void OnAddChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case Storyboard sb: sb.OnPlay += Sb_OnPlay; break;
            }
        }

        protected override void OnRemoveChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case Storyboard sb: sb.OnPlay -= Sb_OnPlay; break;
            }
        }

        private void Sb_OnPlay(Storyboard sender)
        {
            if (m_currentPlay != null && sender != m_currentPlay)
            {
                m_currentPlay.Stop();
            }
            m_currentPlay = sender;
        }
    }
}
