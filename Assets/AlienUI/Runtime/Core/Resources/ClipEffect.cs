using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public class ClipEffect : Effect
    {
        protected override Shader GetShader()
        {
            return Settings.Get().GetUnityAsset<Shader>("Builtin", "Clip");
        }

        public float Clip
        {
            get { return (float)GetValue(ClipProperty); }
            set { SetValue(ClipProperty, value); }
        }

        public static readonly DependencyProperty ClipProperty =
            DependencyProperty.Register("Clip", typeof(float), typeof(ClipEffect), new PropertyMetadata(0.5f), OnClipChanged);

        private static void OnClipChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as ClipEffect;
            self.m_mat.SetFloat("_Clip", self.Clip);
        }
    }
}
