using AlienUI.Core.Resources;
using AlienUI.Models;
using AlienUI.Models.Attributes;
using System;
using UnityEditor.UI;
using UnityEngine;

namespace AlienUI.UIElements
{
    [Description(Icon = "ve")]
    [AllowChild(typeof(Effect))]
    public class VisualElement : UIElement
    {
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(VisualElement), new PropertyMetadata(Color.white), OnColorChanged);


        private static void OnColorChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as VisualElement;
            self.NodeProxy.Color = (Color)newValue;
        }

        protected override void OnInitialized() { }
    }
}
