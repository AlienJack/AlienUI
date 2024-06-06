using AlienUI.Core.Triggers;

namespace AlienUI.Core.Resources
{
    public abstract class TriggerAction : Resource
    {
        public virtual void OnInit(Trigger trigger) { }
        public abstract bool Excute();
    }
}
