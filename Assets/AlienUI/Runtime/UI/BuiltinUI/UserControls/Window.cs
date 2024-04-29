using AlienUI.Models;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Window : UserControl
    {
        protected override TextAsset DefaultTemplate => Engine.Settings.GetTemplate("Builtin.Window");

        public float BorderWidth
        {
            get { return (float)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(float), typeof(Window), 1f, OnBorderWidthChanged);

        private static void OnBorderWidthChanged(DependencyObject sender, object oldValue, object newValue)
        {
            (sender as Window).Padding = new Border((float)newValue);
        }
    }
}
