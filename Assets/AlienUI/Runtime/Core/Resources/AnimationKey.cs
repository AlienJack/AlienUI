using AlienUI.Models;

namespace AlienUI.Core.Resources
{
    public class AnimationKey : Resource
    {
        public float Time
        {
            get { return (float)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(float), typeof(AnimationKey), 0);


        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(string), typeof(AnimationKey), string.Empty);

        public object ActualValue;
    }
}
