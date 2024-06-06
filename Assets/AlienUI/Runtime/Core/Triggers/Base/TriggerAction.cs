using AlienUI.Core.Resources;

namespace AlienUI.Core.Triggers
{
    public abstract class TriggerAction : Resource
    {
        public virtual void OnInit(Trigger trigger) { }
        public abstract bool Excute();
    }
}
