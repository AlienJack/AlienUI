using AlienUI.Models;
using UnityEngine;

namespace AlienUI.Core.Triggers
{
    public abstract class TriggerAction : DependencyObject
    {
        public abstract void Excute();

        public override void AddChild(DependencyObject childObj)
        {
            return;
        }
    }
}
