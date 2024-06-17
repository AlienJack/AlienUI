using UnityEngine;
using UnityEngine.UI;

namespace AlienUI
{
    public class MaterialModifier : MonoBehaviour, IMaterialModifier
    {
        private Material _material;
        public void SetMaterial(Material mat)
        {
            if (mat == null)
                _material = null;
            else
                _material = new Material(mat);
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            if (_material != null) return _material;
            else return baseMaterial;
        }
    }
}
