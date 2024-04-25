using AlienUI.Models;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AlienUI.UIElements
{
    public class Image : ContentPresent<Sprite>
    {
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Image), Color.white, OnColorChanged);


        private UnityEngine.UI.Image img;

        protected override void OnInitialized()
        {
            img = m_rectTransform.gameObject.AddComponent<UnityEngine.UI.Image>();
        }

        protected override void OnContentChanged(Sprite oldValue, Sprite newValue)
        {
            img.sprite = newValue;
        }

        private static void OnColorChanged(DependencyObject sender, object oldValue, object newValue)
        {
            (sender as Image).img.color = (Color)newValue;
        }

        protected override Float2 CalcDesireSize()
        {
            return new Float2 { x = img.preferredWidth, y = img.preferredHeight };
        }
    }
}