using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public class BlurEffect : Effect
    {
        protected override Shader GetShader()
        {
            return Settings.Get().GetUnityAsset<Shader>("Builtin", "Blur");
        }

        public float BlurFactor
        {
            get { return (float)GetValue(BlurFactorProperty); }
            set { SetValue(BlurFactorProperty, value); }
        }

        public static readonly DependencyProperty BlurFactorProperty =
            DependencyProperty.Register("BlurFactor", typeof(float), typeof(BlurEffect), new PropertyMetadata(0f), OnBlurSizeChanged);

        private static void OnBlurSizeChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as BlurEffect;
            self.m_mat.SetFloat("_BlurFactor", self.BlurFactor);
        }
    }
}
