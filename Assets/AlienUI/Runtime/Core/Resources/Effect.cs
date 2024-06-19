using AlienUI.Models;
using AlienUI.UIElements;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public abstract class Effect : Resource
    {
        protected abstract Shader GetShader();
        protected Material m_mat;

        public Effect()
        {
            if (GetShader() is Shader shader)
                m_mat = new Material(shader);
        }

        protected override void OnDocumentPerformed()
        {
            if (Parent is VisualElement ve)
                ve.NodeProxy.AddMaterialmodifier(m_mat);
        }

        protected override void OnParentSet(AmlNodeElement parent)
        {
            if (parent is VisualElement ve && ve.NodeProxy != null)
            {
                ve.NodeProxy.AddMaterialmodifier(m_mat);
            }
        }

        protected override void OnParentRemoved(AmlNodeElement removedParent)
        {
            if (removedParent is VisualElement ve)
            {
                ve.NodeProxy.RemoveMaterialModifier(m_mat);
            }
        }
    }
}
