using AlienUI.Core.Commnands;
using AlienUI.Models;
using AlienUI.Models.Attributes;

namespace AlienUI.UIElements
{
    [Description(Icon = "button")]
    public class Button : UserControl
    {
        protected override ControlTemplate DefaultTemplate => new("Builtin.Button");

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Button), new PropertyMetadata("BUTTON_TEXT"));

        public bool Pressed
        {
            get { return (bool)GetValue(PressedProperty); }
            set { SetValue(PressedProperty, value); }
        }

        public static readonly DependencyProperty PressedProperty =
            DependencyProperty.Register("Pressed", typeof(bool), typeof(Button), new PropertyMetadata(false).SetNotAllowEdit());

        public CommandBase Command
        {
            get { return (CommandBase)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(CommandBase), typeof(Button), new PropertyMetadata(null));



        public EnumButtonState State
        {
            get { return (EnumButtonState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(EnumButtonState), typeof(Button), new PropertyMetadata(EnumButtonState.Normal));

        public bool Interactable
        {
            get { return (bool)GetValue(InteractableProperty); }
            set { SetValue(InteractableProperty, value); }
        }

        public static readonly DependencyProperty InteractableProperty =
            DependencyProperty.Register("Interactable", typeof(bool), typeof(Button), new PropertyMetadata(true));


        protected override void OnInitialized()
        {
            base.OnInitialized();

            OnPointerClick += Button_OnPointerClick;
            OnPointerDown += Button_OnPointerDown;
            OnPointerUp += Button_OnPointerUp;

            OnDependencyPropertyChanged += Button_OnDependencyPropertyChanged;
            OnDrag += Button_OnDrag; //handle this to stop drag event send to parent
        }

        private void Button_OnDrag(object sender, Events.OnDragEvent e) { }

        private void Button_OnDependencyPropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            if (dp == IsPointerHoverProperty || dp == InteractableProperty || dp == PressedProperty || dp == InteractableProperty)
            {
                UpdateButtonState();
            }
        }

        private void Button_OnPointerUp(object sender, Events.OnPointerUpEvent e)
        {
            Pressed = false;
        }

        private void Button_OnPointerDown(object sender, Events.OnPointerDownEvent e)
        {
            Pressed = true;
        }

        private void UpdateButtonState()
        {
            if (!Interactable) State = EnumButtonState.Disabled;
            else if (Pressed) State = EnumButtonState.Pressing;
            else if (IsPointerOver) State = EnumButtonState.Hover;
            else State = EnumButtonState.Normal;
        }


        private void Button_OnPointerClick(object sender, Events.OnPointerClickEvent e)
        {
            if (Interactable) Command?.Execute();
        }

        public enum EnumButtonState
        {
            Normal, Hover, Pressing, Disabled
        }
    }
}
