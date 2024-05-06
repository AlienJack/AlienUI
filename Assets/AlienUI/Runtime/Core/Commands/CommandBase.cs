using System;
using UnityEngine;

namespace AlienUI.Core.Commnands
{
    public abstract class CommandBase
    {
        public void Execute(params object[] paramerters)
        {
            OnInternalExecute(paramerters);
        }

        protected abstract void OnInternalExecute(params object[] paramerters);
    }

    public class Command : CommandBase
    {
        public void Execute()
        {
            Execute(null);
        }

        public event Action OnExecute;
        protected override void OnInternalExecute(params object[] paramerters)
        {
            OnExecute?.Invoke();
        }
    }

    public class Commnad<T> : CommandBase
    {
        public void Execute(T param)
        {
            Execute(param);
        }

        public event Action<T> OnExecute;

        protected override void OnInternalExecute(params object[] paramerters)
        {
            OnExecute?.Invoke((T)paramerters[0]);
        }
    }
}
