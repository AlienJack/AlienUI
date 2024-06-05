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
        public bool Mask
        {
            get { return (bool)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(bool), typeof(Image), new PropertyMetadata(false), OnMaskChanged);

        private static void OnMaskChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Image;
            if (self.Mask)
            {
                if (self.nativecom_mask == null) self.nativecom_mask = self.Rect.gameObject.AddComponent<Mask>();
                self.nativecom_mask.showMaskGraphic = true;
                self.nativecom_mask.enabled = true;
            }
            else
            {
                if (self.nativecom_mask != null)
                {
                    self.nativecom_mask.showMaskGraphic = true;
                    self.nativecom_mask.enabled = false;
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
            self.naticecom_img.type = (ImgType)newValue;
        }

        private UGUIImg naticecom_img;
        private Mask nativecom_mask;

        protected override void OnInitialized()
        {
            naticecom_img = m_rectTransform.gameObject.AddComponent<UGUIImg>();
        }

        protected override void OnContentChanged(Sprite oldValue, Sprite newValue)
        {
            naticecom_img.sprite = newValue;
        }

        protected override Vector2 CalcDesireSize()
        {
            return new Vector2 { x = naticecom_img.preferredWidth, y = naticecom_img.preferredHeight };
        }
    }
}