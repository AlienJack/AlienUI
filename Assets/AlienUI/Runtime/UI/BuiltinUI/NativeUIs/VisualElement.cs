using AlienUI.Models;
using AlienUI.Models.Attributes;
using System;
using UnityEngine;

namespace AlienUI.UIElements
{
    [Description(Icon = "ve")]
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

        public Material Effect
        {
            get { return (Material)GetValue(EffectProperty); }
            set { SetValue(EffectProperty, value); }
        }

        public static readonly DependencyProperty EffectProperty =
            DependencyProperty.Register("Effect", typeof(Material), typeof(VisualElement), new PropertyMetadata(null), OnEffectChanged);

        private static void OnEffectChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as VisualElement;
            if (self.Effect == null)
                self.NodeProxy.RemoveMaterialModifier();
            else
                self.NodeProxy.AddMaterialmodifier(self.Effect);
        }

        protected override void OnInitialized() { }
    }
}
