using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public class CircleMaskEffect : Effect
    {
        protected override Shader GetShader()
        {
            return Settings.Get().GetUnityAsset<Shader>("Builtin", "CircleMask");
        }

        public Vector2 Radius
        {
            get { return (Vector2)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }
        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(Vector2), typeof(CircleMaskEffect), new PropertyMetadata(Vector2.one), OnRadiusChanged);

        private static void OnRadiusChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as CircleMaskEffect;
            self.m_mat.SetFloat("_RadiusX", self.Radius.x);
            self.m_mat.SetFloat("_RadiusY", self.Radius.y);
        }

        public Vector2 Scale
        {
            get { return (Vector2)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(Vector2), typeof(CircleMaskEffect), new PropertyMetadata(Vector2.one), OnScaleChanged);

        private static void OnScaleChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as CircleMaskEffect;
            self.m_mat.SetFloat("_ScaleX", self.Scale.x);
            self.m_mat.SetFloat("_ScaleY", self.Scale.y);
        }
    }
}
