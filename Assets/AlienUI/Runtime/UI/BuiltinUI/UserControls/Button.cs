using AlienUI.Models;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Button : UserControl
    {
        protected override TextAsset DefaultTemplate => Engine.Settings.GetTemplate("Builtin.Button");

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Button), "BUTTON_TEXT");


    }
}
