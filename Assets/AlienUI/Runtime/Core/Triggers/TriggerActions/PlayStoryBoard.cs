using AlienUI.Models;
using UnityEngine;

namespace AlienUI.Core.Triggers
{
    public class PlayStoryBoard : TriggerAction
    {
        public DependencyObjectRef Storyboard
        {
            get { return (DependencyObjectRef)GetValue(StoryboardProperty); }
            set { SetValue(StoryboardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Storyboard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StoryboardProperty =
            DependencyProperty.Register("Storyboard", typeof(DependencyObjectRef), typeof(PlayStoryBoard), default(DependencyObjectRef));


        public override void Excute()
        {
            var sbIns = Storyboard.Get(this) as AlienUI.Core.Resources.Storyboard;
            if (sbIns == null) return;

            sbIns.Play();
        }
    }
}
