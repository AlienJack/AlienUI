using AlienUI.Models;
using UnityEngine;

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