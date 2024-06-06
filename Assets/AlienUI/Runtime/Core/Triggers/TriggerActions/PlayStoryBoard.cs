using AlienUI.Core.Resources;
using AlienUI.Core.Triggers;
using AlienUI.Models;

namespace AlienUI.Core.Resources
{
    public class PlayStoryboard : TriggerAction
    {
        public DependencyObjectRef Target
        {
            get { return (DependencyObjectRef)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(DependencyObjectRef), typeof(PlayStoryboard), new PropertyMetadata(default(DependencyObjectRef), "Data"));


        public override bool Excute()
        {
            var sbIns = Target.Get(this) as Storyboard;
            if (sbIns == null) return true;

            sbIns.Play();

            return true;
        }
    }
}
