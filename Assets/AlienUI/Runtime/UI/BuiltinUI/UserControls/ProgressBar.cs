using AlienUI.Models;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class ProgressBar : UserControl
    {
        public override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.Slider");

        public float MaxValue
        {
            get { return (float)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(float), typeof(ProgressBar), new PropertyMetadata(1f), OnMaxValueChanged);
        private static void OnMaxValueChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as ProgressBar;
            self.MaxValue = Mathf.Clamp(self.MaxValue, 0f, float.MaxValue);
            self.Value = Mathf.Clamp(self.Value, 0f, self.MaxValue);

            self.GapValue = self.MaxValue - self.Value;
        }

        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(float), typeof(ProgressBar), new PropertyMetadata(0.5f), OnValueChanged);

        private static void OnValueChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as ProgressBar;
            self.Value = Mathf.Clamp(self.Value, 0f, self.MaxValue);
            self.GapValue = self.MaxValue - self.Value;
        }

        public float GapValue
        {
            get { return (float)GetValue(GapValueProperty); }
            set { SetValue(GapValueProperty, value); }
        }

        public static readonly DependencyProperty GapValueProperty =
            DependencyProperty.Register("GapValue", typeof(float), typeof(ProgressBar), new PropertyMetadata(0.5f));

    }
}
