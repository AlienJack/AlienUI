using AlienUI.Models;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AlienUI.UIElements
{
    public class Label : ContentPresent<string>
    {
        public Font Font
        {
            get { return (Font)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Label), Color.black, OnColorChange);

        private static void OnColorChange(DependencyObject sender, object oldValue, object newValue)
        {
            (sender as Label).m_text.color = (Color)newValue;
        }

        // Using a DependencyProperty as the backing store for Font.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register("Font", typeof(Font), typeof(Label), null, OnFontChanged);

        private Text m_text;

        protected override void OnInitialized()
        {
            m_text = m_rectTransform.gameObject.AddComponent<Text>();
        }

        protected override Float2 CalcDesireSize()
        {
            return new Float2(m_text.preferredWidth, m_text.preferredHeight);
        }

        private static void OnFontChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Label;

            self.m_text.font = (Font)newValue ?? self.Engine.Settings.DefaultLabelFont;
            OnLayoutParamDirty(sender, oldValue, newValue);
        }

        protected override void OnContentChanged(string oldValue, string newValue)
        {
            m_text.text = Content;
        }
    }
}
