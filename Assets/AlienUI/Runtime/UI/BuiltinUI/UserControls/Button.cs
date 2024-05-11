using AlienUI.Core.Commnands;
using AlienUI.Models;

namespace AlienUI.UIElements
{
    public class Button : UserControl
    {
        protected override ControlTemplate DefaultTemplate => new("Builtin.Button");

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Button), new PropertyMeta("BUTTON_TEXT"));

        public bool Pressed
        {
            get { return (bool)GetValue(PressedProperty); }
            set { SetValue(PressedProperty, value); }
        }

        public static readonly DependencyProperty PressedProperty =
            DependencyProperty.Register("Pressed", typeof(bool), typeof(Button), new PropertyMeta(false));

        public CommandBase Command
        {
            get { return (CommandBase)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(CommandBase), typeof(Button), new PropertyMeta(null));

        protected override void OnInitialized()
        {
            base.OnInitialized();

            OnPointerClick += Button_OnPointerClick;
            OnPointerDown += Button_OnPointerDown;
            OnPointerUp += Button_OnPointerUp;
        }

        private void Button_OnPointerUp(object sender, Events.OnPointerUpEvent e)
        {
            Pressed = false;
        }

        private void Button_OnPointerDown(object sender, Events.OnPointerDownEvent e)
        {
            Pressed = true;
        }

        private void Button_OnPointerClick(object sender, Events.OnPointerClickEvent e)
        {
            if (Command != null)
                Command.Execute();
        }
    }
}
