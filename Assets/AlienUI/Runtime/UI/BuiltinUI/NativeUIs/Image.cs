using AlienUI.Models;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UGUIImg = UnityEngine.UI.Image;
using ImgType = UnityEngine.UI.Image.Type;

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



        public ImgType ImageType
        {
            get { return (ImgType)GetValue(ImageTypeProperty); }
            set { SetValue(ImageTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageTypeProperty =
            DependencyProperty.Register("ImageType", typeof(ImgType), typeof(Image), ImgType.Simple, OnTypeChanged);

        private static void OnTypeChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Image;
            self.img.type = (ImgType)newValue;
        }

        private UGUIImg img;

        protected override void OnInitialized()
        {
            img = m_rectTransform.gameObject.AddComponent<UGUIImg>();
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