using AlienUI.Models;
using AlienUI.Models.Attributes;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;
using UnityEngine.UI;
using ImgType = UnityEngine.UI.Image.Type;
using UGUIImg = UnityEngine.UI.Image;

namespace AlienUI.UIElements
{
    [Description(Icon = "image")]
    public class Image : ContentPresent<Sprite>
    {
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Image), new PropertyMetadata(Color.white), OnColorChanged);



        public bool Mask
        {
            get { return (bool)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(bool), typeof(Image), new PropertyMetadata(false), OnMaskChanged);

        private static void OnMaskChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Image;
            if (self.Mask)
            {
                if (self.mask == null) self.mask = self.Rect.gameObject.AddComponent<Mask>();
                self.mask.showMaskGraphic = true;
                self.mask.enabled = true;
            }
            else
            {
                if (self.mask != null)
                {
                    self.mask.showMaskGraphic = true;
                    self.mask.enabled = false;
                }
            }
        }

        public ImgType ImageType
        {
            get { return (ImgType)GetValue(ImageTypeProperty); }
            set { SetValue(ImageTypeProperty, value); }
        }

        public static readonly DependencyProperty ImageTypeProperty =
            DependencyProperty.Register("ImageType", typeof(ImgType), typeof(Image), new PropertyMetadata(ImgType.Simple), OnTypeChanged);

        private static void OnTypeChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Image;
            self.img.type = (ImgType)newValue;
        }

        private UGUIImg img;
        private Mask mask;

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

        protected override Vector2 CalcDesireSize()
        {
            return new Vector2 { x = img.preferredWidth, y = img.preferredHeight };
        }
    }
}