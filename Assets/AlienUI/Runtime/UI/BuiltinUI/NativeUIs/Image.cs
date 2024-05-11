using AlienUI.Models;
using UnityEngine;
using ImgType = UnityEngine.UI.Image.Type;
using UGUIImg = UnityEngine.UI.Image;

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
            DependencyProperty.Register("Color", typeof(Color), typeof(Image), new PropertyMeta(Color.white), OnColorChanged);



        public ImgType ImageType
        {
            get { return (ImgType)GetValue(ImageTypeProperty); }
            set { SetValue(ImageTypeProperty, value); }
        }
        
        public static readonly DependencyProperty ImageTypeProperty =
            DependencyProperty.Register("ImageType", typeof(ImgType), typeof(Image), new PropertyMeta(ImgType.Simple), OnTypeChanged);

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

        protected override Vector2 CalcDesireSize()
        {
            return new Vector2 { x = img.preferredWidth, y = img.preferredHeight };
        }
    }
}