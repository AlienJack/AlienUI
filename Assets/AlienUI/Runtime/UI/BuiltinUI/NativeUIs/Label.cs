using AlienUI.Models;
using AlienUI.Models.Attributes;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;
using UnityEngine.UI;

namespace AlienUI.UIElements
{
    [Description(Icon = "label")]
    public class Label : ContentPresent<string>
    {
        public Font Font
        {
            get { return (Font)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }
        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register("Font", typeof(Font), typeof(Label), new PropertyMetadata(Settings.Get().DefaultLabelFont), OnFontChanged);

        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Label), new PropertyMetadata(Color.black), OnColorChange);

        public TextAlignHorizontal AlignHorizontal
        {
            get { return (TextAlignHorizontal)GetValue(AlignHorizontalProperty); }
            set { SetValue(AlignHorizontalProperty, value); }
        }
        public static readonly DependencyProperty AlignHorizontalProperty =
            DependencyProperty.Register("AlignHorizontal", typeof(TextAlignHorizontal), typeof(Label), new PropertyMetadata(TextAlignHorizontal.Middle), OnAlignChanged);

        public TextAlignVertical AlignVertical
        {
            get { return (TextAlignVertical)GetValue(AlignVerticalProperty); }
            set { SetValue(AlignVerticalProperty, value); }
        }
        public static readonly DependencyProperty AlignVerticalProperty =
            DependencyProperty.Register("AlignVertical", typeof(TextAlignVertical), typeof(Label), new PropertyMetadata(TextAlignVertical.Middle), OnAlignChanged);

        public int FontSize
        {
            get { return (int)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register("FontSize", typeof(int), typeof(Label), new PropertyMetadata(32), OnFontSizeChanged);

        private static void OnFontSizeChanged(DependencyObject sender, object oldValue, object newValue)
        {
            (sender as Label).m_text.fontSize = (int)newValue;
        }

        private static void OnColorChange(DependencyObject sender, object oldValue, object newValue)
        {
            (sender as Label).m_text.color = (Color)newValue;
        }


        public bool AutoWarp
        {
            get { return (bool)GetValue(AutoWarpProperty); }
            set { SetValue(AutoWarpProperty, value); }
        }
        public static readonly DependencyProperty AutoWarpProperty =
            DependencyProperty.Register("AutoWarp", typeof(bool), typeof(Label), new PropertyMetadata(false), OnAutoWarpChanged);

        public bool Truncate
        {
            get { return (bool)GetValue(TruncateProperty); }
            set { SetValue(TruncateProperty, value); }
        }

        public static readonly DependencyProperty TruncateProperty =
            DependencyProperty.Register("Truncate", typeof(bool), typeof(Label), new PropertyMetadata(false), OnTruncateChanged);

        private Text m_text;

        public Text UGUIText => m_text;

        protected override void OnInitialized()
        {
            m_text = m_rectTransform.gameObject.AddComponent<Text>();
        }

        protected override Vector2 CalcDesireSize()
        {
            return new Vector2(m_text.preferredWidth, m_text.preferredHeight);
        }

        private static void OnFontChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Label;

            self.m_text.font = (Font)newValue;
            OnLayoutParamDirty(sender, oldValue, newValue);
        }

        protected override void OnContentChanged(string oldValue, string newValue)
        {
            m_text.text = Content;
            SetLayoutDirty();
        }

        private static void OnAlignChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Label;

            self.m_text.alignment = AlienUtility.ConvertToTextAnchor(self.AlignHorizontal, self.AlignVertical);

            self.SetLayoutDirty();
        }


        private static void OnAutoWarpChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Label;
            self.m_text.horizontalOverflow = self.AutoWarp ? HorizontalWrapMode.Wrap : HorizontalWrapMode.Overflow;

            self.SetLayoutDirty();
        }

        private static void OnTruncateChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Label;
            self.m_text.verticalOverflow = self.Truncate ? VerticalWrapMode.Truncate : VerticalWrapMode.Overflow;

            self.SetLayoutDirty();
        }
    }
}
