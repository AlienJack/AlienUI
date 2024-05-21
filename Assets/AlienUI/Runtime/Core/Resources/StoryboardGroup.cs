using AlienUI.Models.Attributes;
using AlienUI.UIElements;

namespace AlienUI.Core.Resources
{
    [AllowChild(typeof(Storyboard))]
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
