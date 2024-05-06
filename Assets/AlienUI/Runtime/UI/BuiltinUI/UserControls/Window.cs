using AlienUI.Core.Commnands;
using AlienUI.Models;
using UnityEngine;
using UnityEngine.Events;

namespace AlienUI.UIElements
{
    public class Window : UserControl
    {
        protected override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.Window");

        public Command CloseCmd
        {
            get { return (Command)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("CloseCmd", typeof(Command), typeof(Window), new Command());

        protected override void OnInitialized()
        {
            base.OnInitialized();

            CloseCmd.OnExecute += Close_OnExecute;
        }

        private void Close_OnExecute()
        {
            Close();
        }
    }
}
