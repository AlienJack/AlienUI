using AlienUI.Models;
using AlienUI.Models.Attributes;
using System;
using System.Linq;
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

        protected override void OnInitialized() { }

        protected override Vector2 CalcDesireSize()
        {
            var rect = new Vector2(Padding.left + Padding.right, Padding.top + Padding.bottom);

            Vector2 addRect = default;
            foreach (var child in UIChildren)
            {
                var deSize = child.GetDesireSize();
                addRect.x = Mathf.Max(addRect.x, deSize.x);
                addRect.y = Mathf.Max(addRect.y, deSize.y);
            }

            return rect + addRect;
        }
    }
}
