using AlienUI.Core.Resources;
using AlienUI.Models;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI
{
    public class StoryboardGroup : Resource
    {
        private List<Storyboard> stories = new List<Storyboard>();
        private Storyboard m_currentPlay;

        protected override void OnAddChild(DependencyObject childObj)
        {
            switch (childObj)
            {
                case Storyboard sb: stories.Add(sb); sb.OnPlay += Sb_OnPlay; break;
            }
        }


        
        private void Sb_OnPlay(Storyboard sender)
        {
            if (m_currentPlay != null)
                m_currentPlay.Stop();

            m_currentPlay = sender;
        }
    }
}
