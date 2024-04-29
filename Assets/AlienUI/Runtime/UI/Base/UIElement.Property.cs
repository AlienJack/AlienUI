using AlienUI.Models;

namespace AlienUI.UIElements
{
    public partial class UIElement : DependencyObject
    {
        public bool IsPointerHover
        {
            get { return (bool)GetValue(IsPointerHoverProperty); }
            set { SetValue(IsPointerHoverProperty, value); }
        }
        public static readonly DependencyProperty IsPointerHoverProperty =
            DependencyProperty.Register("IsPointerHover", typeof(bool), typeof(UIElement), false);



        public float Alpha
        {
            get { return (float)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }
        public static readonly DependencyProperty AlphaProperty =
            DependencyProperty.Register("Alpha", typeof(float), typeof(UIElement), 1f, OnAlphaChanged);

        private static void OnAlphaChanged(DependencyObject sender, object oldValue, object newValue)
        {
            (sender as UIElement).NodeProxy.Alpha = (float)newValue;
        }

        public UIElement()
        {
            OnPointerEnter += UIElement_OnPointerEnter;
            OnPointerExit += UIElement_OnPointerExit;
        }

        private void UIElement_OnPointerEnter(object sender, Events.OnPointerEnterEvent e)
        {
            var self = sender as UIElement;
            self.IsPointerHover = true;
        }
        private void UIElement_OnPointerExit(object sender, Events.OnPointerExitEvent e)
        {
            var self = sender as UIElement;
            self.IsPointerHover = false;
        }
    }
}