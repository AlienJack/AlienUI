using AlienUI.Core.Commnands;
using AlienUI.Models;
using AlienUI.Models.Attributes;

namespace AlienUI.UIElements
{
    [Description(Icon = "window")]
    public class Window : UserControl
    {
        protected override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.Window");

        public Command CloseCmd
        {
            get { return (Command)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("CloseCmd", typeof(Command), typeof(Window), new PropertyMetadata(new Command()).ReadOnly().SetNotAllowEdit());


        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Window), new PropertyMetadata(nameof(Window)));


        protected override void OnInitialized()
        {
            base.OnInitialized();

            CloseCmd.OnExecute += Close_OnExecute;
            OnDrag += Window_OnDrag;
        }

        private void Window_OnDrag(object sender, Events.OnDragEvent e)
        {
            Offset += e.EvtData.delta / NodeProxy.Canvas.scaleFactor;
        }

        private void Close_OnExecute()
        {
            Close();
        }
    }
}
