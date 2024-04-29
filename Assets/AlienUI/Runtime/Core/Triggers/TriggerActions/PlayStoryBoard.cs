using AlienUI.Core.Resources;
using AlienUI.Models;

namespace AlienUI.Core.Triggers
{
    public class PlayStoryboard : TriggerAction
    {
        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(PlayStoryboard), default(DependencyObjectRef));


        public override void Excute()
        {
            var sbIns = Target.Get(this) as Storyboard;
            if (sbIns == null) return;

            sbIns.Play();
        }
    }
}
