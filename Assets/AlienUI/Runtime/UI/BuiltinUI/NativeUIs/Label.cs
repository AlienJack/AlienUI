using AlienUI.Models;
using AlienUI.UIElements.ToolsScript;
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
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Label), Color.black, OnColorChange);

        public TextAlignHorizontal AlignHorizontal
        {
            get { return (TextAlignHorizontal)GetValue(AlignHorizontalProperty); }
            set { SetValue(AlignHorizontalProperty, value); }
        }
        public static readonly DependencyProperty AlignHorizontalProperty =
            DependencyProperty.Register("AlignHorizontal", typeof(TextAlignHorizontal), typeof(Label), TextAlignHorizontal.Middle, OnAlignChanged);

        public TextAlignVertical AlignVertical
        {
            get { return (TextAlignVertical)GetValue(AlignVerticalProperty); }
            set { SetValue(AlignVerticalProperty, value); }
        }
        public static readonly DependencyProperty AlignVerticalProperty =
            DependencyProperty.Register("AlignVertical", typeof(TextAlignVertical), typeof(Label), TextAlignVertical.Middle, OnAlignChanged);

        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(Label), 32, OnFontSizeChanged);

        private static void OnFontSizeChanged(DependencyObject sender, object oldValue, object newValue)
        {
            (sender as Label).m_text.fontSize = (int)newValue;
        }

        private static void OnColorChange(DependencyObject sender, object oldValue, object newValue)
        {
            (sender as Label).m_text.color = (Color)newValue;
        }
        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register("Font", typeof(Font), typeof(Label), null, OnFontChanged);

        public bool AutoWarp
        {
            get { return (bool)GetValue(AutoWarpProperty); }
            set { SetValue(AutoWarpProperty, value); }
        }

        public static readonly DependencyProperty AutoWarpProperty =
            DependencyProperty.Register("AutoWarp", typeof(bool), typeof(Label), true, OnAutoWarpChanged);


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

        private static void OnAlignChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Label;

            self.m_text.alignment = Utility.ConvertToTextAnchor(self.AlignHorizontal, self.AlignVertical);

            self.SetLayoutDirty();
        }


        private static void OnAutoWarpChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Label;
            self.m_text.horizontalOverflow = self.AutoWarp ? HorizontalWrapMode.Wrap : HorizontalWrapMode.Overflow;

            self.SetLayoutDirty();
        }
    }
}
