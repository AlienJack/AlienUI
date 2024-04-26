using AlienUI.Models;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public class Storyboard : Resource
    {
        private List<Animation> m_animations = new List<Animation>();

        public override void AddChild(DependencyObject childObj)
        {
            switch (childObj)
            {
                case Animation anim:
                    m_animations.Add(anim);
                    break;
            }
        }
    }
}
