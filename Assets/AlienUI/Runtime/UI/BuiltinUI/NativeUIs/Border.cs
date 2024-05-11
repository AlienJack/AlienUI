using AlienUI.Models;
using AlienUI.UIElements.ToolsScript;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Border : UIElement
    {
        private Rectangle m_border;
        private Rectangle m_front;

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Color), typeof(Border), new PropertyMeta(Color.black), OnBorderColorChanged);

        private static void OnBorderColorChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Border;
            self.m_border.color = (Color)newValue;
        }

        public Color FrontColor
        {
            get { return (Color)GetValue(FrontColorProperty); }
            set { SetValue(FrontColorProperty, value); }
        }
        public static readonly DependencyProperty FrontColorProperty =
            DependencyProperty.Register("FrontColor", typeof(Color), typeof(Border), new PropertyMeta(Color.white), OnFrontColorChanged);

        private static void OnFrontColorChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Border;
            self.m_front.color = (Color)newValue;
        }

        public float BorderWidth
        {
            get { return (float)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(float), typeof(Border), new PropertyMeta(2f), OnBorderWidthChanged);

        private static void OnBorderWidthChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Border;
            self.m_border.Stroke = (float)newValue;
            self.m_border.SetAllDirty();
        }



        public int Division
        {
            get { return (int)GetValue(DivisionProperty); }
            set { SetValue(DivisionProperty, value); }
        }

        public static readonly DependencyProperty DivisionProperty =
            DependencyProperty.Register("Division", typeof(int), typeof(Border), new PropertyMeta(20), OnDivisionChanged);

        private static void OnDivisionChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Border;
            self.m_border.Divisions = (int)newValue;
            self.m_front.Divisions = (int)newValue;
            self.m_border.SetAllDirty();
            self.m_front.SetAllDirty();
        }

        public float BorderRadius
        {
            get { return (float)GetValue(BorderRadiusProperty); }
            set { SetValue(BorderRadiusProperty, value); }
        }
        public static readonly DependencyProperty BorderRadiusProperty =
            DependencyProperty.Register("BorderRadius", typeof(float), typeof(Border), new PropertyMeta(5f), OnRadiusChanged);

        private static void OnRadiusChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Border;
            self.m_border.Radius = (float)newValue;
            self.m_front.Radius = (float)newValue;
            self.m_border.SetAllDirty();
            self.m_front.SetAllDirty();
        }



        public float AA
        {
            get { return (float)GetValue(AAProperty); }
            set { SetValue(AAProperty, value); }
        }
        public static readonly DependencyProperty AAProperty =
            DependencyProperty.Register("AA", typeof(float), typeof(Border), new PropertyMeta(0.5f), OnAntiAliasChanged);

        private static void OnAntiAliasChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Border;
            self.m_border.AntiAlias = (float)newValue;
            self.m_front.AntiAlias = (float)newValue;
            self.m_border.SetAllDirty();
            self.m_front.SetAllDirty();
        }

        protected override void OnInitialized()
        {
            m_front = m_rectTransform.gameObject.AddComponent<Rectangle>();
            m_front.StrokeMode = false;

            var borderRect = AlienUtility.CreateEmptyUIGameObject("[Border]").transform as RectTransform;
            borderRect.SetParent(m_rectTransform, false);
            borderRect.anchorMin = new Vector2(0, 0);
            borderRect.anchorMax = new Vector2(1, 1);
            borderRect.pivot = new Vector2(0.5f, 0.5f);
            borderRect.sizeDelta = Vector2.zero;
            borderRect.anchoredPosition = Vector2.zero;
            borderRect.SetParent(m_rectTransform);
            borderRect.SetStretchModeOffsets(-1, -1, -1, -1);

            m_border = borderRect.gameObject.AddComponent<Rectangle>();
            m_border.StrokeMode = true;
        }

        protected override Vector2 CalcDesireSize()
        {
            return new Vector2(100, 100);
        }
    }
}
