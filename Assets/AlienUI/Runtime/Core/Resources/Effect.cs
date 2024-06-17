using AlienUI.Models;
using AlienUI.UIElements;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public abstract class Effect : Resource
    {
        public Shader Shader
        {
            get { return (Shader)GetValue(ShaderProperty); }
            set { SetValue(ShaderProperty, value); }
        }

        public static readonly DependencyProperty ShaderProperty =
            DependencyProperty.Register("Shader", typeof(Shader), typeof(Effect), new PropertyMetadata(Settings.Get().GetUnityAsset<Shader>("Builtin", "Blur")), OnShaderChanged);

        private static void OnShaderChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Effect;

            if (self.Parent is VisualElement ve)
            {
                if (self.Shader != null)
                    ve.NodeProxy.AddMaterialmodifier(new Material(self.Shader));
                else
                    ve.NodeProxy.AddMaterialmodifier(null);
            }
        }

        protected override void OnParentRemoved(AmlNodeElement removedParent)
        {
            if (removedParent is VisualElement ve)
            {
                ve.NodeProxy.RemoveMaterialModifier();
            }
        }
    }
}
